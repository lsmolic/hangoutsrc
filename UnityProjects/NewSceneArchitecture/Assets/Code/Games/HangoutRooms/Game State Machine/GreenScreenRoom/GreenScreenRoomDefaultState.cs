/* So Phergulous.  10/13/09 */
using Hangout.Shared;

namespace Hangout.Client
{
	public class GreenScreenRoomDefaultState : State
	{
		public override void EnterState()
		{
			GameFacade.Instance.SendNotification(GameFacade.REQUEST_ESCROW, EscrowType.FashionCityJobCoins);

			GameFacade.Instance.RetrieveMediator<GreenScreenRoomCameraStateMachine>().StartDefaultInRoomCamera();
			GameFacade.Instance.RetrieveMediator<GreenScreenRoomInputStateMachine>().MovementEnabled();

			GameFacade.Instance.RetrieveProxy<RoomManagerProxy>().RoomComplete();

            GameFacade.Instance.RetrieveMediator<ToolbarMediator>().Init();
			
			GameFacade.Instance.SendNotification(GameFacade.ENTERED_GREEN_SCREEN_ROOM_DEFAULT_STATE);
		}
		
		public override void ExitState()
		{
        }
	}
}
