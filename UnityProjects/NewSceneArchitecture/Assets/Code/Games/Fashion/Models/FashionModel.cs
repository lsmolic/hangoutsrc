/**  --------------------------------------------------------  *
 *   FashionModel.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/24/2009
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
	public class FashionModel : Npc
	{
		// TODO: Hard coded value
		public const float APPROX_AVATAR_HEIGHT = 1.75f;

		private const string NAMETAG_GUI_PATH = "resources://GUI/Minigames/Fashion/NpcNametag.gui";
		private const string mTweakablesPath = "resources://Fashion Game Settings/Gameplay.tweaks";
		private bool mReady = false;

		private FashionModelNeeds mNeeds = null;

		private FashionModelStateMachine mStateMachine = null;

		// If the avatar is completed in less than mCompletionBonusTime seconds, it 
		//  will be awarded bonus points.
		private float mCompletionBonusTime = 5.0f;

		// Each station's wait time is added to the CompletionBonusTime 
		//  as well as this extra time to allow for walking.
		private readonly float mWalkToStationTimeBonus = 1.5f;

		// used to see if the user should get the bonus "Close One" for the next station clear
		private bool mStationInUseAtStartWalk;

		private const string APPLY_CLOTHING_SFX = "assets://Sounds/phoo03.ogg";
		private AudioClip mApplyClothingSfx;
		private const string ERROR_SFX_PATH = "assets://Sounds/generic_ui_10a.ogg";
		private AudioClip mErrorSfx;

		// All the model's components' materials
		private readonly List<Material> mModelMaterials = new List<Material>();

		private readonly IDictionary<ItemId, IWidget> mDesiredClothing = new Dictionary<ItemId, IWidget>();
		private readonly IDictionary<ModelStation, IWidget> mDesiredStations = new Dictionary<ModelStation, IWidget>();

		private Window mDesiredClothingWindow = null;
		public ITopLevel DesiredClothingWindow
		{
			get { return mDesiredClothingWindow; }
		}

		private IGuiFrame mDesiredClothingFrame = null;

		// fast complete bonus is set to false if the bonus time is passed
		private bool mEarnedBonus = true;

		private readonly IScheduler mScheduler;

		private FashionLevel mLevel;
		private ITask mWalkingTask = null;
		private Hangout.Shared.Action mTargetReachedCallback = null;

		private FollowWorldSpaceObject mFollowWorldSpaceObject;

		private bool mAnimationsSetup = false;

		private AnimationClip mActiveWalkCycle = null;
		private float mActiveWalkCycleSpeed = 1.0f;

		private AnimationClip mImpatientSitIdle = null;
		private AnimationClip mSittingIdle = null;

		private float mMediumHurriedSpeed = 0.5f;
		private AnimationClip mMediumHurried = null;

		private float mCatWalkSpeed = 1.0f;
		private AnimationClip mCatWalk = null;

		private float mWalkSpeed;
		private readonly GuiController mNametag;
		public ITopLevel Nametag
		{
			get { return mNametag.MainGui; }
		}

		private ITask mHandleBonusTask = null;

		public ICollection<ModelStation> RequiredStations
		{
			get 
			{ 
				if( mNeeds == null )
				{
					throw new Exception("Fashion Model needs have not been set up yet.");
				}
				return mNeeds.Stations; 
			}
		}

		public ICollection<ItemId> RequiredClothes
		{
			get
			{
				if (mNeeds == null)
				{
					throw new Exception("Fashion Model needs have not been set up yet.");
				}
				return mNeeds.Clothing; 
			}
		}

		public override bool Active
		{
			get 
			{
				bool result = false;
				if (mStateMachine != null)
				{
					result = !(mStateMachine.CurrentFashionState is ModelInactiveState);
				}
				return result;
			}
		}

		/// <summary>
		/// If this avatar were to be completed right now, would it be worth fast-complete bonus points?
		/// </summary>
		public bool EarnedBonus
		{
			get { return mEarnedBonus; }
		}

		public float WalkSpeed
		{
			get { return mWalkSpeed; }
			set 
			{
				if (value <= 0.0f)
				{
					throw new ArgumentOutOfRangeException("WalkSpeed needs to be greater than 0");
				}
				mWalkSpeed = value;
			}
		}

		/// <summary>
		/// Creates a new FashionModel Entity
		/// </summary>
		/// <param name="start">Pair of Vector3s, first = Position, second = Direction</param>
		/// <param name="walkSpeed">Constant speed that the fashion models walk at</param>
		public FashionModel(string name, FashionLevel level, GameObject displayObject, HeadController headController,  BodyController bodyController)
			: base(name, displayObject, headController, bodyController)
		{
			if (level == null)
			{
				throw new ArgumentNullException("level");
			}
			mLevel = level;

			mScheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;

			ClientAssetRepository assetRepo = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			assetRepo.LoadAssetFromPath<SoundAsset>(APPLY_CLOTHING_SFX, delegate(SoundAsset asset)
			{
				mApplyClothingSfx = asset.AudioClip;
			});
			assetRepo.LoadAssetFromPath<SoundAsset>(ERROR_SFX_PATH, delegate(SoundAsset asset)
			{
				mErrorSfx = asset.AudioClip;
			});

			mNametag = SetupNametag(name, displayObject);
		}

		private GuiController SetupNametag(string name, GameObject displayObject)
		{
			GuiController nametag = new GuiController(GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>(), NAMETAG_GUI_PATH);
			Transform headTransform = GameObjectUtility.GetNamedChildRecursive("Head", displayObject).transform;
			nametag.Manager.SetTopLevelPosition
			(
				nametag.MainGui,
				new FollowWorldSpaceObject
				(
					GameFacade.Instance.RetrieveMediator<FashionCameraMediator>().Camera,
					headTransform,
					GuiAnchor.CenterCenter,
					Vector2.zero,
					new Vector3(0.0f, 0.3f, 0.0f) // TODO: Hard coded value
				)
			);
			ITextGuiElement nameElement = nametag.MainGui.SelectSingleElement<ITextGuiElement>("**/NameLabel");
			nameElement.Text = String.Format(nameElement.Text, name);

			nametag.MainGui.Showing = false;

			return nametag;
		}

		public void WalkTo(Pair<Vector3> target, Hangout.Shared.Action onTargetReachedCallback)
		{
			if (mWalkingTask != null)
			{
				mWalkingTask.Exit();
			}

			PlayWalkCycleAnimation();

			mWalkingTask = mScheduler.StartCoroutine(WalkingTask(target, onTargetReachedCallback));
		}

		private IEnumerator<IYieldInstruction> HandleBonus()
		{
			float startTime = Time.time;
			while (Time.time - startTime < mCompletionBonusTime)
			{
				yield return new YieldUntilNextFrame();
			}

			if (!this.Ready)
			{
				mEarnedBonus = false;
			}
		}

		public void SetupAnimationClips()
		{
			if (!mAnimationsSetup)
			{
				Animation animationComponent = UnityGameObject.GetComponentInChildren(typeof(Animation)) as Animation;

				if (animationComponent == null)
				{
					throw new Exception("Unable to find the animation component under " + UnityGameObject.name);
				}

				if( mImpatientSitIdle == null ||
					mSittingIdle == null ||
					mMediumHurried == null ||
					mCatWalk == null )
				{
					throw new Exception
					(
						"Not all the animations are set up on FashionModel. Still need; " + 
						(mImpatientSitIdle == null ? "mImpatientSitIdle " : "") +
						(mSittingIdle == null ? "mSittingIdle " : "") +
						(mMediumHurried == null ? "mMediumHurried " : "") +
						(mCatWalk == null ? "mCatWalk " : "")
					);
				}

				animationComponent.AddClip(mImpatientSitIdle, mImpatientSitIdle.name);
				animationComponent.AddClip(mSittingIdle, mSittingIdle.name);
				animationComponent.AddClip(mMediumHurried, mMediumHurried.name);
				animationComponent.AddClip(mCatWalk, mCatWalk.name);

				mAnimationsSetup = true;
			}
		}

		public void SetImpatientSitIdle(AnimationClip clip)
		{
			if( clip == null )
			{
				throw new ArgumentNullException("clip");
			}
			mImpatientSitIdle = clip;
		}
		
		public void SetSittingIdle(AnimationClip clip)
		{
			if (clip == null)
			{
				throw new ArgumentNullException("clip");
			}
			mSittingIdle = clip;
		}

		public void SetMediumHurried(AnimationClip clip, float walkSpeed)
		{
			if (clip == null)
			{
				throw new ArgumentNullException("clip");
			}
			mMediumHurried = clip;
			mMediumHurriedSpeed = walkSpeed;
		}

		public void SetCatWalk(AnimationClip clip, float walkSpeed)
		{
			if (clip == null)
			{
				throw new ArgumentNullException("clip");
			}
			mCatWalk = clip;
			mCatWalkSpeed = walkSpeed;
		}

		/// <summary>
		/// Starts a walk down the runway
		/// </summary>
		public void SetActive(FashionModelNeeds needs, FashionLevel level)
		{
			if( needs == null )
			{
				throw new ArgumentNullException("needs");
			}
			mNeeds = needs;

			if( level == null )
			{
				throw new ArgumentNullException("level");
			}
			mLevel = level;

			mNametag.MainGui.Showing = true;
			
			mDesiredClothing.Clear();
			if (mDesiredClothingFrame != null)
			{
				mDesiredClothingFrame.ClearChildWidgets();
			}
			mDesiredStations.Clear(); 
			
			mStateMachine = new FashionModelStateMachine(this, mLevel);

			UnityGameObject.transform.position = mLevel.Start.First;

			mActiveWalkCycle = mCatWalk;
			mActiveWalkCycleSpeed = mCatWalkSpeed;

			mCompletionBonusTime = 5.0f;
			mReady = false;

			mHandleBonusTask = mScheduler.StartCoroutine(HandleBonus());

			IGuiManager manager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
			mDesiredClothingFrame = new GuiFrame("MainFrame", new MainFrameSizePosition());

			IGuiStyle windowStyle = new GuiStyle(manager.GetDefaultStyle(typeof(Window)), "ModelNeedsWindow");
			windowStyle.Normal.Background = null;
			windowStyle.Hover.Background = null;
			
			// TODO: Hard coded values
			float windowHeight = 192.0f;
			mDesiredClothingWindow = new Window
			(
				"ModelClothingPanel",
				new FixedSize(128.0f, windowHeight),
				manager,
				mDesiredClothingFrame,
				windowStyle
			);

			// TODO: Hard coded values
			mFollowWorldSpaceObject = new FollowWorldSpaceObject
			(
				GameFacade.Instance.RetrieveMediator<FashionCameraMediator>().Camera,
				this.DisplayObject.transform,
				GuiAnchor.CenterLeft,
				new Vector2(0.0f, windowHeight * 0.4f),
				new Vector3(0.125f, APPROX_AVATAR_HEIGHT * 1.3f, 0.25f)
			);

			manager.SetTopLevelPosition
			(
				mDesiredClothingWindow,
				mFollowWorldSpaceObject
			);

			ClothingMediator clothingMediator = GameFacade.Instance.RetrieveMediator<ClothingMediator>();

			// Setup Clothes GUI
			foreach (ItemId clothing in mNeeds.Clothing)
			{
				Image desiredClothingImage = new Image("DesiredClothingLabel", clothingMediator.GetThumbStyle(clothing), clothingMediator.GetThumbnail(clothing));
				mDesiredClothing.Add(clothing, desiredClothingImage);
				mDesiredClothingFrame.AddChildWidget(desiredClothingImage, new HorizontalAutoLayout());
			}

			// Setup Station GUI
			foreach (ModelStation station in mNeeds.Stations)
			{
				Image desiredStationImage = new Image("DesiredStationImage", station.Image);

				mDesiredStations.Add(station, desiredStationImage);
				mDesiredClothingFrame.AddChildWidget(desiredStationImage, new HorizontalAutoLayout());

				mCompletionBonusTime += station.WaitTime + mWalkToStationTimeBonus;
			}

			this.DisplayObject.SetActiveRecursively(true);

			Shader fashionModelShader = Shader.Find("Avatar/Fashion Model");
			if (fashionModelShader == null)
			{
				throw new Exception("Cannot find 'Avatar/Fashion Model' shader");
			}

			mModelMaterials.Clear();
			foreach (Component component in UnityGameObject.GetComponentsInChildren(typeof(Renderer)))
			{
				Renderer renderer = (Renderer)component;
				foreach (Material mat in renderer.materials)
				{
					mat.shader = fashionModelShader;
					mModelMaterials.Add(mat);
				}
			}

			mNeeds.AddOnCompleteAction(ModelComplete);
		}

		private void CleanupWindow()
		{
			if (mDesiredClothingWindow != null)
			{
				mDesiredClothingWindow.Close();
				mDesiredClothingWindow = null;
				mDesiredClothingFrame = null;
			}
		}

		/// <summary>
		/// Register a callback that will be executed when the Model has reached the end of the gameplay space (Level ModelDrain)
		/// </summary>
		public void AddOnTargetReachedAction(Hangout.Shared.Action onTargetReached)
		{
			mTargetReachedCallback += onTargetReached;
		}

		/// <summary>
		/// Called by the ModelDefaultState when the model gets to the end point
		/// </summary>
		public void TargetReached()
		{
			if (mTargetReachedCallback != null)
			{
				mTargetReachedCallback();
			}
		}

		public void WalkToCenter()
		{
			mStateMachine.TransitionToState(mStateMachine.WalkToCenterState);
		}

		public void WalkToEndTarget()
		{
			mStateMachine.TransitionToState(mStateMachine.WalkToEndGoalState);
		}

		// Distance to the walk target that the model starts to ignore collisions and just go to the end
		private const float DISTANCE_TO_TARGET_CHECK = 1.0f;
		private const float COLLISION_CHECK_RADIUS = 2.0f;
		
		private IEnumerator<IYieldInstruction> WalkingTask(Pair<Vector3> target, Hangout.Shared.Action onTargetReachedCallback)
		{
			Vector3 headingSmooth = this.UnityGameObject.transform.forward;
			do
			{
				this.DisplayObject.animation[mActiveWalkCycle.name].speed = mWalkSpeed * mActiveWalkCycleSpeed;

				yield return new YieldUntilNextFrame();

				Vector3 position = this.UnityGameObject.transform.position;
				Vector3 positionDifference = target.First - position;
				float distanceToTarget = positionDifference.magnitude;
				if( distanceToTarget < 0.00001f )
				{
					// Avoid divide by zero and floating point rounding weirdness
					break;
				}
				Vector3 directionToTarget = positionDifference / distanceToTarget;
				Vector3 heading = directionToTarget;

				ConfigManagerClient configManager = GameFacade.Instance.RetrieveProxy<ConfigManagerClient>();

				bool enableCollisionAvoidance = configManager.GetBool("enableCollisionAvoidance", true);

				if (enableCollisionAvoidance)
				{
					Vector3 avoidCollisionsVect = Vector3.zero;
					int collisionLayer = mStateMachine.CurrentFashionState.CollisionCheckLayer;
					foreach (Collider collider in Physics.OverlapSphere(position, COLLISION_CHECK_RADIUS, collisionLayer))
					{
						if (collider.gameObject == UnityGameObject)
						{
							continue;
						}

						Vector3 colliderToModel = collider.gameObject.transform.position - position;
						
						float weight = -1.0f / Mathf.Max(colliderToModel.sqrMagnitude, 0.01f); // inverse square weight
						Vector3 avoidCollisionInfluence = colliderToModel.normalized * weight;
						if( Vector3.Dot(avoidCollisionInfluence, positionDifference) > 0.3f ) // make sure we only influence towards the end goal
						{
							avoidCollisionsVect += avoidCollisionInfluence;
						}
					}

					DebugUtility.DrawCicrleXZ(position, COLLISION_CHECK_RADIUS, Color.yellow);

					if (avoidCollisionsVect.magnitude > 0.0f)
					{
						Debug.DrawLine(position, position + avoidCollisionsVect, Color.yellow);

						heading += avoidCollisionsVect * Mathf.Clamp01(distanceToTarget * DISTANCE_TO_TARGET_CHECK);
						heading.Normalize();

						headingSmooth += heading * Time.deltaTime;
						headingSmooth.Normalize();

						Debug.DrawLine(position, position + headingSmooth, Color.green);
						heading = headingSmooth;
					}
					else
					{
						headingSmooth = heading;
					}
				}
				else
				{
					headingSmooth = heading;
				}

				this.UnityGameObject.transform.position += headingSmooth * mWalkSpeed * Time.deltaTime;

				// Walk towards the target
				this.UnityGameObject.transform.forward = headingSmooth;
			}
			while (DistanceTo(target.First) > (mWalkSpeed * Time.deltaTime * 1.1f));

			this.UnityGameObject.transform.position = target.First;
			this.UnityGameObject.transform.forward = target.Second;

			if (onTargetReachedCallback != null)
			{
				onTargetReachedCallback();
			}
		}

		private Hangout.Shared.Action mOnUnselected;
		public void Select(Hangout.Shared.Action onUnselected)
		{
			mOnUnselected = onUnselected;
			this.Clickable = false;
			mModelMaterials.ForEach
			(
				delegate(Material m)
				{
					// TODO: Hard coded value
					m.SetFloat("_Selection", 0.5f);
				}
			);
		}

		public void Unselect()
		{
			this.Clickable = true;
			mModelMaterials.ForEach
			(
				delegate(Material m)
				{
					m.SetFloat("_Selection", 0.0f);
				}
			);

			if (mOnUnselected != null)
			{
				mOnUnselected();
			}
			mOnUnselected = null;

			GameFacade.Instance.RetrieveMediator<FashionGameInput>().Unselect(this);
		}

		public bool Clickable
		{
			get { return this.UnityGameObject.layer == FashionMinigame.MODEL_LAYER; }
			set
			{
				if (this.UnityGameObject != null)
				{
					if (!value)
					{
						this.UnityGameObject.layer = FashionMinigame.UNSELECTABLE_MODEL_LAYER;
					}
					else
					{
						this.UnityGameObject.layer = FashionMinigame.MODEL_LAYER;
					}
				}
			}
		}

		public void ClearNeeds()
		{
			mNeeds = null;
		}

		public void PlaySittingIdleAnimation()
		{
			this.DisplayObject.animation[mSittingIdle.name].speed = 1.0f;
			this.DisplayObject.animation[mSittingIdle.name].wrapMode = WrapMode.Loop;
			this.DisplayObject.animation.Play(mSittingIdle.name, AnimationPlayMode.Stop);
		}

		public void ArrivedAtStation(ModelStation station)
		{
			if( mNeeds != null )
			{
				if (mStationInUseAtStartWalk && !station.InUse)
				{
					GameFacade.Instance.SendNotification
					(
						FashionMinigame.EARNED_EXPERIENCE_NOTIFICATION,
						new ExperienceInfo
						(
							ExperienceType.CloseSave,
							station.UnityGameObject.transform.position + Vector3.up
						)
					);
				}

				IWidget widget;
				if (station.InUse)
				{
					// Play a gentle error sfx
					if (mErrorSfx != null)
					{
						AudioSource.PlayClipAtPoint(mErrorSfx, station.UnityGameObject.transform.position, 0.8f);
					}
					mStateMachine.TransitionToState(mStateMachine.WalkToCenterState);
				}
				else if (station is HoldingStation)
				{
					mStateMachine.TransitionToStayAtStation(station);
				}
				else if (mDesiredStations.TryGetValue(station, out widget))
				{
					mStationInUseAtStartWalk = station.InUse;
					mDesiredClothingFrame.RemoveChildWidget(widget);
					mDesiredStations.Remove(station);
					mNeeds.Remove(station);

					mStateMachine.TransitionToStayAtStation(station);
					mStateMachine.RemoveStation(station);
				}
			}
		}

		public bool ApplyClothing(ClothingItem clothingItem)
		{
			bool removed = false;

			if (mNeeds != null)
			{
				IWidget clothingGui;

				if (mDesiredClothing.TryGetValue(clothingItem.ItemId, out clothingGui))
				{
					mDesiredClothingFrame.RemoveChildWidget(clothingGui);
					mDesiredClothing.Remove(clothingItem.ItemId);
					mNeeds.Remove(clothingItem.ItemId);

					removed = true;
					if (clothingItem.NeedsFixin)
					{
						mLevel.UnfixedClothing++;
					}

					if (mApplyClothingSfx != null)
					{
						AudioSource.PlayClipAtPoint(mApplyClothingSfx, UnityGameObject.transform.position);
					}

					// If this clothing completed this model and it was close to the end, get a bonus
					if (this.Ready && this.DistanceTo(mLevel.End.First) < mLevel.CloseSaveDistance)
					{
						GameFacade.Instance.SendNotification
						(
							FashionMinigame.EARNED_EXPERIENCE_NOTIFICATION,
							new ExperienceInfo
							(
								ExperienceType.CloseSave
							)
						);
					}

					//Add asset to model
					AddAssetsToModel(clothingItem.Assets);
				}
			}

			return removed;
		}

		private void AddAssetsToModel(IEnumerable<Asset> assets)
		{
			this.BodyController.SetAssets(assets);
			//this.HeadController.SetAssets(assets);
			this.BodyController.UpdateAssetsOverFrames(10);
			//this.HeadController.UpdateAssets();
		}

		public void GoToStation(ModelStation station)
		{
			if (mNeeds != null)
			{
				mStationInUseAtStartWalk = false;

				// Might not actually transition if the avatar doesn't need this station
				if (mStateMachine.TransitionToWalkToStation(station))
				{
					if (station.InUse)
					{
						mStationInUseAtStartWalk = true;
					}
				}
			}
		}

		/// <summary>
		/// Has the user completely set up this model?
		/// </summary>
		public bool Ready
		{
			get { return mReady; }
		}

		/// <summary>
		/// Sets this model to complete (if there are still active needs, this method will clear them)
		/// </summary>
		private void ModelComplete()
		{
			if (!mReady)
			{
				mReady = true;


				GameFacade.Instance.SendNotification
				(
					FashionMinigame.EARNED_EXPERIENCE_NOTIFICATION,
					new ExperienceInfo
					(
						ExperienceType.ModelComplete,
						this.UnityGameObject.transform.position
					)
				);

				this.Clickable = false;

				//TODO: Hard coded value
				WalkSpeed = 3.0f;
				mActiveWalkCycle = mMediumHurried;
				mActiveWalkCycleSpeed = mMediumHurriedSpeed;

				if( !(mStateMachine.CurrentFashionState is AtStationState) )
				{
					PlayWalkCycleAnimation();
				}
			}
		}

		private void PlayWalkCycleAnimation()
		{
			this.DisplayObject.animation[mActiveWalkCycle.name].speed = mActiveWalkCycleSpeed;
			this.DisplayObject.animation[mActiveWalkCycle.name].wrapMode = WrapMode.Loop;
			this.DisplayObject.animation.Play(mActiveWalkCycle.name, AnimationPlayMode.Stop);
		}

		public void SetToInactive()
		{
			CleanupAsyncFunctions();
			CleanupWindow();

			mNametag.MainGui.Showing = false;
			mStateMachine.TransitionToInactiveState();
		}

		private void CleanupAsyncFunctions()
		{
			if (mWalkingTask != null)
			{
				mWalkingTask.Exit();
				mWalkingTask = null;
			}

			if (mHandleBonusTask != null)
			{
				mHandleBonusTask.Exit();
				mHandleBonusTask = null;
			}

			mTargetReachedCallback = null;
			if (mFollowWorldSpaceObject != null)
			{
				mFollowWorldSpaceObject.Dispose();
			}
		}

		public override void Dispose()
		{
			CleanupAsyncFunctions();
			mNametag.Dispose();
			base.Dispose();
		}
	}
}
