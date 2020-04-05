using System;
using System.Collections.Generic;
using System.Text;

using Hangout.Shared;

namespace Hangout.Client
{
	class FashionGameTutorialState : TutorialState
	{
		public FashionGameTutorialState(GreenScreenRoomTutorialGui tutorialGui, Action<TutorialState> tutorialCompletedCallback)
			: base(tutorialGui, tutorialCompletedCallback)
		{
		}
		
		public override void EnterState()
		{
			GreenScreenRoomTutorialGui.OpenFashionGamePopup();
		}

		public override void TutorialComplete()
		{
			this.UserAccountProxy.SetAccountProperty<bool>(UserAccountProperties.HasPlayedFashionMiniGame, true);
			base.TutorialComplete();
		}
	}
}