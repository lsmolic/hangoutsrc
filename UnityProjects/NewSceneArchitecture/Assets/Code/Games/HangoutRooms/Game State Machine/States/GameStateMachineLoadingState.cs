using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Client
{
	public class GameStateMachineLoadingState : State
	{
		private bool mClientAssetRepositoryDoneLoading = false;
		private bool mSoundProxyDoneLoading = false;
		
		private readonly Hangout.Shared.Action mFinishedLoadingCallback;
		
		public GameStateMachineLoadingState(Hangout.Shared.Action finishedLoadingCallback)
		{
			mFinishedLoadingCallback = finishedLoadingCallback;
		}
		
		public void ClientAssetRepositoryDoneLoading()
		{
			mClientAssetRepositoryDoneLoading = true;
			CheckIfAllDoneLoading();
		}
		
		public void SoundProxyDoneLoading()
		{
			mSoundProxyDoneLoading = true;
			CheckIfAllDoneLoading();
		}
		
		// Check each bool.  If all true then call finished loading callback.
		private void CheckIfAllDoneLoading()
		{
			if (mClientAssetRepositoryDoneLoading && mSoundProxyDoneLoading)
			{
				mFinishedLoadingCallback();
			}
		}

		public override void EnterState()
		{
		}

		public override void ExitState()
		{
		}
	}
}
