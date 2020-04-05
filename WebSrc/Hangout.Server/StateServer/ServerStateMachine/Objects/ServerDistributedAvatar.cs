using System.Collections.Generic;
using Hangout.Shared;
using System;
using System.Xml;
using log4net;

namespace Hangout.Server
{
	public class ServerDistributedAvatar : ServerDistributedObject
	{
        private static ILog mLogger = LogManager.GetLogger("ServerDistributedAvatar");
        private List<object> mCurrentTelemetryData = null;
        private readonly AvatarId mAvatarId;
        private string mAvatarName = "";
        private readonly ServerAssetRepository mServerAssetRepository;
        private readonly ServerAccount mServerAccount;
        private static readonly uint mMaxChatLength = uint.Parse(System.Configuration.ConfigurationSettings.AppSettings["StateServerMaxChatLength"]);

        public ServerDistributedAvatar(DistributedObjectId doId, AvatarId avatarId, Guid localSessionId, string nickname, XmlDocument itemIdXml, ServerAccount serverAccount, ServerObjectRepository serverObjectRepository, ServerAssetRepository serverAssetRepository)
            : base(serverObjectRepository, doId)
		{
            mCurrentTelemetryData = new List<object>();
            mObjectType = DistributedObjectTypes.Avatar;
            mAvatarId = avatarId;
            mAvatarName = nickname;
            mServerAccount = serverAccount;
            mServerAssetRepository = serverAssetRepository;

            mObjectData.Add(mObjectType);
            mObjectData.Add("Avatar/Avatar Rig");
			//UNCOMMENT THIS TO SEE WHICH SESSION THIS AVATAR BELONGS TO
			//Console.Write("creating server distributed avatar data with session id: " + localSessionId);
            mObjectData.Add(localSessionId);
            mObjectData.Add(itemIdXml.InnerXml);
            mObjectData.Add(mAvatarName);
		}
		
        protected override void RegisterMessageActions()
		{
			base.RegisterMessageAction((int)MessageSubType.Emoticon, RecvEmoticon);
			base.RegisterMessageAction((int)MessageSubType.Chat, RecvChat);
			base.RegisterMessageAction((int)MessageSubType.Telemetry, RecvTelemetry);
            base.RegisterMessageAction((int)MessageSubType.UpdateDna, RecvDnaUpdate);
			base.RegisterMessageAction((int)MessageSubType.Emote, RecvEmote);
		}
		
		private void RecvChat(Message message)
		{
			string chatText = (string)message.Data[0];

            if (chatText.Length > mMaxChatLength)
            {
                // Either the client is hacked or we have a bug.  Ideally we would boot them here.
                mLogger.Warn("Client sent an excessively large chat string.  Dropping message.  AvID: " + mAvatarId.ToString());
                return;
            }

            // Filter chat and broadcast the message back out
            string filteredChat = TextFilter.Instance.ReplaceNaughtyWords(chatText);
            if (filteredChat != null)
            {
                message.Data[0] = filteredChat;
            }

            ChatLog.LogChatResult(mAvatarId.ToString(), "", chatText, filteredChat);

            BroadcastMessage(message);
		}
		
		private void RecvEmoticon(Message message)
		{
			BroadcastMessage(message);
		}

		private void RecvEmote(Message message)
		{
			BroadcastMessage(message);
		}
		
		private void RecvTelemetry(Message message)
		{
            mCurrentTelemetryData = message.Data;
            BroadcastMessage(message);
		}

        /// <summary>
        /// Received a DNA update from the client.   Save the DNA via the AvatarManagerServiceAPI and broadcast to other clients  
        /// TODO:  Verify the dna is valid
        /// </summary>
        /// <param name="message"></param>
        private void RecvDnaUpdate(Message message)
        {
            string newDnaString = CheckType.TryAssignType<string>(message.Data[0]);
            XmlDocument newDna = new XmlDocument();
            newDna.LoadXml(newDnaString);
            mLogger.Info("RecvDnaUpdate: " + mAvatarId.ToString() + ": " + newDnaString);

            // Get list of ItemIds from DNA
            List<ItemId> itemIds;
            AvatarXmlUtil.GetItemIdsFromAvatarDnaNode(newDna, out itemIds);

            // Get XML of assets from ItemIds
            XmlDocument assetXml = mServerAssetRepository.GetXmlDna(itemIds);

            // Replace the DNA in the message with the asset xml
            message.Data[0] = assetXml.OuterXml;
            mObjectData[3] = assetXml.InnerXml;

            // Broadcast the change out
            BroadcastMessage(message);

            // Save the dna to the DB
            AvatarManagerServiceAPI.UpdateAvatarDna(mAvatarId, newDna, delegate(XmlDocument xmlResponse) 
            {
                mLogger.Info("RecvDnaUpdate, DNA saved: " + mAvatarId.ToString() + "\n" + xmlResponse.OuterXml);
            });
        }

        /// <summary>
        /// Update clients to the avatar's current position
        /// </summary>
        /// <param name="sessionIds"></param>
        public override bool DistributedObjectStateUpdate(out Message outgoingMessage)
        {
            outgoingMessage = null;
            //if the count is 0 that means this avatar object has not received a telemetry update yet
            // therefore don't try and send a message with empty telemetry data!
            if (mCurrentTelemetryData.Count == 0)
            {
                return false;
            }
            outgoingMessage = new Message();
            outgoingMessage.UpdateObjectMessage(true, false, this.DistributedObjectId, (int)MessageSubType.SetPosition, mCurrentTelemetryData);
            return true;
        }
	}
}