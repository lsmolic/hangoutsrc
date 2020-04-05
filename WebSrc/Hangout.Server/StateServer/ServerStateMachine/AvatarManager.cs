using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using Hangout.Server.StateServer;
using System.Xml;
using log4net;

namespace Hangout.Server
{
    public class AvatarManager
    {
        private static ILog mLogger = LogManager.GetLogger("AvatarManager");
        private ServerStateMachine mServerStateMachine;
        private ServerObjectRepository mServerObjectRepository = null;
        private ServerEngine mServerEngine = null;
        private ServerAssetRepository mServerAssetRepository = null;
        private DistributedObjectIdManager mDistributedObjectIdManager = null;
        private Dna mReferenceAvatarDna = null;

        public AvatarManager(ServerStateMachine serverStateMachine)
        {
            mServerStateMachine = serverStateMachine;
            mServerObjectRepository = serverStateMachine.ServerObjectRepository;
            mServerEngine = serverStateMachine.ServerEngine;
            mServerAssetRepository = serverStateMachine.ServerAssetRepository;
            mDistributedObjectIdManager = serverStateMachine.DistributedObjectIdManager;
        }


        public void GetAvatar(Guid sessionId, ZoneId zoneId, ServerAccount serverAccount, AvatarId defaultAvatarId, System.Action<bool> getAvatarFinishedCallback)
        {
            Action<XmlDocument> getAvatarCallback = delegate(XmlDocument xmlResponse)
            {
                XmlNode avatarXmlNode = xmlResponse.SelectSingleNode("Avatars/Avatar");

                //if no avatars were found for this account, create one!
                if(avatarXmlNode == null)
                {
                    CreateNewAvatarForAccount(sessionId, zoneId, serverAccount, defaultAvatarId, getAvatarFinishedCallback);
                }
                //otherwise, just grab the first avatar out of the avatar returned list
                else
                {
                    GetAvatarServiceResponse(avatarXmlNode, serverAccount.Nickname, sessionId, zoneId, getAvatarFinishedCallback);
                }

            };

            AvatarManagerServiceAPI.GetAvatarForUser(serverAccount, getAvatarCallback);
        }

        public void CreateNewAvatarForAccount(Guid sessionId, ZoneId zoneId, ServerAccount serverAccount, AvatarId defaultAvatarId, System.Action<bool> createAvatarFinishedCallback)
        {
            Action<XmlDocument> createAvatarServiceCallback = delegate(XmlDocument xmlResponse)
            {
                XmlNode avatarXmlNode = xmlResponse.SelectSingleNode("Avatars/Avatar");

                if (avatarXmlNode != null)
                {
                    Metrics.Log(LogGlobals.CATEGORY_ACCOUNT, LogGlobals.EVENT_AVATAR_CREATED, LogGlobals.AVATAR_ID_LABEL, defaultAvatarId.ToString(), serverAccount.AccountId.ToString());
					GetAvatarServiceResponse(avatarXmlNode, serverAccount.Nickname, sessionId, zoneId, createAvatarFinishedCallback);
                }
                else
                {
					StateServerAssert.Assert(new System.Exception("Error: Could not create avatar: " + xmlResponse.InnerText));
					createAvatarFinishedCallback(false);
                }
            };
            AvatarManagerServiceAPI.CreateAvatarForUser(serverAccount, defaultAvatarId, createAvatarServiceCallback);
        }

        /// <summary>
        /// Get a reference system avatar.  This avatar is used to fill in missing required info when pulling an avatar from the db
        /// </summary>
        private void GetReferenceAvatar(Action<Dna> gotReferenceAvatarFinished)
        {
            Action<XmlDocument> systemAvatarCallback = delegate(XmlDocument xmlResponse)
            {
                // Get the avatars for the friends without Hangout Avatars
                XmlNode avatarXmlNode = xmlResponse.SelectSingleNode("/Avatars/Avatar[@AvatarId='1']");
                AvatarId avatarId;
                List<ItemId> itemIds = null;
				if (AvatarXmlUtil.GetAvatarInfoFromAvatarXmlNode(avatarXmlNode, out avatarId, out itemIds))
				{
					//use the ServerAssetRepo to composite the List<ItemId> into an XmlNode
					XmlDocument itemsXml = mServerAssetRepository.GetXmlDna(itemIds);

					// Get a list of AssetInfos from this xml
					IEnumerable<AssetInfo> assetInfoList = ServerAssetInfo.Parse(itemsXml);

					// Make dna
					mReferenceAvatarDna = new Dna();
					mReferenceAvatarDna.UpdateDna(assetInfoList);
					gotReferenceAvatarFinished(mReferenceAvatarDna);
					mLogger.Debug("System avatar xml = " + xmlResponse.OuterXml);
				}
				else
				{
					StateServerAssert.Assert(new Exception("Didn't get a valid system avatar for reference avatar, using an empty DNA"));
					mReferenceAvatarDna = new Dna();
					gotReferenceAvatarFinished(mReferenceAvatarDna);
				}
            };
            if (mReferenceAvatarDna == null) 
            {
                AvatarManagerServiceAPI.GetSystemAvatars(systemAvatarCallback);
            }
			else 
			{
				gotReferenceAvatarFinished(mReferenceAvatarDna);
			}

        }

        private void GetAvatarServiceResponse(XmlNode avatarXmlNode, string nickname, Guid sessionId, ZoneId zoneId, Action<bool> gotAvatarServiceResponse)
        {
			List<ItemId> itemIds = null;
            AvatarId avatarId;
            try
            {

                if (AvatarXmlUtil.GetAvatarInfoFromAvatarXmlNode(avatarXmlNode, out avatarId, out itemIds))
                {
                    Dna savedDna = mServerAssetRepository.GetDna(itemIds);

                    // Create an updated dna by starting with the reference dna and updating it with the savedDna
					GetReferenceAvatar(delegate(Dna referenceAvatarDna)
					{
						Dna avatarDna = new Dna(mReferenceAvatarDna);
						avatarDna.UpdateDna(savedDna);
						// TODO?  If UpdateDna overwrites something or fills in missing data we should save the dna back to the db

						// Get xml from updated avatar dna
						XmlDocument dnaXml = mServerAssetRepository.GetXmlDna(avatarDna.GetItemIdList());

						DistributedObjectId distributedObjectId = mDistributedObjectIdManager.GetNewId();

						ServerAccount serverAccount = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(sessionId);
						ServerDistributedAvatar serverDistributedAvatar = new ServerDistributedAvatar(distributedObjectId, avatarId, sessionId, nickname, dnaXml, serverAccount, mServerObjectRepository, mServerAssetRepository);
						//register object with object repository
						mServerObjectRepository.AddObjectToSessionId(sessionId, serverDistributedAvatar);
						//register object with session manager
						mServerEngine.ProcessZoneChange(serverDistributedAvatar, zoneId);
						gotAvatarServiceResponse(true);
					});
                }
				else 
				{
					gotAvatarServiceResponse(false);
				}
			}
            catch (System.Exception ex)
            {
                StateServerAssert.Assert(ex);
				gotAvatarServiceResponse(false);
            }
        }

    }
}
