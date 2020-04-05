/**  --------------------------------------------------------  *
 *   FashionGameStation.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/22/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;

using Hangout.Client.Gui;
using Hangout.Shared;

using UnityEngine;

namespace Hangout.Client.FashionGame
{
	public abstract class FashionGameStation : GuiController, IEntity
	{
		private const string PROGRESS_INDICATOR_GUI = "resources://GUI/Minigames/Fashion/FashionStation.gui";

		private GameObject mUnityGameObject;
		protected readonly IScheduler mScheduler;

		protected IDictionary<Renderer, Color> mOriginalColors = new Dictionary<Renderer, Color>();
		private ITask mMouseOverTask = null;

		private readonly Transform mStationWorkerTransform;

		private StationWorker mWorker = null;
		public StationWorker Worker
		{
			get { return mWorker; }
		}

		private AnimationClip mWorkingAnimation = null;
		private AnimationClip mIdleAnimation = null;

		private Window mIdleWindow;
		private Window mInProgressWindow;
		private FollowWorldSpaceObject mGuiPosition;
		private ProgressIndicator mProgressIndicator = null;

		private readonly Texture2D mImage;
		public Texture2D Image
		{
			get { return mImage; }
		}
		
		private Vector3 mLocation;
		public Vector3 Location
		{
			get { return mLocation; }
		}

		private readonly string mName;
		public string Name
		{
			get { return mName; }
		}

		private readonly float mWaitTime; // How long this station takes to do it's thing
		public float WaitTime
		{
			get { return mWaitTime; }
		}

		public abstract bool RequiresWorker{ get; }

		private readonly List<AudioClip> mActivationSounds = new List<AudioClip>();
		public void AddActivationSound(AudioClip clip)
		{
			mActivationSounds.Add(clip);
		}
		
		public FashionGameStation(Pair<Vector3> location, string name, Texture2D image, float time, Vector3 guiOffset, GameObject displayObject)
			: base(GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>(), PROGRESS_INDICATOR_GUI)
		{
			if( time < 0.0f )
			{
				throw new ArgumentOutOfRangeException("time");
			}

			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			mName = name;

			if (location == null)
			{
				throw new ArgumentNullException("location");
			}
			mLocation = location.First;

			mImage = image;

			mScheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;

			mUnityGameObject = displayObject;
			foreach(Component component in mUnityGameObject.GetComponentsInChildren(typeof(Collider)))
			{
				component.gameObject.layer = FashionMinigame.STATION_LAYER;
			}
			mUnityGameObject.transform.forward = location.Second;

			foreach( Component component in mUnityGameObject.GetComponentsInChildren(typeof(Renderer)) )
			{
				Renderer renderer = (Renderer)component;
				mOriginalColors.Add(renderer, renderer.material.color);
			}

			mWaitTime = time;

			mGuiPosition = new FollowWorldSpaceObject(GameFacade.Instance.RetrieveMediator<FashionCameraMediator>().Camera, mUnityGameObject.transform, GuiAnchor.CenterCenter, Vector2.zero, guiOffset);

			foreach(ITopLevel topLevel in this.AllGuis)
			{
				GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>().SetTopLevelPosition(topLevel, mGuiPosition);
				Image stationIcon = topLevel.SelectSingleElement<Image>("**/StationIcon");
				stationIcon.Texture = mImage;
				switch (topLevel.Name)
				{
					case "IdleStationWindow":
						mIdleWindow = (Window)topLevel;
						mIdleWindow.Showing = true;
						break;

					case "InProgressWindow":
						mInProgressWindow = (Window)topLevel;
						mProgressIndicator = mInProgressWindow.SelectSingleElement<ProgressIndicator>("**/Progress");
						mInProgressWindow.Showing = false;
						break;
				}
			}

			GameObject friendLocation = GameObjectUtility.GetNamedChildRecursive("Friend Location", displayObject);
			if (friendLocation != null)
			{
				mStationWorkerTransform = friendLocation.transform;
			}
			
			HideProgressGui();
		}

		public void SetWorkingAnimation(AnimationClip workingAnimation)
		{
			if (workingAnimation == null)
			{
				throw new ArgumentNullException("workingAnimation");
			}
			mWorkingAnimation = workingAnimation;
		}

		public void SetIdleAnimation(AnimationClip idleAnimation)
		{
			if (idleAnimation == null)
			{
				throw new ArgumentNullException("idleAnimation");
			}
			mIdleAnimation = idleAnimation;
		}

		public void AssignWorker(StationWorker worker)
		{
			if (mWorkingAnimation == null)
			{
				throw new Exception("Attempting to use station (" + this.Name + ") without it's Working Animation loaded.");
			}
			if (mIdleAnimation == null)
			{
				throw new Exception("Attempting to use station (" + this.Name + ") without it's Idle Animation loaded.");
			}

			mWorker = worker;
			mWorker.UnityGameObject.transform.position = mStationWorkerTransform.position;
			mWorker.UnityGameObject.transform.rotation = mStationWorkerTransform.rotation;
			mWorker.PutAtStation(this);
			mWorker.SetWorkingAnimation(mWorkingAnimation);
			mWorker.SetIdleAnimation(mIdleAnimation);
		}

		public void PlayActivationSound()
		{
			if( mActivationSounds.Count > 0 )
			{
				AudioSource.PlayClipAtPoint(mActivationSounds[UnityEngine.Random.Range(0, mActivationSounds.Count)], mLocation);
			}
		}

		public void SetPositionDirection(Pair<Vector3> location)
		{
			mLocation = location.First;
			mUnityGameObject.transform.position = location.First;
			mUnityGameObject.transform.forward = location.Second;

			if( mWorker != null )
			{
				mWorker.UnityGameObject.transform.position = mStationWorkerTransform.position;
				mWorker.UnityGameObject.transform.rotation = mStationWorkerTransform.rotation;
			}
		}
		
		/// <summary>
		/// Sets the box to mouse-hover for 1 frame
		/// </summary>
		public void MouseIsOver()
		{
			if( mMouseOverTask != null )
			{
				mMouseOverTask.Exit();
			}
			mMouseOverTask = mScheduler.StartCoroutine(MouseOverForAFrame());
		}

		private IEnumerator<IYieldInstruction> MouseOverForAFrame()
		{
			foreach (KeyValuePair<Renderer, Color> kvp in mOriginalColors)
			{
				//TODO: Hard coded values
				kvp.Key.material.color = Color.Lerp(kvp.Value, Color.green, 0.25f);
			}

			yield return new YieldUntilNextFrame();


			foreach (KeyValuePair<Renderer, Color> kvp in mOriginalColors)
			{
				kvp.Key.material.color = kvp.Value;
			}
		}

		public GameObject UnityGameObject
		{
			get { return mUnityGameObject; }
		}

		public Vector3 FacingDirection
		{
			get { return mUnityGameObject.transform.forward; }
		}

		public void ShowProgressGui()
		{
			mIdleWindow.Showing = false;
			mInProgressWindow.Showing = true;
		}

		public void SetProgress(float progress)
		{
			if( mProgressIndicator == null )
			{
				mProgressIndicator = this.MainGui.SelectSingleElement<ProgressIndicator>("**/Progress");
			}

			mProgressIndicator.SetProgress(progress);
		}

		public void HideProgressGui()
		{
			mIdleWindow.Showing = true;
			mInProgressWindow.Showing = false;
		}

		public override void Dispose()
		{
			base.Dispose();
			mGuiPosition.Dispose();
			GameObject.Destroy(mUnityGameObject);
			mUnityGameObject = null;

			if( mWorker != null )
			{
				mWorker.EndOfLevel();
			}
		}
	}
}
