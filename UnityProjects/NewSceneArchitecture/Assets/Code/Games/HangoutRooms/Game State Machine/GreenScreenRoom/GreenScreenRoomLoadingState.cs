/* Pherg 10/26/09 */

using Hangout.Client.Gui;

namespace Hangout.Client
{
	public class GreenScreenRoomLoadingState : State
	{
		// GUI
		private const string mLoadingScreenGuiPath = "resources://GUI/Minigames/Fashion/LoadingScreen.gui";
		private GuiController mLoadingGui;
		private IGuiManager mGuiManager;

		public override void EnterState()
		{
			Console.WriteLine("GreenScreenRoomLoadingState.EnterState");
			//GameFacade.Instance.RetrieveMediator<GreenScreenRoomCameraStateMachine>().EnterLoadingState();
			mGuiManager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
			mLoadingGui = new GuiController(mGuiManager, mLoadingScreenGuiPath);
            Window mainWindow = (Window)mLoadingGui.MainGui;
            mainWindow.InFront = true;
		}

		public override void ExitState()
		{
			mLoadingGui.MainGui.Close();
			Console.WriteLine("GreenScreenRoomLoadingState.ExitState");
		}
	}
}
