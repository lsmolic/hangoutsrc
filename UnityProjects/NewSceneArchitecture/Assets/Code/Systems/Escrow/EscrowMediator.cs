using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using Hangout.Shared;
using Hangout.Shared.Messages;

namespace Hangout.Client
{
	public class EscrowMediator : Mediator
	{
		private Dictionary<MessageSubType, Action<Message>> mMessageActions = new Dictionary<MessageSubType, Action<Message>>();

		public EscrowMediator()
		{
			mMessageActions.Add(MessageSubType.ProcessEscrowTransaction, ProcessEscrowTransaction);
		}

		public override IList<string> ListNotificationInterests()
		{
			List<string> interestList = new List<string>();

			interestList.Add(GameFacade.REQUEST_ESCROW);

			return interestList;
		}

		public override void HandleNotification(INotification notification)
		{
			EscrowType escrowType;
			try
			{
				escrowType = (EscrowType)notification.Body;
			}
			catch (System.Exception ex)
			{
				throw new Exception("EscrowType was expected.", ex);
			}
			switch (notification.Name)
			{
				case GameFacade.REQUEST_ESCROW:
					EscrowCommands.RequestEscrow(escrowType);
					break;
			}
		}

		public void ReceiveMessage(Message receivedMessage)
		{
			MessageUtil.ReceiveMessage<MessageSubType>(receivedMessage, mMessageActions);
		}

		private void ProcessEscrowTransaction(Message receivedMessage)
		{
			EscrowType escrowType = CheckType.TryAssignType<EscrowType>(receivedMessage.Data[0]);

			switch(escrowType)
			{
				case EscrowType.FashionCityJobCoins:

					int escrowTransactionCoin = CheckType.TryAssignType<int>(receivedMessage.Data[1]);
					// Popup notification of getting PAAAAAAAAAID
					List<object> args = new List<object>();
					args.Add(Translation.ESCROW_RECEIVED_ITEM);
					args.Add(String.Format(Translation.ESCROW_FASHION_CITY_JOB_COINS_RECEIVED, escrowTransactionCoin));
					Hangout.Shared.Action okcb = delegate() { };
					args.Add(okcb);

					GameFacade.Instance.SendNotification(GameFacade.SHOW_DIALOG, args);

					break;
			}
		}
	}
}
