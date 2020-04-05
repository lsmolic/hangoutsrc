/*
 * A Pherg Joint.
 * 10/02/09
 */
using Hangout.Shared;

namespace Hangout.Client
{
	public class GreenScreenRoomGameState : State
	{
		public override void EnterState()
		{
            Console.WriteLine("GreenScreenRoomGameState.EnterState");
			GameFacade.Instance.RegisterMediator(new GreenScreenRoomLoadingMediator(LoadingFinished));
		}

		public override void ExitState()
		{
            Console.WriteLine("GreenScreenRoomGameState.ExitState");

			GameFacade.Instance.RemoveMediator(typeof(GreenScreenRoomStateMachine).Name);
			
			RoomManagerProxy roomManagerProxy = GameFacade.Instance.RetrieveProxy<RoomManagerProxy>();
			if (roomManagerProxy != null && roomManagerProxy.CurrentRoom != null)
			{
				RoomId currentRoomId = roomManagerProxy.CurrentRoom.RoomId;
				RoomAPICommands.LeaveRoom(currentRoomId);
			}

			// Unregister Commands
			GameFacade.Instance.RemoveCommand(GameFacade.SEND_CHAT);
			GameFacade.Instance.RemoveCommand(GameFacade.RECV_CHAT);
			GameFacade.Instance.RemoveCommand(GameFacade.SEND_EMOTICON);
			GameFacade.Instance.RemoveCommand(GameFacade.RECV_EMOTICON);
		}
		
		private void LoadingFinished()
		{
			// Loading complete.  No longer need this in the system.
			GameFacade.Instance.RemoveMediator(typeof(GreenScreenRoomLoadingMediator).Name);

			// TODO: Write the logic to figure out which state we're transitioning into.  Right now
			// it's set to default, not shopping.
			// Build the green screen room state machine to kick off the Green Screen Room.
			GameFacade.Instance.RegisterMediator(new GreenScreenRoomStateMachine());

			// Register Commands
			GameFacade.Instance.RegisterCommand(GameFacade.RECV_CHAT, typeof(ShowChatCommand));
			GameFacade.Instance.RegisterCommand(GameFacade.RECV_EMOTICON, typeof(ShowEmoticonCommand));
			GameFacade.Instance.RegisterCommand(GameFacade.SEND_CHAT, typeof(SendChatCommand));
			GameFacade.Instance.RegisterCommand(GameFacade.SEND_EMOTICON, typeof(SendEmoticonCommand));
		}
	}
}
