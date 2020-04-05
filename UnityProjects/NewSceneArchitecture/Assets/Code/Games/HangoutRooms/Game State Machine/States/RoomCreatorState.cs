/**  --------------------------------------------------------  *
 *   RoomCreatorState.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/17/2009
 *	 
 *   --------------------------------------------------------  *
 */

using Hangout.Client.Gui;

namespace Hangout.Client
{
	public class RoomCreatorState : State
	{
		private NewRoomDialog mNewRoomDialog;
		public override string Name
		{
			get { return this.GetType().Name; }
		}

		public override void EnterState()
		{
			mNewRoomDialog = new NewRoomDialog(GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>());
			mNewRoomDialog.Showing = true;
		}

		public override void ExitState()
		{
			mNewRoomDialog.Showing = false;
			mNewRoomDialog = null;
		}

	}
	
}

