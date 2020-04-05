/**  --------------------------------------------------------  *
 *   FashionGameInput.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/01/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;

using PureMVC.Patterns;

using UnityEngine;

using Hangout.Shared;
using Hangout.Client.Gui;

namespace Hangout.Client.FashionGame
{
	public class FashionGameInput : Mediator
	{		
		private readonly FashionGameSelection mSelection;
		public FashionGameSelection Selection
		{
			get { return mSelection; }
		}

		private Vector3 mMousePosition;
		private const string APPLY_CLOTHING_SFX = "assets://Sounds/phoo03.ogg";
		private AudioClip mApplyClothingSfx;
		private const string ERROR_SFX_PATH = "assets://Sounds/generic_ui_10a.ogg";
		private AudioClip mErrorSfx;

		public FashionGameInput()
		{
			mSelection = new FashionGameSelection();

			// Uncomment this to see the raycasts used from this class
			//GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(Debug());

			ClientAssetRepository assetRepo = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			assetRepo.LoadAssetFromPath<SoundAsset>(APPLY_CLOTHING_SFX, delegate(SoundAsset asset)
			{
				mApplyClothingSfx = asset.AudioClip;
			});
			assetRepo.LoadAssetFromPath<SoundAsset>(ERROR_SFX_PATH, delegate(SoundAsset asset)
			{
				mErrorSfx = asset.AudioClip;
			});
		}

		// TODO: This should be using the InputManager already registered in the GameFacade
		//			but it isn't cause after transitioning into the minigame, it's borked.
		private IReceipt mRegisterForMouseDownReceipt = null;
		private IReceipt mRegisterForMouseUpReceipt = null;
		private IReceipt mRegisterForMousePositionReceipt = null;

		private bool mInputStarted = false;
		private InputManagerMediator mInputManager = new InputManagerMediator();
		public void StartListeningForInput()
		{
			if( !mInputStarted )
			{
				mInputStarted = true;
				mInputManager.StartInput();

				mRegisterForMouseDownReceipt = mInputManager.RegisterForButtonDown(KeyCode.Mouse0, MouseDown);
				mRegisterForMouseUpReceipt = mInputManager.RegisterForButtonUp(KeyCode.Mouse0, MouseUp);
				mRegisterForMousePositionReceipt = mInputManager.RegisterForMousePosition(MousePosition);

				if (Application.isEditor)
				{
					mInputManager.RegisterForButtonDown(KeyCode.Semicolon, delegate()
					{
						GameFacade.Instance.RetrieveMediator<FashionLevel>().ReloadStationPositions();
					});
				}
			}
		}

		public void Unselect(FashionModel model)
		{
			if( model == mSelection.Model )
			{
				mSelection.ClearModel();
			}
		}

		private void CastRaysThruClothing(Action<Ray> rayCalculatedStrategy)
		{
			if( mSelection.TopClothing == null )
			{
				return; // No clothing selected
			}
			// cast 9 rays thru the clothing billboard, corners, midpoints and center
			float shift = mSelection.TopClothing.OriginalBilboardSize;
			Vector3 clothingPosition = mSelection.TopClothing.BillboardPosition;
			Vector3 screenPosition = GameFacade.Instance.RetrieveMediator<FashionCameraMediator>().Camera.transform.position;
			Ray raycastRay = new Ray();
			raycastRay.origin = screenPosition;

			int samplesPerAxis = 3;
			float step = 2.0f / (float)(samplesPerAxis - 1);
			float x = -1.0f;
			
			for (int i = 0; i < samplesPerAxis; ++i)
			{
				if( mSelection.TopClothing == null )
				{
					throw new Exception("Something cleared the clothing before checking if the clothing was dropped on a gameplay object.");
				}

				Vector3 rightVector = mSelection.TopClothing.RightVector * x * shift;
				float z = -1.0f;
				for (int j = 0; j < samplesPerAxis; ++j)
				{
					Vector3 upVector = mSelection.TopClothing.UpVector * z * shift;
					raycastRay.direction = ((clothingPosition + rightVector + upVector) - screenPosition).normalized;
					rayCalculatedStrategy(raycastRay);
					z += step;
				}
				x += step;
			}
		}

		private IEnumerator<IYieldInstruction> Debug()
		{
			while(true)
			{
				if (mSelection.TopClothing != null)
				{
					CastRaysThruClothing(delegate(Ray castRay)
					{
						UnityEngine.Debug.DrawRay(castRay.origin, castRay.direction, Color.green);
					});
				}

				yield return new YieldUntilNextFrame();
			}
		}

		private void MousePosition(Vector3 position)
		{
			mMousePosition = position;
			FashionCameraMediator fashionCam = GameFacade.Instance.RetrieveMediator<FashionCameraMediator>();
			Ray mouseThruScreenRay = fashionCam.Camera.ScreenPointToRay(position);

			// Update the clothing icon's world position
			mSelection.UpdateClothingPosition(mouseThruScreenRay.GetPoint(fashionCam.Camera.nearClipPlane * 1.1f));

			// Do Mouseovers
			RaycastHit rh;
			if ( GameFacade.Instance.HasMediator<FashionLevel>() && Physics.Raycast(mouseThruScreenRay, out rh, Mathf.Infinity, 1 << FashionMinigame.STATION_LAYER))
			{
				FashionGameStation station = GameFacade.Instance.RetrieveMediator<FashionLevel>().GetStationFromGameObject(rh.collider.gameObject);
				if( station != null )
				{
					station.MouseIsOver();
				}
			}
		}

		private void MouseDown()
		{
			RaycastHit rh;
			Ray mouseThruScreenRay = GameFacade.Instance.RetrieveMediator<FashionCameraMediator>().Camera.ScreenPointToRay(mMousePosition);

			GameFacade.Instance.RetrieveMediator<FashionGameGui>().MouseDown();
			if( !GameFacade.Instance.HasMediator<FashionLevel>() )
			{
				return;
			}

			bool clickHadEffect = false;
			FashionLevel level = GameFacade.Instance.RetrieveMediator<FashionLevel>();

			if( !GameFacade.Instance.RetrieveMediator<FashionGameGui>().OccludesScreenPoint(mMousePosition) )
			{
				if (Physics.Raycast(mouseThruScreenRay, out rh, Mathf.Infinity, 1 << FashionMinigame.STATION_LAYER))
				{
					FashionGameStation station = level.GetStationFromGameObject(rh.collider.gameObject);
					if (station is TailorStation)
					{
						TailorStation tailorStation = (TailorStation)station;
						if( tailorStation.HasReadyClothing )
						{
							mSelection.Select(tailorStation.RetrieveFixedClothing());
							clickHadEffect = true;
						}
					}
				}

				if (Physics.Raycast(mouseThruScreenRay, out rh, Mathf.Infinity, 1 << FashionMinigame.CLOTHING_LAYER) )
				{
					Transform billboardHit = rh.collider.transform;
					foreach(TailorStation tailorStation in level.TailorStations)
					{
						if (tailorStation != null && 
							tailorStation.CurrentClothing != null && 
							tailorStation.CurrentClothing.UnityGameObject.transform == billboardHit)
						{
							mSelection.Select(tailorStation.RetrieveFixedClothing());
							clickHadEffect = true;
							break;
						}
					}
				}

				if (mSelection.TopClothing == null)
				{
					if (Physics.Raycast(mouseThruScreenRay, out rh, Mathf.Infinity, 1 << FashionMinigame.STATION_LAYER))
					{
						FashionGameStation station = level.GetStationFromGameObject(rh.collider.gameObject);
						if (station is ModelStation)
						{
							clickHadEffect = mSelection.Select((ModelStation)station);
						}
					}

					if (Physics.Raycast(mouseThruScreenRay, out rh, Mathf.Infinity, 1 << FashionMinigame.MODEL_LAYER))
					{
						mSelection.ClearModel();

						mSelection.Select( level.GetModelFromGameObject(rh.collider.gameObject) );
						FashionModel thisModel = mSelection.Model;
						mSelection.Model.AddOnTargetReachedAction(delegate()
						{
							// If the selection is still this model, unselect it
							if (mSelection.Model == thisModel)
							{
								mSelection.ClearModel();
							}
						});
						clickHadEffect = true;
					}
				}
			}
			else
			{
				clickHadEffect = DropClothing(mouseThruScreenRay);
			}

			if (!clickHadEffect && !MouseIsOverGui(level))
			{
				EventLogger.LogNoMixPanel
				(
					LogGlobals.CATEGORY_FASHION_MINIGAME, 
					LogGlobals.GAMEPLAY_BEHAVIOR,
					LogGlobals.USELESS_CLICK,
					level.Name + " (" + mMousePosition.x.ToString("f0") + " " + mMousePosition.y.ToString("f0") + ")"
				);
			}
		}

		/// <summary>
		/// Does not count any of the GUIs made by models or stations that don't effect gameplay
		/// </summary>
		public bool MouseIsOverGui(FashionLevel level)
		{
			bool mouseOverImportantGui = false;
			foreach (ITopLevel topLevel in GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>().OccludingTopLevels(mMousePosition))
			{
				if( !IsModelOrStationTopLevel(topLevel, level) )
				{
					mouseOverImportantGui = true;
					break;
				}
			}

			return mouseOverImportantGui;
		}

		private bool IsModelOrStationTopLevel(ITopLevel topLevel, FashionLevel level)
		{
			foreach (FashionGameStation station in level.Stations)
			{
				if (topLevel == station.MainGui)
				{
					return true;
				}
			}

			foreach (FashionModel model in level.ActiveModels)
			{
				if (topLevel == model.DesiredClothingWindow || topLevel == model.Nametag)
				{
					return true;
				}
			}
			return false;
		}

		private bool DropClothing(Ray mouseThruScreenRay)
		{
			bool clickHadEffect = false;
			if (mSelection.NeedsFixin)
			{
				List<RaycastHit> stationHits = new List<RaycastHit>();
				CastRaysThruClothing(delegate(Ray castRay)
				{
					stationHits.AddRange(Physics.RaycastAll(castRay, Mathf.Infinity, 1 << FashionMinigame.STATION_LAYER));
				});

				foreach (RaycastHit hit in stationHits)
				{
					FashionGameStation station = GameFacade.Instance.RetrieveMediator<FashionLevel>().GetStationFromGameObject(hit.collider.gameObject);
					if (station is TailorStation)
					{
						foreach (ClothingItem item in mSelection.Clothes)
						{
							if (item.NeedsFixin)
							{
								((TailorStation)station).AddClothing(item);
								mSelection.RemoveClothing(item);
								clickHadEffect = true;
								break;
							}
						}
					}
				}
			}

			int layerMask = (1 << FashionMinigame.MODEL_LAYER) | (1 << FashionMinigame.UNSELECTABLE_MODEL_LAYER);
			List<RaycastHit> hits = new List<RaycastHit>();
			CastRaysThruClothing(delegate(Ray castRay)
			{
				hits.AddRange(Physics.RaycastAll(castRay, Mathf.Infinity, layerMask));
			});

			FashionModel modelToSelect = null;
			int removed = 0;
			foreach (RaycastHit hit in hits)
			{
				FashionModel model = GameFacade.Instance.RetrieveMediator<FashionLevel>().GetModelFromGameObject(hit.collider.gameObject);
				if( model == null )
				{
					continue;
				}

				List<ClothingItem> toRemove = new List<ClothingItem>();
				foreach (ClothingItem item in mSelection.Clothes)
				{
					if (model.ApplyClothing(item))
					{
						if (model.RequiredStations.Count != 0)
						{
							modelToSelect = model;
						}
						toRemove.Add(item);
					}
				}

				removed += toRemove.Count;

				foreach (ClothingItem item in toRemove)
				{
					mSelection.RemoveClothing(item);
				}
			}

			// Bonus score for multiple clothing applications
			if (removed > 1)
			{
				GameFacade.Instance.SendNotification
				(
					FashionMinigame.EARNED_EXPERIENCE_NOTIFICATION,
					new ExperienceInfo
					(
						ExperienceType.MultipleClothingPlacement,
						mouseThruScreenRay.GetPoint(1.0f),
						(uint)removed
					)
				);
			}

			// Sound effects
			if (removed >= 1)
			{
				// Play an apply clothing sfx if anybody got clothing from this drop
				if (mApplyClothingSfx != null)
				{
					GameObject mainCamera = GameFacade.Instance.RetrieveMediator<CameraManagerMediator>().MainCamera;
					AudioSource.PlayClipAtPoint(mApplyClothingSfx, mainCamera.transform.position, 0.5f);
				}
			}
			else if (hits.Count != 0)
			{
				// Only play the error sound if we hit something and no model needed any clothing.
				if (mErrorSfx != null)
				{
					GameObject mainCamera = GameFacade.Instance.RetrieveMediator<CameraManagerMediator>().MainCamera;
					AudioSource.PlayClipAtPoint(mErrorSfx, mainCamera.transform.position, 0.2f);
				}
			}

			if (hits.Count > 0)
			{
				mSelection.ClearSelection();
				if (modelToSelect != null && !modelToSelect.Ready)
				{
					mSelection.Select(modelToSelect);
				}
			}

			if( removed > 0 )
			{
				clickHadEffect = true;
			}
			return clickHadEffect;
		}

		public void ClothingSelected(ClothingItem clothingItem)
		{
			mSelection.Select(clothingItem);
		}

		private void MouseUp()
		{
			GameFacade.Instance.RetrieveMediator<FashionGameGui>().MouseUp();
			if (!GameFacade.Instance.RetrieveMediator<FashionGameGui>().OccludesScreenPoint(mMousePosition))
			{
				DropClothing(GameFacade.Instance.RetrieveMediator<FashionCameraMediator>().Camera.ScreenPointToRay(mMousePosition));
			}
		}

		public override void OnRemove()
		{
			base.OnRemove();

			//TODO: this cleans up the workaround for the input bug
			if (mRegisterForMouseDownReceipt != null)
			{
				mRegisterForMouseDownReceipt.Exit();
			}
			if (mRegisterForMouseUpReceipt != null)
			{
				mRegisterForMouseUpReceipt.Exit();
			}
			if (mRegisterForMousePositionReceipt != null)
			{
				mRegisterForMousePositionReceipt.Exit();
			}
			if( mInputManager != null )
			{
				mInputManager.OnRemove();
			}
		}
	}
}
