using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using Hangout.Shared.Messages;
using System.Xml;
using Hangout.Server.StateServer;
using System.Collections.Specialized;

namespace Hangout.Server
{
	public class EscrowManager : AbstractExtension
	{
		// Most coins that can be awarded at once
		// Would be better to do this per day or per week...
		private static int MAX_COINS = 10000;

		public EscrowManager(ServerStateMachine serverStateMachine) : base(serverStateMachine)
		{
			mMessageActions.Add(MessageSubType.ProcessEscrowTransaction, ProcessEscrowTransaction);
		}

		private static void VerifyGiveCoinSuccess(XmlDocument responseXml)
		{
			XmlNode successNode = responseXml.SelectSingleNode("success");
			if ((successNode == null) || (successNode.InnerXml != "true"))
			{
				StateServerAssert.Assert(new Exception("Create Coin Error:\n" + responseXml.OuterXml));
			}
		}

		/// <summary>
		/// Use the payment system to award this account with this number of coins.
		/// </summary>
		/// <param name="serverStateMachine"></param>
		/// <param name="serverAccount"></param>
		/// <param name="totalCoins"></param>
		private void ProcessCoinPayment(ServerAccount serverAccount, int totalCoins, EscrowType escrowType, Guid sessionId)
		{
			NameValueCollection args = new NameValueCollection();
			args.Add("userId", serverAccount.PaymentItemUserId);
			args.Add("amount", totalCoins.ToString());
			args.Add("ipAddress", serverAccount.IpAddress);

			PaymentItemsProcessClientMessage clientMessage = new PaymentItemsProcessClientMessage();
			PaymentCommand cmd = clientMessage.AddVirtualCoinForUser(args);
			mServerStateMachine.PaymentItemsManager.ProcessPaymentCommand(cmd, delegate(string response)
			{
				XmlDocument xmlResponse = new XmlDocument();
				xmlResponse.LoadXml(response);
				//send message to client
				Message processCoinPaymentMessage = new Message();
				List<object> processCoinPaymentMessageData = new List<object>();
				processCoinPaymentMessageData.Add(escrowType);
				processCoinPaymentMessageData.Add(totalCoins);
				processCoinPaymentMessage.EscrowMessage(processCoinPaymentMessageData);
				processCoinPaymentMessage.Callback = (int) MessageSubType.ProcessEscrowTransaction;

				SendMessageToClient(processCoinPaymentMessage, sessionId);
			});
		}

		/// <summary>
		/// After heading back from the db, now process the xml to add up coins,
		/// then clear the escrow db for this account.
		/// </summary>
		/// <param name="serverStateMachine"></param>
		/// <param name="serverAccount"></param>
		/// <param name="responseXml"></param>
		private void AwardMinigameCoins(ServerAccount serverAccount, XmlDocument responseXml, EscrowType escrowType, Guid sessionId)
		{
			int totalCoins = EscrowXmlUtil.GetTotalCoins(responseXml);
			// Clamp to max coins
			totalCoins = Math.Min(MAX_COINS, totalCoins);
			if (totalCoins <= 0)
			{
				// No coins to award, we are done.
				return;
			}
			// Ok, now process the coins
			ProcessCoinPayment(serverAccount, totalCoins, escrowType, sessionId);
		}

		private void ProcessEscrowTransaction(Message receivedMessage, Guid sessionId)
		{
			EscrowType escrowType = CheckType.TryAssignType<EscrowType>(receivedMessage.Data[0]);

			ServerAccount serverAccount = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(sessionId);
			EscrowManagerServiceAPI.GetEscrowTransactions(serverAccount.FacebookAccountId, 0, escrowType, "", true,
			delegate(XmlDocument responseXml)
			{
				switch (escrowType)
				{
					case EscrowType.FashionCityJobCoins:
						AwardMinigameCoins(serverAccount, responseXml, EscrowType.FashionCityJobCoins, sessionId);
						break;
					default:
						//TODO: log here about invalid callbacks
						break;
				}
			});
		}

		/// <summary>
		/// Give coins to friends who were hired in this show.
		/// These coins are stored in the db in the escrow table until the friend logs in next time.
		/// </summary>
		/// <param name="serverAccount"></param>
		/// <param name="coins"></param>
		public static void GiveFashionMinigameHiredFriendsCoins(ServerAccount serverAccount, int coins)
		{
			if (coins <= 0)
			{
				// No coins to give, done.
				return;
			}

			FashionMinigameServiceAPI.GetAllHiredFriends(serverAccount, delegate(ICollection<long> friendList)
			{
				EscrowManagerServiceAPI.CreateEscrowTransaction(friendList, serverAccount.FacebookAccountId, EscrowType.FashionCityJobCoins.ToString(), coins, VerifyGiveCoinSuccess);
			});
		}

	}
}
