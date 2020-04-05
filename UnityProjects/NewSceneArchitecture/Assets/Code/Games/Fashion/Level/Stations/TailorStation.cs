/**  --------------------------------------------------------  *
 *   TailorStation.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/22/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System.Collections.Generic;

using Hangout.Shared;

using UnityEngine;

namespace Hangout.Client.FashionGame
{
	public class TailorStation : FashionGameStation
	{
		private ITask mWaitForClothingTask = null;

		public TailorStation(Pair<Vector3> location, string label, Texture2D image, float time, Vector3 guiOffset, GameObject displayObject)
			: base(location, label, image, time, guiOffset, displayObject)
		{
		}

		public override bool RequiresWorker
		{
			get { return true; }
		}

		private IEnumerator<IYieldInstruction> WaitForClothing(ClothingItem item)
		{
			mProcessingClothing = item;
			yield return new YieldUntilNextFrame();
			item.Fix();
			item.Show();

			// TODO: Hard coded values
			Camera camera = GameFacade.Instance.RetrieveMediator<FashionCameraMediator>().Camera;
			Vector3 position = this.UnityGameObject.transform.position + (this.UnityGameObject.transform.up * 1.25f);
			item.SetBillboardPosition(position + ((camera.transform.position - position).normalized * 1.25f));
			item.SetBillboardSize(0.1f);

			this.ShowProgressGui();

			// Animate the progress indicator
			float inverseTime = 1.0f / this.WaitTime;
			for (float t = 0; t < this.WaitTime; t += Time.deltaTime)
			{
				yield return new YieldUntilNextFrame();
				this.SetProgress(t * inverseTime);
			}

			// TODO: Hard coded value
			item.SetBillboardSize(0.3f);

			this.HideProgressGui();

			GameFacade.Instance.SendNotification
			(
				FashionMinigame.EARNED_EXPERIENCE_NOTIFICATION, 
				new ExperienceInfo
				(
					ExperienceType.ClothingSewn,
					item.BillboardPosition
				)
			);

			mClothingReady = true;
		}

		private ClothingItem mProcessingClothing = null;
		
		public ClothingItem CurrentClothing
		{
			get { return mProcessingClothing; }
		}

		private bool mClothingReady = false;

		public bool HasReadyClothing
		{
			get { return mClothingReady; }
		}

		public ClothingItem RetrieveFixedClothing()
		{
			ClothingItem result = null;
			if (mClothingReady)
			{
				result = mProcessingClothing;
				mProcessingClothing = null;
				mClothingReady = false;
				result.SetBillboardSize(result.OriginalBilboardSize);
			}
			return result;
		}

		public void Clear()
		{
			if( mWaitForClothingTask != null )
			{
				mWaitForClothingTask.Exit();
				mWaitForClothingTask = null;
			}

			if( mProcessingClothing != null )
			{
				mProcessingClothing.Hide();
				mProcessingClothing = null;
			}

			mClothingReady = false;

			this.HideProgressGui();
		}

		public void AddClothing(ClothingItem item)
		{
			if (mProcessingClothing == null && (mWaitForClothingTask == null || !mWaitForClothingTask.IsRunning))
			{
				this.PlayActivationSound();

				if( Worker != null )
				{
					Worker.PlayWorkingAnimation();
				}

				mWaitForClothingTask = mScheduler.StartCoroutine(WaitForClothing(item));
			}
			else
			{
				// If it didn't go to this station, put it back
				GameFacade.Instance.RetrieveMediator<FashionGameGui>().PutItemInGui(item);
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			Clear();
		}
	}
}
