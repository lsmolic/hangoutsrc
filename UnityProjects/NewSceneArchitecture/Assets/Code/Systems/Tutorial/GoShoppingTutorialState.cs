/* Pherg 10/21/09 */

using System;
using System.Collections.Generic;
using System.Text;

using Hangout.Shared;

namespace Hangout.Client
{
	public class GoShoppingTutorialState : TutorialState
	{
		public GoShoppingTutorialState(GreenScreenRoomTutorialGui tutorialGui, Action<TutorialState> tutorialCompletedCallback)
			: base(tutorialGui, tutorialCompletedCallback)
		{
		}
		
		public override void EnterState()
		{
			GreenScreenRoomTutorialGui.OpenShoppingPopup();
		}

		public override void TutorialComplete()
		{
			this.UserAccountProxy.SetAccountProperty<bool>(UserAccountProperties.HasCompletedShoppingTutorial, true);
			base.TutorialComplete();
		}
	}
}
