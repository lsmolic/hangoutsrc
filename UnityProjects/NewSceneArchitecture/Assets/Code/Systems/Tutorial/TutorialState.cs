/* Phergalicious Def 10/21/09 */

using System;
using System.Collections.Generic;
using System.Text;

using Hangout.Shared;

namespace Hangout.Client
{
	public abstract class TutorialState : State
	{
		private GreenScreenRoomTutorialGui mTutorialGui;
		protected GreenScreenRoomTutorialGui GreenScreenRoomTutorialGui
		{
			get { return mTutorialGui; }
		}
		
		private UserAccountProxy mUserAccountProxy;
		protected UserAccountProxy UserAccountProxy
		{
			get { return mUserAccountProxy; }
		}

		private Action<TutorialState> mTutorialStateFinishedCallback;
		
		public TutorialState(GreenScreenRoomTutorialGui tutorialGui, Action<TutorialState> tutorialStateFinishedCallback)
		{
			if (tutorialGui == null)
			{
				throw new ArgumentNullException("tutorialGui");
			}
			mTutorialGui = tutorialGui;
			
			mTutorialStateFinishedCallback = tutorialStateFinishedCallback;

			mUserAccountProxy = GameFacade.Instance.RetrieveProxy<UserAccountProxy>();
		}
		
		//TODO: Put in the code to switch the user account proxy to reflect that we've finished the toot.
		public virtual void TutorialComplete()
		{
			mTutorialStateFinishedCallback(this);
		}

		public override void ExitState()
		{
			GameFacade.Instance.SendNotification(GameFacade.PLAY_SOUND_POPUP_APPEAR_B);
			mTutorialGui.HidePopup();
		}
	}
}
