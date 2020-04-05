/* Pherg 10/23/09 */

using System;
using System.Collections.Generic;
using System.Text;

using Hangout.Shared;

namespace Hangout.Client
{
	public class GetCashTutorialState : TutorialState
	{
		public GetCashTutorialState(GreenScreenRoomTutorialGui tutorialGui, Action<TutorialState> tutorialCompletedCallback)
			: base(tutorialGui, tutorialCompletedCallback)
		{
		}

		public override void EnterState()
		{
			GreenScreenRoomTutorialGui.OpenGetCashPopup();
		}

		public override void TutorialComplete()
		{
			this.UserAccountProxy.SetAccountProperty<bool>(UserAccountProperties.HasCompletedGetCashTutorial, true);
			base.TutorialComplete();
		}
	}
}
