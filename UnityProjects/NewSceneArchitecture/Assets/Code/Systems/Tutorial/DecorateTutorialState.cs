/* Pherg 10/23/09 */

using System;
using System.Collections.Generic;
using System.Text;

using Hangout.Shared;

namespace Hangout.Client
{
	public class DecorateTutorialState : TutorialState
	{
		public DecorateTutorialState(GreenScreenRoomTutorialGui tutorialGui, Action<TutorialState> tutorialCompletedCallback)
			: base(tutorialGui, tutorialCompletedCallback)
		{
		}

		public override void EnterState()
		{
			GreenScreenRoomTutorialGui.OpenDecoratePopup();
		}

		public override void TutorialComplete()
		{
			this.UserAccountProxy.SetAccountProperty<bool>(UserAccountProperties.HasCompletedDecorateTutorial, true);
			base.TutorialComplete();
		}
	}
}
