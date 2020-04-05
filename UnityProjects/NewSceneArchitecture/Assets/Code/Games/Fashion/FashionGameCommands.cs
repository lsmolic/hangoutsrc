/**  --------------------------------------------------------  *
 *   FashionGameCommands.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/02/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Text;
using System.Collections.Generic;

using Hangout.Shared;
using Hangout.Shared.FashionGame;

namespace Hangout.Client.FashionGame
{
	public static class FashionGameCommands
	{
		private static ulong mNextId = 0;
		private readonly static Dictionary<ulong, Action<Message>> mCallbacks = new Dictionary<ulong, Action<Message>>();

		/// <summary>
		/// Called by ClientMessageProcessor
		/// </summary>
		public static void ProcessMessage(Message incMsg)
		{
			if (incMsg.Data.Count == 0)
			{
				throw new Exception("Message has no data");
			}

			ulong callbackId = ulong.Parse(incMsg.Data[0].ToString());
			incMsg.Data.RemoveAt(0);
			mCallbacks[callbackId](incMsg);
			mCallbacks.Remove(callbackId);
		}

		private static ulong RegisterForResponse(string callbackName, Action<Message> response)
		{
			ulong thisId = mNextId++;

			string responseName = String.Format("{0}({1})", callbackName, thisId);
			Console.Log("Waiting: " + responseName);
			EventLogger.LogNoMixPanel("FashionGameCommunication", "Waiting", "Event", responseName);

			response += delegate(Message msg)
			{
				Console.Log("Received: " + responseName);
				EventLogger.LogNoMixPanel("FashionGameCommunication", "Received", "Event", responseName);
			};

			mCallbacks.Add(thisId, response);
			return thisId;
		}

		public static void GetLoadingInfo(Action<Message> onMessageReceived)
		{
			ulong thisId = RegisterForResponse("GetLoadingInfo", onMessageReceived);

			ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
			Message requestDefaultClothesMessage = new Message();
			List<object> messageData = new List<object>();
			messageData.Add(thisId);
			requestDefaultClothesMessage.FashionGameLoadingInfoMessage(messageData);
			clientMessageProcessor.SendMessageToReflector(requestDefaultClothesMessage);
		}

		public static void GetAvatars(Jobs job, Action<Message> onMessageReceived)
		{
			ulong thisId = RegisterForResponse("GetAvatars", onMessageReceived);

			ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
			Message requestModelMessage = new Message();
			List<object> messageData = new List<object>();
			messageData.Add(thisId);
			messageData.Add(job.ToString());
			requestModelMessage.FashionGameModelAssetMessage(messageData);
			clientMessageProcessor.SendMessageToReflector(requestModelMessage);
		}

		public static void GetPlayerData(string dataKey, Action<Message> onMessageReceived)
		{
			ulong thisId = RegisterForResponse("GetPlayerData", onMessageReceived);

			ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
			Message requestModelMessage = new Message();
			List<object> messageData = new List<object>();
			messageData.Add(thisId);
			messageData.Add(dataKey);
			requestModelMessage.FashionGameGetData(messageData);
			clientMessageProcessor.SendMessageToReflector(requestModelMessage);
		}

		public static void SetLevelComplete(uint totalXP, uint XPThisLevel, bool leveledUp, Action<Message> onMessageReceived)
		{
			ulong thisId = RegisterForResponse("SetLevelComplete", onMessageReceived);

			ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
			Message message = new Message();
			List<object> messageData = new List<object>();
			messageData.Add(thisId);
			messageData.Add(totalXP);
			messageData.Add(XPThisLevel);
			messageData.Add(leveledUp);
			message.FashionGameLevelComplete(messageData);
			clientMessageProcessor.SendMessageToReflector(message);
		}

		public static void SetServerData(string dataKey, string dataValue)
		{
			ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
			Message requestModelMessage = new Message();
			List<object> messageData = new List<object>();
			messageData.Add(dataKey);
			messageData.Add(dataValue);
			requestModelMessage.FashionGameSetData(messageData);
			clientMessageProcessor.SendMessageToReflector(requestModelMessage);
		}

		public static void GetFriendsToHire(Jobs job, Action<Message> onMessageReceived)
		{
			ulong thisId = RegisterForResponse("GetFriendsToHire", onMessageReceived);

			ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
			Message requestModelMessage = new Message();
			List<object> messageData = new List<object>();
			messageData.Add(thisId);
			messageData.Add(job.ToString());
			requestModelMessage.FashionGameFriendsToHire(messageData);
			clientMessageProcessor.SendMessageToReflector(requestModelMessage);
		}

		public static void HireFriend(long facebookId, Jobs job, Action<Message> onMessageReceived)
		{
			ulong thisId = RegisterForResponse("HireFriend", onMessageReceived);

			ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
			Message requestModelMessage = new Message();
			List<object> messageData = new List<object>();
			messageData.Add(thisId);
			messageData.Add(facebookId.ToString());
			messageData.Add(job.ToString());

			requestModelMessage.FashionGameHireFriend(messageData);
			clientMessageProcessor.SendMessageToReflector(requestModelMessage);

			EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.FRIEND_HIRED, job.ToString(), facebookId.ToString());
		}

		public static void GetAllHiredAvatars(Action<Message> onMessageReceived)
		{
			ulong thisId = RegisterForResponse("GetAllHiredAvatars", onMessageReceived);

			ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
			
			List<object> messageData = new List<object>();
			messageData.Add(thisId);

			clientMessageProcessor.SendMessageToReflector
			(
				new Message
				(
					MessageType.FashionMinigame, 
					(int)MessageSubType.GetAllHiredAvatars, 
					messageData
				)
			);
		}

		public static void TryUseEnergy(float energy, Action<Message> onMessageReceived)
		{
			ulong thisId = RegisterForResponse("TryUseEnergy", onMessageReceived);

			ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();

			List<object> messageData = new List<object>();
			messageData.Add(thisId);
			messageData.Add(energy.ToString());

			clientMessageProcessor.SendMessageToReflector
			(
				new Message
				(
					MessageType.FashionMinigame,
					(int)MessageSubType.UseEnergy,
					messageData
				)
			);
		}
	}
}
