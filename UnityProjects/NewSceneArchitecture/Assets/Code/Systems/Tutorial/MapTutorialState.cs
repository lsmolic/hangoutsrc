/* Pherg 10/23/09 */

using System;
using System.Collections.Generic;
using System.Text;

using Hangout.Shared;

namespace Hangout.Client
{
	public class MapTutorialState : TutorialState
	{
		public MapTutorialState(GreenScreenRoomTutorialGui tutorialGui, Action<TutorialState> tutorialCompletedCallback)
			: base(tutorialGui, tutorialCompletedCallback)
		{
		}

		public override void EnterState()
		{
			GreenScreenRoomTutorialGui.OpenMapPopup();
		}
		
		public override void TutorialComplete()
		{
			base.TutorialComplete();
			
			this.UserAccountProxy.SetAccountProperty<bool>(UserAccountProperties.HasCompletedOpenMapTutorial, true);
			
			GreenScreenRoomTutorialGui.HidePopup();
		}
	}
}
