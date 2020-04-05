/*
 * Pherg 12/03/09
 * The runway state.  In charge of loading up the runway sequence, kicking it off and cleaning it up.
 */

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using Hangout.Client.FashionGame;
using Hangout.Shared;
using Hangout.Client.Gui;

namespace Hangout.Client
{
	public class RunwaySequenceState : State
	{
		private const string PATH_TO_MODELS_XML = "assets://FashionMinigame/RunwaySequence.xml";
		private const string PATH_TO_GUI = "resources://GUI/Minigames/Fashion/RunwaySequenceGui.gui";
		
		private GuiController mRunwaySequenceGuiController;
		
		private string mPathToRunwayWalkAnimation;
		private string mPathToRunwayEnvironment;

		private string mBackgroundUrl;
		public string BackgroundUrl
		{
			get { return mBackgroundUrl; }
			set { mBackgroundUrl = value; }
		}
		
		private string mLocationName;
		public string LocationName
		{
			get { return mLocationName; }
			set { mLocationName = value; }
		}
		
		private string mMusicPath;
		public string MusicPath
		{
			get { return mMusicPath; }
			set { mMusicPath = value; }
		}
		
		private ITask mRetrievingPathsCoroutine;
		
		private UnityEngine.Object mRunwayEnvironmentObject = null;
		private GameObject mRunwayGameObject = null;
		
		private AnimationClip mRunwayWalkClip = null;
		
		private ClientAssetRepository mClientAssetRepository;
		
		private FashionCameraMediator mCameraMediator;
		
		private Hangout.Shared.Action mOnExitCallback;

		private ITask mRunwayLoopTask;
		private ITask mSwitchViewsTask;
		private ITask mManageShowTask;
		private ITask mSmoothlyAnimateTask;

		private ICollection<FashionModel> mFashionModels;
		
		private Transform mActiveCameraTarget;		
		private Transform mLoadingPosition;
		
		private Texture2D mBackgroundTexture;
		
		private GameObject mSpotLight;
		private GameObject mMusicGameObject;
		
		private AudioClip mMusicAudioClip;
		
		private List<ITask> mActiveModelTasks = new List<ITask>();
		
		private List<KeyValuePair<View, float>> mCameraViews = null;
		private View mActiveView;
		
		private Button mExitButton;
		
		private GuiFrame mTopPerformersFrame;
		
		private bool mBackgroundLoaded = false;
		private bool mMusicLoaded = false;
		
		private SmoothDampTransform mSmoothDampTransform;
		
		private List<GameObject> mCameraTransforms = new List<GameObject>();
		
		public RunwaySequenceState(Hangout.Shared.Action onExitCallback)
		{
			mOnExitCallback = onExitCallback;
			
			mClientAssetRepository = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			mClientAssetRepository.LoadAssetFromPath<XmlAsset>(PATH_TO_MODELS_XML, delegate(XmlAsset asset)
			{
				mPathToRunwayWalkAnimation = asset.XmlDocument.SelectSingleNode("RunwaySequence/RunwayWalkPath").InnerText;
				mPathToRunwayEnvironment = asset.XmlDocument.SelectSingleNode("RunwaySequence/RunwayEnvironmentPath").InnerText;
			});
		}
		
		private void BuildCameraViews()
		{
			mCameraViews = new List<KeyValuePair<View, float>>();
			
			Transform cameraTransform = GameFacade.Instance.RetrieveMediator<FashionCameraMediator>().Camera.gameObject.transform;
			mSmoothDampTransform = new SmoothDampTransform(cameraTransform, 1.0f, 1.0f);
			
			GameObject cameraTransform1 = new GameObject("cameraTransform");
			GameObject cameraTransform2 = new GameObject("cameraTransform");
			GameObject cameraTransform3 = new GameObject("cameraTransform");
			GameObject cameraTransform4 = new GameObject("cameraTransform");
			GameObject cameraTransform5 = new GameObject("cameraTransform");
			GameObject cameraTransform6 = new GameObject("cameraTransform");
			GameObject cameraTransform7 = new GameObject("cameraTransform");
			GameObject cameraTransform8 = new GameObject("cameraTransform");
			GameObject cameraTransform9 = new GameObject("cameraTransform");
			GameObject cameraTransform10 = new GameObject("cameraTransform");
			
			mCameraTransforms.Add(cameraTransform1);
			mCameraTransforms.Add(cameraTransform2);
			mCameraTransforms.Add(cameraTransform3);
			mCameraTransforms.Add(cameraTransform4);
			mCameraTransforms.Add(cameraTransform5);
			mCameraTransforms.Add(cameraTransform6);
			mCameraTransforms.Add(cameraTransform7);
			mCameraTransforms.Add(cameraTransform8);
			mCameraTransforms.Add(cameraTransform9);
			mCameraTransforms.Add(cameraTransform10);

			LookAtView lookAtView = new LookAtView(cameraTransform1.transform, cameraTransform, new Vector3(10.7f, 4.8f, 10.5f), null);
			lookAtView.SnapToView = true;
			lookAtView.TranslateLag = 1.0f;
			lookAtView.RotateLag = 1.0f;
			lookAtView.TargetOffset = new Vector3(0.0f, 0.8f, -0.5f);
			lookAtView.offsetRelativeToTarget = false;
			lookAtView.FieldOfView = 30;
			lookAtView.StartTracking();
			mCameraViews.Add(new KeyValuePair<View, float>(lookAtView, mRunwayWalkClip.length * 0.25f * 2));

			FixedAngleFollowView fixedAngleFollowView = new FixedAngleFollowView(cameraTransform2.transform, cameraTransform, new Vector3(-4.0f, 240.0f, 0.0f), null);
			fixedAngleFollowView.SnapToView = true;
			fixedAngleFollowView.TranslateLag = 0.25f;
			fixedAngleFollowView.RotateLag = 1.0f;
			fixedAngleFollowView.TargetOffset = new Vector3(0.0f, -0.3f, 0.3f);
			fixedAngleFollowView.offsetRelativeToTarget = false;
			fixedAngleFollowView.RelativeToTarget = false;
			fixedAngleFollowView.FieldOfView = 24;
			fixedAngleFollowView.StartTracking();
			mCameraViews.Add(new KeyValuePair<View, float>(fixedAngleFollowView, mRunwayWalkClip.length * 0.22f * 2));

			lookAtView = new LookAtView(cameraTransform3.transform, cameraTransform, new Vector3(4.7f, 1.9f, 22.0f), null);
			lookAtView.SnapToView = true;
			lookAtView.TranslateLag = 1.0f;
			lookAtView.RotateLag = 0.5f;
			lookAtView.TargetOffset = new Vector3(0.0f, -0.25f, 0.0f);
			lookAtView.offsetRelativeToTarget = false;
			lookAtView.FieldOfView = 3;
			lookAtView.StartTracking();
			mCameraViews.Add(new KeyValuePair<View, float>(lookAtView, mRunwayWalkClip.length * 0.16f * 2));

			lookAtView = new LookAtView(cameraTransform4.transform, cameraTransform, new Vector3(3.1f, 2.7f, 9.6f), null);
			lookAtView.SnapToView = true;
			lookAtView.TranslateLag = 1.1f;
			lookAtView.RotateLag = 1.0f;
			lookAtView.TargetOffset = new Vector3(0.0f, -0.3f, 0.0f);
			lookAtView.offsetRelativeToTarget = false;
			lookAtView.FieldOfView = 20;
			lookAtView.StartTracking();
			mCameraViews.Add(new KeyValuePair<View, float>(lookAtView, mRunwayWalkClip.length * 0.12f * 2));

			fixedAngleFollowView = new FixedAngleFollowView(cameraTransform5.transform, cameraTransform, new Vector3(25.0f, 180.0f, 0.0f), null);
			fixedAngleFollowView.SnapToView = true;
			fixedAngleFollowView.TranslateLag = 0.3f;
			fixedAngleFollowView.RotateLag = 1.0f;
			fixedAngleFollowView.TargetOffset = new Vector3(0.0f, 0.1f, 1.0f);
			fixedAngleFollowView.offsetRelativeToTarget = false;
			fixedAngleFollowView.RelativeToTarget = false;
			fixedAngleFollowView.FieldOfView = 15;
			fixedAngleFollowView.StartTracking();
			mCameraViews.Add(new KeyValuePair<View, float>(fixedAngleFollowView, mRunwayWalkClip.length * 0.25f * 2));

			lookAtView = new LookAtView(cameraTransform6.transform, cameraTransform, new Vector3(17.2f, 5.5f, 11.6f), null);
			lookAtView.SnapToView = true;
			lookAtView.TranslateLag = 2.0f;
			lookAtView.RotateLag = 3.0f;
			lookAtView.TargetOffset = new Vector3(0.0f, 2.0f, 1.0f);
			lookAtView.offsetRelativeToTarget = false;
			lookAtView.FieldOfView = 30;
			lookAtView.StartTracking();
			mCameraViews.Add(new KeyValuePair<View, float>(lookAtView, mRunwayWalkClip.length * 0.25f * 2));

			fixedAngleFollowView = new FixedAngleFollowView(cameraTransform7.transform, cameraTransform, new Vector3(-4.0f, 200.0f, 0.0f), null);
			fixedAngleFollowView.SnapToView = true;
			fixedAngleFollowView.TranslateLag = 0.4f;
			fixedAngleFollowView.RotateLag = 1.0f;
			fixedAngleFollowView.TargetOffset = new Vector3(0.0f, -0.3f, 0.3f);
			fixedAngleFollowView.offsetRelativeToTarget = false;
			fixedAngleFollowView.RelativeToTarget = false;
			fixedAngleFollowView.FieldOfView = 30;
			fixedAngleFollowView.StartTracking();
			mCameraViews.Add(new KeyValuePair<View, float>(fixedAngleFollowView, mRunwayWalkClip.length * 0.22f * 2));

			lookAtView = new LookAtView(cameraTransform8.transform, cameraTransform, new Vector3(8.9f, 1.1f, 12.7f), null);
			lookAtView.SnapToView = true;
			lookAtView.TranslateLag = 1.0f;
			lookAtView.RotateLag = 0.5f;
			lookAtView.TargetOffset = new Vector3(0.0f, -0.3f, 0.0f);
			lookAtView.offsetRelativeToTarget = false;
			lookAtView.FieldOfView = 6;
			lookAtView.StartTracking();
			mCameraViews.Add(new KeyValuePair<View, float>(lookAtView, mRunwayWalkClip.length * 0.16f * 2));

			lookAtView = new LookAtView(cameraTransform9.transform, cameraTransform, new Vector3(3.1f, 2.7f, 9.6f), null);
			lookAtView.SnapToView = true;
			lookAtView.TranslateLag = 1.1f;
			lookAtView.RotateLag = 1.0f;
			lookAtView.TargetOffset = new Vector3(0.0f, -0.5f, 0.0f);
			lookAtView.offsetRelativeToTarget = false;
			lookAtView.FieldOfView = 20;
			lookAtView.StartTracking();
			mCameraViews.Add(new KeyValuePair<View, float>(lookAtView, mRunwayWalkClip.length * 0.12f * 2));

			fixedAngleFollowView = new FixedAngleFollowView(cameraTransform10.transform, cameraTransform, new Vector3(5.0f, 240.0f, 0.0f), null);
			fixedAngleFollowView.SnapToView = true;
			fixedAngleFollowView.TranslateLag = 0.5f;
			fixedAngleFollowView.RotateLag = 1.0f;
			fixedAngleFollowView.TargetOffset = new Vector3(0.0f, -0.5f, 0.4f);
			fixedAngleFollowView.offsetRelativeToTarget = false;
			fixedAngleFollowView.RelativeToTarget = false;
			fixedAngleFollowView.FieldOfView = 35;
			fixedAngleFollowView.StartTracking();
			mCameraViews.Add(new KeyValuePair<View, float>(fixedAngleFollowView, mRunwayWalkClip.length * 0.25f * 2));
		}
		
		private void Loading()
		{
			mClientAssetRepository.LoadAssetFromPath<ImageAsset>(mBackgroundUrl, delegate(ImageAsset asset)
			{
				mBackgroundTexture = asset.Texture2D;
				mBackgroundLoaded = true;
				CheckIfDoneLoading();
			});

			if (mMusicPath != null)
			{
				mClientAssetRepository.LoadAssetFromPath<SoundAsset>(mMusicPath, delegate(SoundAsset asset)
				{
					mMusicLoaded = true;
					mMusicAudioClip = asset.AudioClip;
					mMusicGameObject = new GameObject("music game object");
					AudioSource audioSource = (AudioSource)mMusicGameObject.AddComponent(typeof(AudioSource));
					audioSource.clip = mMusicAudioClip;
					audioSource.loop = true;
					audioSource.volume = 0.9f;
					audioSource.Play();
					CheckIfDoneLoading();
				});
			}
			else
			{
				// If there's no music path, there ain't gonna be any music
				mMusicLoaded = true;
			}

			// If these are null then we need to load them from the repo using their paths.
			if (mRunwayEnvironmentObject == null)
			{
				mClientAssetRepository.LoadAssetFromPath<UnityEngineAsset>(mPathToRunwayEnvironment, delegate(UnityEngineAsset asset)
				{
					mRunwayEnvironmentObject = asset.UnityObject;
					CheckIfDoneLoading();
				});
			}

			if (mRunwayWalkClip == null)
			{
				mClientAssetRepository.LoadAssetFromPath<UnityEngineAsset>(mPathToRunwayWalkAnimation, delegate(UnityEngineAsset asset)
				{
					UnityEngine.Object unityObject = ((UnityEngineAsset)asset).UnityObject;
					GameObject gameObject = (GameObject)GameObject.Instantiate(unityObject);
					try
					{
						Animation animation1 = (Animation)gameObject.GetComponentInChildren(typeof(Animation));
						if (animation1 == null)
						{
							throw new Exception("Error loading late bound asset file, there's no animation component on the asset (" + gameObject.name + ")");
						}

						AnimationClip result = animation1.GetClip(unityObject.name);
						if (result == null)
						{
							throw new Exception("Cannot find clip (" + unityObject.name + ") in Animation component on " + animation1.name);
						}
						mRunwayWalkClip = result;
					}
					finally
					{
						GameObject.Destroy(gameObject);
						CheckIfDoneLoading();
					}
				});
			}
		}
		
		private void CheckIfDoneLoading()
		{
			if (mRunwayEnvironmentObject != null && mRunwayWalkClip != null && mBackgroundLoaded && mMusicLoaded)
			{
				SetUpScene();
			}
		}
		
		// Will instantiate the game objects and build the camera manager.
		private void SetUpScene()
		{
			BuildGui();	
			mRunwayGameObject = (GameObject)GameObject.Instantiate(mRunwayEnvironmentObject);
			
			foreach (Transform child in mRunwayGameObject.transform)
			{
				if (child.gameObject.name == "Model Loading Position")
				{
					mLoadingPosition = child;
					break;
				}
			}

			SetBackground(mBackgroundTexture);
			if (mCameraViews == null)
			{
				BuildCameraViews();
			}
			
			mFashionModels = GameFacade.Instance.RetrieveMediator<FashionNpcMediator>().FashionModelList;
			
			mRunwayLoopTask = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(RunwayLoop());
			mSmoothlyAnimateTask = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(SmoothlyAnimate());
			mManageShowTask = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(ManageShow());
		}

		public override void EnterState()
		{
			foreach (ITopLevel fashionGui in GameFacade.Instance.RetrieveMediator<FashionGameGui>().AllGuis)
			{
				fashionGui.Showing = false;
			}
			
			if (mPathToRunwayWalkAnimation.Length == 0 ||  mPathToRunwayEnvironment.Length == 0)
			{
				mRetrievingPathsCoroutine = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(WaitForPathsToLoadThenLoadObjects());
			}
			else
			{
				Loading();
			}

			GameFacade.Instance.RetrieveMediator<ToolbarMediator>().TopBar.Showing = false;
			GameFacade.Instance.RetrieveMediator<ToolbarMediator>().NavigationBar.Showing = false;		
		}
		
		private void BuildGui()
		{
			IGuiManager manager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
			mRunwaySequenceGuiController = new GuiController(manager, PATH_TO_GUI);

			mExitButton = mRunwaySequenceGuiController.MainGui.SelectSingleElement<Button>("**/ExitButton");
			mExitButton.AddOnPressedAction(mOnExitCallback);
			mExitButton.Showing = false;
			GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(WaitThenDisplayExitButton());

			mTopPerformersFrame = mRunwaySequenceGuiController.MainGui.SelectSingleElement<GuiFrame>("**/TopPerformerFrame");

			// Build Title label
			// ISSUES WITH AVATAR PROXY.  Avatar proxy is set up when the local avatar distributed object is created.  This only happens
			// when the Green Screen Room is loaded first.
			Label titleLabel = mRunwaySequenceGuiController.MainGui.SelectSingleElement<Label>("**/TitleLabel");
			titleLabel.Text = String.Format(titleLabel.Text, GameFacade.Instance.RetrieveProxy<ConnectionProxy>().NickName, mLocationName);
			
			Pair<FacebookFriendInfo, FacebookFriendInfo> topPerformers = GetTopPerformers();
			if (topPerformers.First == null || topPerformers.Second == null)
			{
				mTopPerformersFrame.Showing = false;
				return;
			}
			// Populate the first ThankFrame:
			Image topPerformerImage1 = mRunwaySequenceGuiController.MainGui.SelectSingleElement<Image>("**/TopPerformerImage1");
			Label topPerformerNameLabel1 = mRunwaySequenceGuiController.MainGui.SelectSingleElement<Label>("**/TopPerformerNameLabel1");
			Button chooseButton1 = mRunwaySequenceGuiController.MainGui.SelectSingleElement<Button>("**/ChooseButton1");
			if (topPerformers.First != null)
			{
				if (topPerformers.First.ImageUrl != "")
				{
					GameFacade.Instance.RetrieveProxy<ClientAssetRepository>().LoadAssetFromPath<ImageAsset>(topPerformers.First.ImageUrl, delegate(ImageAsset asset)
					{
						topPerformerImage1.Texture = asset.Texture2D;
					});
				}
				topPerformerNameLabel1.Text = topPerformers.First.FirstName + " " + topPerformers.First.LastName;
				chooseButton1.AddOnPressedAction(delegate()
				{
					GameFacade.Instance.RetrieveMediator<FacebookFeedMediator>().PostFeed(topPerformers.First.FbAccountId, FashionMinigame.FACEBOOK_FEED_COPY_PATH, "feed_thanks", FeedPostWindowClosed, topPerformers.First.FirstName + " " + topPerformers.First.LastName);
					EventLogger.Log(LogGlobals.CATEGORY_FACEBOOK, LogGlobals.FACEBOOK_FEED_POST, "ThankTopPerformer", topPerformers.First.FirstName + " " + topPerformers.First.LastName);
				});
			}
			else
			{
				topPerformerNameLabel1.Showing = false;
				topPerformerImage1.Showing = false;
				chooseButton1.Showing = false;
			}

			Image topPerformerImage2 = mRunwaySequenceGuiController.MainGui.SelectSingleElement<Image>("**/TopPerformerImage2");
			Label topPerformerNameLabel2 = mRunwaySequenceGuiController.MainGui.SelectSingleElement<Label>("**/TopPerformerNameLabel2");
			Button chooseButton2 = mRunwaySequenceGuiController.MainGui.SelectSingleElement<Button>("**/ChooseButton2");
			if (topPerformers.Second != null)
			{
				if (topPerformers.Second.ImageUrl != "")
				{
					
					GameFacade.Instance.RetrieveProxy<ClientAssetRepository>().LoadAssetFromPath<ImageAsset>(topPerformers.Second.ImageUrl, delegate(ImageAsset asset)
					{
						topPerformerImage2.Texture = asset.Texture2D;
					});
				}
				topPerformerNameLabel2.Text = topPerformers.Second.FirstName + " " + topPerformers.Second.LastName;
				chooseButton2.AddOnPressedAction(delegate()
				{
					GameFacade.Instance.RetrieveMediator<FacebookFeedMediator>().PostFeed(topPerformers.Second.FbAccountId, FashionMinigame.FACEBOOK_FEED_COPY_PATH, "feed_thanks", FeedPostWindowClosed, topPerformers.Second.FirstName + " " + topPerformers.Second.LastName);
					EventLogger.Log(LogGlobals.CATEGORY_FACEBOOK, LogGlobals.FACEBOOK_FEED_POST, "ThankTopPerformer", topPerformers.Second.FirstName + " " + topPerformers.Second.LastName);
				});
			}
			else
			{
				topPerformerNameLabel2.Showing = false;
				topPerformerImage2.Showing = false;
				chooseButton2.Showing = false;
			}
		}
		
		private void FeedPostWindowClosed()
		{
			mTopPerformersFrame.Showing = false;
		}

		private Pair<FacebookFriendInfo, FacebookFriendInfo> GetTopPerformers()
		{
			List<FacebookFriendInfo> hiredFacebookFriends = GameFacade.Instance.RetrieveMediator<FashionNpcMediator>().HiredFacebookFriendInfos;
			
			//Small hack to remove yourself from the list of hired friends.
			FacebookFriendInfo usersFacebookInfo = null;
			foreach(FacebookFriendInfo friendInfo in hiredFacebookFriends)
			{
				if (friendInfo.FbAccountId == GameFacade.Instance.RetrieveProxy<ConnectionProxy>().FacebookAccountId)
				{
					usersFacebookInfo = friendInfo;
					break;
				}
			}
			if (usersFacebookInfo != null)
			{
				
				hiredFacebookFriends.Remove(usersFacebookInfo);
			}
			
			List<FacebookFriendInfo> nonHangoutFriends = new List<FacebookFriendInfo>();
			List<FacebookFriendInfo> hangoutFriends = new List<FacebookFriendInfo>();
			foreach (FacebookFriendInfo facebookInfo in hiredFacebookFriends)
			{
				if (facebookInfo.AccountId.Value == 0)
				{
					nonHangoutFriends.Add(facebookInfo);
				} 
				else
				{
					hangoutFriends.Add(facebookInfo);
				}
			}
			
			if (nonHangoutFriends.Count <= 2)
			{
				if (nonHangoutFriends.Count == 0)
				{
					if (hangoutFriends.Count > 1)
					{
						int firstPerformerIndex = UnityEngine.Random.Range(0, hangoutFriends.Count);
						int secondPerformerIndex = UnityEngine.Random.Range(0, hangoutFriends.Count);
						while(firstPerformerIndex == secondPerformerIndex)
						{
							secondPerformerIndex = UnityEngine.Random.Range(0, hangoutFriends.Count);
						}
						return new Pair<FacebookFriendInfo, FacebookFriendInfo>(hangoutFriends[firstPerformerIndex], hangoutFriends[secondPerformerIndex]);
					}
					if (hangoutFriends.Count == 1)
					{
						return new Pair<FacebookFriendInfo, FacebookFriendInfo>(hangoutFriends[0], null);
					}
					else
					{
						return new Pair<FacebookFriendInfo, FacebookFriendInfo>(null, null);
					}
				}
				if (nonHangoutFriends.Count == 1)
				{
					FacebookFriendInfo firstPerformer = nonHangoutFriends[0];
					if (hangoutFriends.Count >= 1)
					{
						int secondPerformerIndex = UnityEngine.Random.Range(0, hangoutFriends.Count);
						return new Pair<FacebookFriendInfo, FacebookFriendInfo>(firstPerformer, hangoutFriends[secondPerformerIndex]);
					}
					else
					{
						return new Pair<FacebookFriendInfo,FacebookFriendInfo>(firstPerformer, null);
					}
				}
				else
				{
					return new Pair<FacebookFriendInfo, FacebookFriendInfo>(nonHangoutFriends[0], nonHangoutFriends[1]);
				}
			}
			else
			{
				int firstPerformerIndex = UnityEngine.Random.Range(0, nonHangoutFriends.Count);
				int secondPerformerIndex = UnityEngine.Random.Range(0, nonHangoutFriends.Count);
				while (firstPerformerIndex == secondPerformerIndex)
				{
					secondPerformerIndex = UnityEngine.Random.Range(0, nonHangoutFriends.Count);
				}
				return new Pair<FacebookFriendInfo, FacebookFriendInfo>(nonHangoutFriends[firstPerformerIndex], nonHangoutFriends[secondPerformerIndex]);
			}
		}
		
		private IEnumerator<IYieldInstruction> WaitThenDisplayExitButton()
		{
			yield return new YieldForSeconds(5.0f);
			mExitButton.Showing = true;
		}
		
		private IEnumerator<IYieldInstruction> WaitForPathsToLoadThenLoadObjects()
		{
			yield return new YieldWhile(delegate()
			{
				return mPathToRunwayWalkAnimation.Length == 0 || mPathToRunwayEnvironment.Length == 0;
			});
			Loading();
		}
		
		private IEnumerator<IYieldInstruction> RunwayLoop()
		{
 			FashionModel[] fashionModelArray = new FashionModel[mFashionModels.Count];
 			mFashionModels.CopyTo(fashionModelArray, 0);

			float modelSpawnRate = mRunwayWalkClip.length / 2;
			if (mFashionModels.Count == 1)
			{
				modelSpawnRate = mRunwayWalkClip.length;
			}
			
 			int modelIndex = 0;
			StartModelRunwayWalk(fashionModelArray[modelIndex]);
			mActiveCameraTarget = fashionModelArray[modelIndex].UnityGameObject.transform;
			GameObject head = GameObjectUtility.GetNamedChildRecursive("Head", mActiveCameraTarget.gameObject);
			SetCameraTargets(head.transform);

			float time = 0.0f;
			float nextCameraChangeTimer = 0.0f;

			int cameraIndex = 0;
			if (mSwitchViewsTask != null)
			{
				mSwitchViewsTask.Exit();
			}
			mSwitchViewsTask = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(SwitchView(mCameraViews[cameraIndex].Key));
			nextCameraChangeTimer += mCameraViews[cameraIndex].Value;

			while(true)
			{
				if (time >= (modelIndex + 1) * modelSpawnRate)
				{
					++modelIndex;
					if (modelIndex >= fashionModelArray.Length)
					{
						if (modelIndex % 4 == 0 && modelIndex != 0)
						{
							nextCameraChangeTimer = time;
						}
					}
					StartModelRunwayWalk(fashionModelArray[modelIndex % fashionModelArray.Length]);
					mActiveCameraTarget = fashionModelArray[modelIndex % fashionModelArray.Length].UnityGameObject.transform;
				}

				if (time >= nextCameraChangeTimer)
				{
					++cameraIndex;
					if (mSwitchViewsTask != null)
					{
						mSwitchViewsTask.Exit();
					}
					mSwitchViewsTask = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(SwitchView(mCameraViews[cameraIndex % mCameraViews.Count].Key));
					nextCameraChangeTimer += (mCameraViews[cameraIndex % mCameraViews.Count].Value);
				}

				time += Time.deltaTime;
				yield return new YieldUntilNextFrame();
			}
		}
		
		private void SetCameraTargets(Transform targetTransform)
		{
			foreach (KeyValuePair<View, float> kvp in mCameraViews)
			{
				kvp.Key.Target = targetTransform;
			}
		}
		
		private void StartModelRunwayWalk(FashionModel fashionModel)
		{
			IScheduler scheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			
			fashionModel.UnityGameObject.SetActiveRecursively(true);
			fashionModel.UnityGameObject.transform.position = mLoadingPosition.position;
			fashionModel.UnityGameObject.transform.rotation = mLoadingPosition.rotation;
			fashionModel.Nametag.Showing = true;

			Animation animationComponent = fashionModel.UnityGameObject.GetComponentInChildren(typeof(Animation)) as Animation;
			SkinnedMeshRenderer skinnedMeshRenderer = fashionModel.UnityGameObject.GetComponentInChildren(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
			skinnedMeshRenderer.updateWhenOffscreen = true;

			if (animationComponent != null)
			{
				animationComponent.AddClip(mRunwayWalkClip, mRunwayWalkClip.name);
				animationComponent.Play(mRunwayWalkClip.name);
			}

			FashionModel model = fashionModel;
			ITask activeFashionModelTask = scheduler.StartCoroutine(CleanUpModelAfterRunwayWalk(model));
			if (mFashionModels.Count > 1)
			{
				activeFashionModelTask.AddOnExitAction(delegate()
				{
					CleanUpModel(model.UnityGameObject);
					fashionModel.Nametag.Showing = false;
				});	
			}
			mActiveModelTasks.Add(activeFashionModelTask);
		}
		
		private IEnumerator<IYieldInstruction> SwitchView(View view)
		{
			mActiveView = view;
			mSmoothDampTransform.RotateSmoothTime = view.RotateLag;
			mSmoothDampTransform.TranslateSmoothTime = view.TranslateLag;

			GameObject head = GameObjectUtility.GetNamedChildRecursive("Head", mActiveCameraTarget.gameObject);
			SetCameraTargets(head.transform);
			
			yield return new YieldUntilNextFrame();
			mSmoothDampTransform.SnapTo(mActiveView.Position, mActiveView.Rotation);
			GameFacade.Instance.RetrieveMediator<FashionCameraMediator>().Camera.fieldOfView = view.FieldOfView;
		}

		private IEnumerator<IYieldInstruction> SmoothlyAnimate()
		{
			while (true)
			{
				mSmoothDampTransform.AnimateTo(mActiveView.Position, mActiveView.Rotation);
				yield return new YieldUntilFixedUpdate();
			}
		}

		private IEnumerator<IYieldInstruction> ManageShow()
		{
			mSpotLight = GameObjectUtility.GetNamedChildRecursive("Spotlight", mRunwayGameObject);
			if (mSpotLight != null)
			{
				//FashionCameraMediator fashionCameraMediator = GameFacade.Instance.RetrieveMediator<FashionCameraMediator>();
				while (true)
				{
					mSpotLight.transform.LookAt(mActiveView.Target.position + Vector3.down * 1.1f, Vector3.up);
					yield return new YieldUntilNextFrame();
				}
			}
			else
			{
				// Just return one frame and don't have a spot light.
				Debug.LogError("Could not find spot light in environment object.");
				yield return new YieldUntilNextFrame();
			}
			
		}

		private void SetBackground(Texture2D backgroundTexture)
		{
			if (mRunwayGameObject != null)
			{
				MeshRenderer[] meshRenderers = mRunwayGameObject.GetComponentsInChildren<MeshRenderer>() as MeshRenderer[];
				if (meshRenderers != null)
				{
					foreach (MeshRenderer meshRenderer in meshRenderers)
					{
						if (meshRenderer.gameObject.name == "BackgroundPlane")
						{
							meshRenderer.material.mainTexture = backgroundTexture;
							break;
						}
						Debug.LogError("Could not find MeshRenderer on gameObject BackgroundPlane.");
					}
				}
				else
				{
					Debug.LogError("Could not find MeshRenderers on RunwayGameObject.");
				}
			}
			else
			{
				Debug.LogError("Trying to apply background before the runway environment has been loaded.");
			}
		}
		
		private IEnumerator<IYieldInstruction> CleanUpModelAfterRunwayWalk(FashionModel fashionModel)
		{
			yield return new YieldForSeconds(mRunwayWalkClip.length);
		}
		
		private void CleanUpModel(GameObject fashionModel)
		{
			Animation animationComponent = fashionModel.GetComponentInChildren(typeof(Animation)) as Animation;
			if (animationComponent != null)
			{
				animationComponent.Stop();
				animationComponent.RemoveClip(mRunwayWalkClip);
			}
			fashionModel.SetActiveRecursively(false);
		}
		
		public override void ExitState()
		{
			GameObject.Destroy(mMusicGameObject);
			foreach (KeyValuePair<View, float> kvp in mCameraViews)
			{
				kvp.Key.StopTracking();
			}
			foreach (GameObject cameraTransform in mCameraTransforms)
			{
				GameObject.Destroy(cameraTransform);
			}
			mRunwaySequenceGuiController.Dispose();
			if (mRetrievingPathsCoroutine != null)
			{
				mRetrievingPathsCoroutine.Exit();
			}
			if (mManageShowTask != null)
			{
				mManageShowTask.Exit();
			}
			if (mSwitchViewsTask != null)
			{
				mSwitchViewsTask.Exit();
			}
			if (mRunwayLoopTask != null)
			{
				mRunwayLoopTask.Exit();
			}
			if (mSmoothlyAnimateTask != null)
			{
				mSmoothlyAnimateTask.Exit();
			}
			
			foreach (KeyValuePair<View, float> kvp in mCameraViews)
			{
				kvp.Key.StopTracking();
			}
			
			foreach (ITask task in mActiveModelTasks)
			{
				if (task != null)
				{
					task.Exit();
				}
			}
			
			if (mActiveCameraTarget != null)
			{
				CleanUpModel(mActiveCameraTarget.gameObject);
			}
			
			if (mRunwayGameObject != null)
			{
				GameObject.Destroy(mRunwayGameObject);
			}

			if (GameFacade.Instance.HasMediator<FashionGameGui>())
			{
				foreach (ITopLevel fashionGui in GameFacade.Instance.RetrieveMediator<FashionGameGui>().AllGuis)
				{
					fashionGui.Showing = true;
				}
			}

			if( GameFacade.Instance.HasMediator<ToolbarMediator>() )
			{
				GameFacade.Instance.RetrieveMediator<ToolbarMediator>().TopBar.Showing = true;
				GameFacade.Instance.RetrieveMediator<ToolbarMediator>().NavigationBar.Showing = true;
			}
		}
		
		public override void Dispose()
		{
			ExitState();
			base.Dispose();
		}
	}
}
