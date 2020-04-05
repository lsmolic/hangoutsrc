/*
 * Created by Pherg on 2009-9-1.
 * 
 *	This class handles turning AssetInfos into Assets which can be
 *  processed by the UnityEngine.
 */


using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;

using PureMVC.Interfaces;
using PureMVC.Patterns;

using Hangout.Shared;
using Hangout.Shared.Messages;

namespace Hangout.Client
{	
	public class ClientAssetRepository : Proxy
	{
		private bool mLoaded = false;
		public bool Loaded
		{
			get { return mLoaded; }
		}
        private readonly Dictionary<MessageSubType, Action<Message>> mMessageActions = new Dictionary<MessageSubType, Action<Message>>();

        //Mood factory for building Mood Asssets.
		private FaceAnimationFactory mFaceAnimationFactory;
		private bool mFaceAnimationFactoryLoaded = false;
		private bool mAnimationProxyLoaded = false;
		
		//Look up table to see if the asset exists is in repo already.
		private Dictionary<string, Asset> mAssetDictionary = new Dictionary<string, Asset>();
		
		//Look up table of assets being downloaded at a path and the callbacks looking for that downloaded object.
		private Dictionary<string, List<Action<UnityEngine.Object>>> mCallbacksForFinishedDownloadAtPath = new Dictionary<string, List<Action<UnityEngine.Object>>>();
		
		private readonly GameObject mAvatarTemplate;

		private void FinishedLoadingFaceAnimationAssets()
		{
			mFaceAnimationFactoryLoaded = true;
			CheckIfFinishedLoading();
		}
		
		// If we ever have more things which need to be loaded before we move out from the GameStateMachine loading State it would
		// be checked here.
		private void CheckIfFinishedLoading()
		{
			if (mFaceAnimationFactoryLoaded && mAnimationProxyLoaded)
			{
				GameFacade.Instance.SendNotification(GameFacade.CLIENT_ASSET_REPOSITORY_LOADED);
				mLoaded = true;
			}
		}
		
		//Since skinned mesh renderers are MonoBehaviors (ugh! bleh! disgusting!) we have to have the repo
		//hold onto a gameobject in the scene to pull SkinnedMeshRenderers off it.
		public ClientAssetRepository()
		{
			mAvatarTemplate = GameObject.Instantiate(Resources.Load("Avatar/Gabriella Templates")) as GameObject;
			// Adding an empty skinned mesh renderer node for accessories to use when being removed.
			GameObject emptyMeshGameObject = new GameObject("EmptySkinnedMeshRenderer");
			SkinnedMeshRenderer smr = emptyMeshGameObject.AddComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
			smr.sharedMesh = new Mesh();
			emptyMeshGameObject.transform.parent = mAvatarTemplate.transform;
			
			
			foreach (Renderer r in mAvatarTemplate.GetComponentsInChildren(typeof(Renderer)))
			{
				r.enabled = false;
			}

            // Setup message callbacks
            mMessageActions.Add(MessageSubType.GetItemsById, HandleGetItemsById);
        }
        
        public void Init()
        {
			mFaceAnimationFactory = new FaceAnimationFactory(FinishedLoadingFaceAnimationAssets);
			AnimationProxy animationProxy = new AnimationProxy(FinishedLoadingAnimationProxy);
			animationProxy.Init();
			GameFacade.Instance.RegisterProxy(animationProxy);
        }
        
        private void FinishedLoadingAnimationProxy()
        {
			mAnimationProxyLoaded = true;
			CheckIfFinishedLoading();
        }

        public void ReceiveMessage(Message receivedMessage)
        {
            MessageUtil.ReceiveMessage<MessageSubType>(receivedMessage, mMessageActions);
        }

        private string UniqueKey(AssetInfo assetInfo)
        {
            string path = assetInfo.Path;
            if (path == null)
            {
				Console.Log("NULL Path");
				Console.Log("Null path assetinfo = " + assetInfo);//, assetId = " + assetInfo.AssetId.Value.ToString());
				Console.Log("Null path asstId = " + assetInfo.AssetId);//, assetId = " + assetInfo.AssetId.Value.ToString());
            }
            if (path == "" || ProtocolUtility.SplitProtocol(path).First == "resources")
            {
                return AssetId.AssetIdNamespace + assetInfo.AssetId.Value.ToString();
            }
            else
            {
                return path;
            }
        }

        public void GetAsset<T>(AssetInfo assetInfo, Action<T> returnAssetCallback) where T : Asset
		{
			GetAsset<T>(assetInfo.AssetData, assetInfo.Type, assetInfo.AssetSubType, UniqueKey(assetInfo), assetInfo.Path, returnAssetCallback);
		}

		/// <summary>
		/// Downloads all the assets defined in assetInfos and returns them to the callback when the entire list is ready
		/// </summary>
		public void GetAssets<T>(IEnumerable<AssetInfo> assetInfos, Action<IEnumerable<T>> returnAssetsCallback) where T : Asset
		{
			GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine
			(
				GetAssetsCoroutine<T>(assetInfos, returnAssetsCallback)
			);
		}

		private IEnumerator<IYieldInstruction> GetAssetsCoroutine<T>
		(
			IEnumerable<AssetInfo> assetInfos,
			Action<IEnumerable<T>> returnAssetsCallback
		) where T: Asset
		{
			List<AssetInfo> incompleteAssets = new List<AssetInfo>(assetInfos); // All the assets that haven't downloaded yet

			int assetCount = incompleteAssets.Count;
			List<T> result = new List<T>();
			foreach (AssetInfo assetInfo in incompleteAssets)
			{
				GetAsset<T>(assetInfo, delegate(T doneAsset)
				{
					result.Add(doneAsset);
					assetCount--;
				});
			}

			yield return new YieldWhile(delegate()
			{
				return assetCount > 0;
			});

			returnAssetsCallback(result);
		}

		//Interface which all classes will use to get an asset.
		//It first checks to see if the asset is in the dictionary.  If so it returns that asset.
		//Otherwise it attempts to load it from disk or download it from the web based on that path.
		private void GetAsset<T>
		(
			XmlNode assetData, 
			System.Type assetType, 
			AssetSubType assetSubType, 
			string key, 
			string path, 
			Action<T> returnAssetCallback
		) where T : Asset
		{
			//Check if the asset is in the dictionary already.
			Asset returnAsset;
			if (mAssetDictionary.TryGetValue(key, out returnAsset))
			{
				if (!(returnAsset.GetType() == assetType))
				{
					throw new ArgumentException("The Asset with key: (" + key + ") expected type " + typeof(T).Name + " actually was " + returnAsset.GetType());

				}
				returnAssetCallback((T)returnAsset);
			}
			else
			{
				//Determine type of asset, build a delegate to handle creating the asset when it's downloaded
				//or instantiated, and return it using the callback supplied.
				if (assetType == typeof(TextureAsset))
				{
					LoadUnityObjectFromPath<T>(path, delegate(UnityEngine.Object loadedObject)
					{
						Texture2D texture = (Texture2D)loadedObject;
						TexturePixelSource texturePixelSource = new TexturePixelSource(texture);
						TextureAsset textureAsset = new TextureAsset(assetSubType, texturePixelSource, texture.name, path, key);
						if (textureAsset == null)
						{
							throw new Exception("Couldn't create TexturePixelSource.");
						}
						returnAsset = textureAsset;
						if (!mAssetDictionary.ContainsKey(key))
						{
							mAssetDictionary.Add(key, returnAsset);
						}
						returnAssetCallback((T)returnAsset);
					});
				}
                else if (assetType == typeof(ImageAsset))
                {
                    LoadUnityObjectFromPath<T>(path, delegate(UnityEngine.Object loadedObject)
                    {
                        Texture2D texture = (Texture2D)loadedObject;
                        ImageAsset imageAsset = new ImageAsset(assetSubType, texture, texture.name, path, key);
                        if (imageAsset == null)
                        {
                            throw new Exception("Couldn't create ImageAsset.");
                        }
                        returnAsset = imageAsset;
                        if (!mAssetDictionary.ContainsKey(key))
                        {
                            mAssetDictionary.Add(key, returnAsset);
                        }
                        returnAssetCallback((T)returnAsset);
                    });
                }
				else if (assetType == typeof(ColorAsset))
				{
					//Pull out color values.
					string hexColorValue = assetData.SelectSingleNode("AssetColor").InnerText;
					string alphaValue = assetData.SelectSingleNode("Alpha").InnerText;
					//Set color values.
					Color color = ColorUtility.HexToColor(hexColorValue);
					color.a = float.Parse(alphaValue);

					ColorAsset colorAsset = new ColorAsset(assetSubType, color, path, path, key);
					if (colorAsset == null)
					{
						throw new Exception("Couldn't create color asset.");
					}
					returnAsset = colorAsset;
					if (!mAssetDictionary.ContainsKey(key))
					{
						mAssetDictionary.Add(key, returnAsset);
					}
					returnAssetCallback((T)returnAsset);
				}
				else if (assetType == typeof(MeshAsset))
				{
					LoadUnityObjectFromPath<T>(path, delegate(UnityEngine.Object loadedObject)
					{
						GameObject avatar = loadedObject as GameObject;
						if (avatar == null)
						{
							throw new Exception("couldn't instantiate gameobject at path: " + path);
						}

						string meshFilterToFind = assetData.SelectSingleNode("MeshName").InnerText;
						foreach (Transform child in avatar.GetComponentsInChildren(typeof(Transform)))
						{
							if (child.name == meshFilterToFind)
							{
								MeshFilter meshFilter = child.GetComponent(typeof(MeshFilter)) as MeshFilter;
								Mesh mesh = meshFilter.mesh;

								MeshAsset meshAsset = new MeshAsset(assetSubType, mesh, path, path, key);

								if (meshAsset == null)
								{
									throw new Exception("Couldn't create MeshAsset.");
								}
								returnAsset = meshAsset;
								if (!mAssetDictionary.ContainsKey(key))
								{
									mAssetDictionary.Add(key, returnAsset);
								}
								returnAssetCallback((T)returnAsset);
								break;
							}
						}
					});
				}
				else if (assetType == typeof(SkinnedMeshAsset))
				{
					LoadUnityObjectFromPath<T>(path, delegate(UnityEngine.Object loadedObject)
					{
						GameObject avatar = loadedObject as GameObject;
						if (avatar == null)
						{
							throw new Exception("couldn't instantiate gameobject at path: " + path);
						}
						string skinnedMeshRendererToFind = assetData.SelectSingleNode("MeshName").InnerText;

						foreach (Transform child in avatar.GetComponentsInChildren(typeof(Transform)))
						{
							if (child.name == skinnedMeshRendererToFind)
							{
								SkinnedMeshRenderer skinnedMeshRenderer = child.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
								SkinnedMeshAsset skinnedMeshAsset = new SkinnedMeshAsset(assetSubType, skinnedMeshRenderer, "", path, key);
								
								if (skinnedMeshRenderer == null)
								{
									throw new Exception("Couldn't create MeshAsset.");
								}
								returnAsset = skinnedMeshAsset;
								if (!mAssetDictionary.ContainsKey(key))
								{
									mAssetDictionary.Add(key, returnAsset);
								}
								returnAssetCallback((T)returnAsset);
								break;
							}
						}
					});
				}
				else if (assetType == typeof(FaceMeshAsset))
				{
					LoadUnityObjectFromPath<T>(path, delegate(UnityEngine.Object loadedObject)
					{
						GameObject avatar = loadedObject as GameObject;
						if (avatar == null)
						{
							throw new Exception("couldn't instantiate gameobject at path: " + path);
						}

						string meshFilterToFind = assetData.SelectSingleNode("MeshName").InnerText;

						foreach (Transform child in avatar.GetComponentsInChildren(typeof(Transform)))
						{
							if (child.name == meshFilterToFind)
							{
								XmlNode meshInfoNode = assetData.SelectSingleNode("MeshData/FaceMeshInfo");
								if (meshInfoNode == null)
								{
									throw new XmlException("Couldn't get face mesh info xml from node: " + assetData.OuterXml);
								}
								MeshFilter meshFilter = child.GetComponent(typeof(MeshFilter)) as MeshFilter;
								meshFilter.gameObject.active = true;
								Mesh mesh = meshFilter.mesh;

								FaceMeshAsset faceMeshAsset = new FaceMeshAsset(assetSubType, mesh, path, path, key, meshInfoNode);

								if (faceMeshAsset == null)
								{
									throw new Exception("Couldn't create MeshAsset.");
								}
								returnAsset = faceMeshAsset;
								if (!mAssetDictionary.ContainsKey(key))
								{
									mAssetDictionary.Add(key, returnAsset);
								}
								returnAssetCallback((T)returnAsset);
								break;
							}
						}
					});
				}
				else if (assetType == typeof(FaceAnimationAsset))
				{
					string moodName = assetData.SelectSingleNode("Name").InnerText;
					FaceAnimationName faceAnimationName = (FaceAnimationName)Enum.Parse(typeof(FaceAnimationName), moodName);
					FaceAnimation faceAnimation = mFaceAnimationFactory.GetFaceAnimation(moodName);
					FaceAnimationAsset faceAnimationAsset = new FaceAnimationAsset(assetSubType, faceAnimation, moodName, path, key, faceAnimationName);

					GameFacade.Instance.RetrieveProxy<AnimationProxy>().AddFaceAnimation(faceAnimationAsset);
					
					returnAsset = faceAnimationAsset;
					if (!mAssetDictionary.ContainsKey(key))
					{
						mAssetDictionary.Add(key, returnAsset);
					}
					returnAssetCallback((T)returnAsset);
				}
				else if (assetType == typeof(RigAnimationAsset))
				{
					LoadUnityObjectFromPath<UnityEngineAsset>(path, delegate(UnityEngine.Object loadedObject)
					{
						GameObject animationGameObject = GameObject.Instantiate(loadedObject as UnityEngine.Object) as GameObject;
						if (animationGameObject == null)
						{
							Console.LogError("Could not cast loadedObject to GameObject from path: " + path);
						}
						Animation animation = animationGameObject.GetComponent(typeof(Animation)) as Animation;
						if (animation == null)
						{
							Console.LogError("No animation component on loadedObject cast as a gameobject loaded from path: " + path);
						}
						
						string nameOfAnimation = assetData.SelectSingleNode("AnimationName").InnerText;
						RigAnimationName animationName;
						if (nameOfAnimation == null)
						{
							Console.LogError("AssetData on rig animation asset info does not contain an RigAnimationName node.");
							animationName = RigAnimationName.None;
						}
						else
						{
							animationName = (RigAnimationName)Enum.Parse(typeof(RigAnimationName), nameOfAnimation);
						}
						
						RigAnimationAsset rigAnimationAsset = new RigAnimationAsset(assetSubType, animation.clip, animation.clip.name, path, key, animationName);
						GameObject.Destroy(animationGameObject);
						
						GameFacade.Instance.RetrieveProxy<AnimationProxy>().AddRigAnimation(rigAnimationAsset);
						
						returnAsset = rigAnimationAsset;
						if (!mAssetDictionary.ContainsKey(key))
						{
							mAssetDictionary.Add(key, returnAsset);
						}
						returnAssetCallback((T)returnAsset);
					});
				}
				else
				{
					throw new ArgumentException("The ClientAssetRepository does not know how to load an asset of type " + typeof(T).Name);
				}
			}
		}

		public void LoadAssetFromPath<T>(string path, Action<T> loadedAssetCallback) where T : Asset
		{
			string uniqueKey = path;

			if( path == null )
			{
				throw new ArgumentNullException("path");
			}

			Asset loadedAsset;
			if( mAssetDictionary.TryGetValue(uniqueKey, out loadedAsset) )
			{
				if (!(loadedAsset is T))
				{
					throw new Exception("Asset loaded from " + path + " expected to be type: " + typeof(T).Name + ", actually was: " + loadedAsset.GetType());
				}
				loadedAssetCallback((T)loadedAsset);
			}
			// This might not ever be called.
 			else if (typeof(T) == typeof(RigAnimationAsset))
 			{
 				LoadUnityObjectFromPath<RigAnimationAsset>(path, delegate(UnityEngine.Object loadedUnityObject)
 				{
 					GameObject gameObject = (GameObject)GameObject.Instantiate(loadedUnityObject);
 					try
 					{
 						Animation animation = gameObject.GetComponent<Animation>();
 						
 						Asset asset = new RigAnimationAsset(AssetSubType.NotSet, animation.clip, path, path, path, RigAnimationName.None);
 
 						if (!mAssetDictionary.ContainsKey(uniqueKey))
 						{
 							mAssetDictionary.Add(uniqueKey, asset);
 						}
 
 						loadedAssetCallback((T)asset);
 					}
 					finally
 					{
 						GameObject.Destroy(gameObject);
 					}
 				});
 			}
			else if (typeof(T) == typeof(UnityEngineAsset))
			{
				LoadUnityObjectFromPath<UnityEngineAsset>(path, delegate(UnityEngine.Object loadedUnityObject)
				{
					Asset asset = new UnityEngineAsset(loadedUnityObject, path);
					if (!mAssetDictionary.ContainsKey(uniqueKey))
					{
						mAssetDictionary.Add(uniqueKey, asset);
					}

					loadedAssetCallback((T)asset);
				});
			}
			else if (typeof(T) == typeof(SoundAsset))
			{
				LoadUnityObjectFromPath<SoundAsset>(path, delegate(UnityEngine.Object loadedUnityObject)
				{
					AudioClip audioClip = loadedUnityObject as AudioClip;
					if (audioClip == null)
					{
						throw new Exception("audioClip could not be cast from UnityEngine.Object");
					}
					Asset asset = new SoundAsset(audioClip, path);
					if (!mAssetDictionary.ContainsKey(uniqueKey))
					{
						mAssetDictionary.Add(uniqueKey, asset);
					}
					loadedAssetCallback((T)asset);
				});
			}
			else if (typeof(T) == typeof(ImageAsset))
			{
				LoadUnityObjectFromPath<ImageAsset>(path, delegate(UnityEngine.Object loadedUnityObject)
				{
					Texture2D texture = loadedUnityObject as Texture2D;
					if (texture == null)
					{
						throw new Exception("texture could not be cast from UnityEngine.Object");
					}
					Asset asset = new ImageAsset(texture, path);
					if (!mAssetDictionary.ContainsKey(uniqueKey))
					{
						mAssetDictionary.Add(uniqueKey, asset);
					}
					loadedAssetCallback((T)asset);
				});
			}
			else if( typeof(T) == typeof(XmlAsset) )
			{
				string resolvedPath = ProtocolUtility.SplitAndResolve(path).Second;
				Console.WriteLine("resolved path for XmlAsset: " + resolvedPath + ", path = " + path);
				GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(DownloadText(resolvedPath, delegate(string wwwData)
				{
					Asset result = new XmlAsset(wwwData, uniqueKey);
					loadedAssetCallback((T)result);
				}));
			}
			else
			{
				throw new Exception("LoadAssetFromPath doesn't support " + typeof(T).Name);
			}
		}

		/// <summary>
		/// Downloads text from a URL without caching it in Unity (browser will still cache normally)
		/// </summary>
		private IEnumerator<IYieldInstruction> DownloadText(string url, Action<string> wwwDataResult)
		{
			SpooledWWW assetWWW = new SpooledWWW(url);
			yield return new YieldForSpooledWWW(assetWWW);
			if (!String.IsNullOrEmpty(assetWWW.result.error))
			{
				Console.LogError("Error while trying to download text: " + assetWWW.result.error + " From path: " + url);
			}
			wwwDataResult(assetWWW.result.data);
		}

		private void LoadUnityObjectFromPath<T>(string path, Action<UnityEngine.Object> loadedAssetCallback) where T : Asset
		{
            
			try
			{
	
				//This hack is here because we don't have an empty texture on the web yet.  We need to figure out how to handle that.
				if (path == "")
				{
					UnityEngine.Object unityEngineObject = Resources.Load("EmptyTexture");
					if (unityEngineObject == null)
					{
						throw new Exception("Could not instantiate Empty Texture.");
					}
	
					loadedAssetCallback(unityEngineObject);
					return;
				}
	
				Pair<string> protocolAndPath = ProtocolUtility.SplitAndResolve(path);
				string loadAssetProtocol = protocolAndPath.First;
				string resourcePath = protocolAndPath.Second;
				if (loadAssetProtocol == "http" || loadAssetProtocol == "file" || loadAssetProtocol == "assets")
				{
					//If the repo is already downloading from the path, the callback
					//is added to this dictionary which gets called when the object at
					//said path is finished downloading.
					if (mCallbacksForFinishedDownloadAtPath.ContainsKey(resourcePath))
					{
						//If we've downloaded the path before we'll have the key but nothing in
						//the list so we have to download it again.
						mCallbacksForFinishedDownloadAtPath[resourcePath].Add(loadedAssetCallback);
					}
					else
					{
						List<Action<UnityEngine.Object>> downloadCompleteCallbacks = new List<Action<UnityEngine.Object>>();
						downloadCompleteCallbacks.Add(loadedAssetCallback);
						mCallbacksForFinishedDownloadAtPath.Add(resourcePath, downloadCompleteCallbacks);
						
						if (typeof(T) == typeof(TextureAsset) || typeof(T) == typeof(ImageAsset) || typeof(T) == typeof(Asset))
						{
							LoadTextureAssetFromWeb(resourcePath, loadedAssetCallback);
						}
						else if (typeof(T) == typeof(SoundAsset))
						{
							LoadSoundAssetFromWeb(resourcePath, loadedAssetCallback);
						}
						else if( typeof(T) == typeof(UnityEngineAsset) || typeof(T) == typeof(RigAnimationAsset) )
						{
							LoadLateBoundUnityObject(resourcePath, loadedAssetCallback);
						}
						else
						{
							throw new Exception("ClientAssetRepository doesn't know how to handle type: " + typeof(T).Name);
						}
					}
				}
				else if (loadAssetProtocol == "resources")
				{
					if (resourcePath == "Avatar/Gabriella Templates")
					{
						loadedAssetCallback(mAvatarTemplate);
						return;
					}
					UnityEngine.Object unityEngineObject = Resources.Load(resourcePath);
					if (unityEngineObject == null)
					{
						throw new Exception("Could not instantiate object from path " + resourcePath + ".");
					}
					
					loadedAssetCallback(unityEngineObject);
					return;
				}
				else
				{
					throw new Exception("ClientAssetRepo does not know how to load an object with protocol: " + loadAssetProtocol);
				}
			}
			catch (System.Exception ex)
			{
                Console.LogError("Exception in LoadUnityObjectFromPath: " + path + " " + ex.ToString());
			}
		}
		
		//Starts a coroutine to download a texture asset using WWW class.
		private void LoadTextureAssetFromWeb(string path, Action<UnityEngine.Object> loadedAssetCallback)
		{
			GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(DownloadTextureAsset(path, loadedAssetCallback));
		}

		//The coroutine to download asset using WWW class and convert to Texture asset.
		private IEnumerator<IYieldInstruction> DownloadTextureAsset(string path, Action<UnityEngine.Object> loadedAssetCallback)
		{
			SpooledWWW assetWWW = new SpooledWWW(path);
			yield return new YieldForSpooledWWW(assetWWW);

			UnityEngine.Object returnObject = null;

			if (String.IsNullOrEmpty(assetWWW.result.error))
			{
				returnObject = assetWWW.result.texture;
			}

			if (returnObject == null)
			{
				Console.LogError("The WWW class could not load a UnityEngine.Object from path (" + path + "): " + assetWWW.result.error);
				returnObject = Resources.Load("GUI/BrokenLink");
			}

			DownloadFinishedExecuteCallbacks(path, loadedAssetCallback, returnObject);
		}

		//Starts a coroutine to download a late bound asset using WWW class.
		private void LoadLateBoundUnityObject(string path, Action<UnityEngine.Object> loadedAssetCallback)
		{
			GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(DownloadLateBoundAsset(path, loadedAssetCallback));
		}

		//The coroutine to download asset using WWW class and convert to a Unity Object.
		private IEnumerator<IYieldInstruction> DownloadLateBoundAsset(string path, Action<UnityEngine.Object> loadedAssetCallback)
		{
			SpooledWWW assetWWW = new SpooledWWW(path);
			yield return new YieldForSpooledWWW(assetWWW);

			if( !String.IsNullOrEmpty(assetWWW.result.error) )
			{
				throw new Exception("Error while trying to download late bound asset: " + assetWWW.result.error);
			}

			UnityEngine.Object returnObject = assetWWW.result.assetBundle.mainAsset;

			if (returnObject == null)
			{
				throw new Exception("The WWW class could not load a UnityEngine.Object from path " + path);
			}

			DownloadFinishedExecuteCallbacks(path, loadedAssetCallback, returnObject);
		}

		//Starts a coroutine to download a sound asset using WWW class.
		private void LoadSoundAssetFromWeb(string path, Action<UnityEngine.Object> loadedAssetCallback)
		{
			GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(DownloadSoundAsset(path, loadedAssetCallback));
		}

		//The coroutine to download asset using WWW class and convert to an AudioClip.
		private IEnumerator<IYieldInstruction> DownloadSoundAsset(string path, Action<UnityEngine.Object> loadedAssetCallback)
		{
			SpooledWWW assetWWW = new SpooledWWW(path);
			yield return new YieldForSpooledWWW(assetWWW);
			if (!String.IsNullOrEmpty(assetWWW.result.error))
			{
				//throw new Exception("Error while trying to download late bound asset: " + assetWWW.result.error);
				Console.LogError("Error while trying to download late bound asset: " + assetWWW.result.error);
			}

			UnityEngine.Object returnObject = assetWWW.result.oggVorbis;
			if (returnObject == null)
			{
				//throw new Exception("The WWW class could not load a UnityEngine.Object from path " + path);
				Console.LogError("The WWW class could not load a UnityEngine.Object from path " + path);
			}

			DownloadFinishedExecuteCallbacks(path, loadedAssetCallback, returnObject);
		}

		//Iterate through the callbacks registered for this path download.
		private void DownloadFinishedExecuteCallbacks(string path, Action<UnityEngine.Object> loadedAssetCallback, UnityEngine.Object returnObject)
		{
			foreach (Action<UnityEngine.Object> downloadedAssetCallback in mCallbacksForFinishedDownloadAtPath[path])
			{
				downloadedAssetCallback(returnObject);
			}

			mCallbacksForFinishedDownloadAtPath.Remove(path);
		}
		
		//This interface is here to add animations to the repo which we are pulling from disk.
		//We haven't late bound animations so we're building them from disk on the AvatarMediator
		//and adding them to the repo explicitly through this function.  This should ONLY be used
		//for animations.
		public void AddAssetToRepository(Asset assetToAdd, string key)
		{
			if (assetToAdd == null)
			{
				throw new ArgumentException("assetToAdd");
			}
			if (key == null)
			{
				throw new ArgumentException("key");
			}
			if (!mAssetDictionary.ContainsKey(key))
			{
				mAssetDictionary.Add(key, assetToAdd);
			}
		}

		//If we know an asset all ready exists in the repo (only case for this right now 
		//are the animation assets) then this function just returns an asset correlated to
		//the asset id.
		public Asset GetAsset(string key)
		{
			Asset result;
			if (mAssetDictionary.ContainsKey(key))
			{
				result = mAssetDictionary[key];
			}
			else
			{
				throw new Exception("Asset with id: " + key.ToString() + " is not cached in the ClientAssetRepository.  This function is only to be used when assets are known to be in Repo.");
			}
			return result;
		}
		
        //////////////////////////////////////////////////////////////////////////
        // Message senders
        //////////////////////////////////////////////////////////////////////////

		//
		// TODO: Make this function able to be accessed by more than one class at a time.
		//
		private Action<XmlDocument> mCallbackForGetItemsById = null;
		public void GetItemToAssetsXmlDoc(IEnumerable<ItemId> itemIds, Action<XmlDocument> assetsCallback)
		{
			if (mCallbackForGetItemsById != null)
			{
				throw new NotImplementedException("This function is not set up to handle multiple calls.  This is a one and done type deal.  A class needs to retrieve the itemsById Xml returned by the server before another call to this function can be made.  This functionality needs to be written.");
			}
			mCallbackForGetItemsById = assetsCallback;
			List<object> data = new List<object>();

			// Wrap the IEnumerable<ItemId> with a List<ItemId> in case the itemIds in isn't a 
			// serializable type (like a yielding function or some other silliness).
			data.Add(new List<ItemId>(itemIds));

			Message getItemsMessage = new Message(MessageType.AssetRepository, (int)MessageSubType.GetItemsById, data);
			GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>().SendMessageToReflector(getItemsMessage);
		}
 
        
        //////////////////////////////////////////////////////////////////////////
        // Message receivers
        //////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Parses the xml sent by the server which contains the asset info of the list items sent by the client
        /// </summary>
        /// <param name="message"></param>
		public void HandleGetItemsById(Message message)
		{
 			XmlDocument itemsDoc = new XmlDocument();
			itemsDoc.LoadXml(message.Data[0].ToString());
 			if (mCallbackForGetItemsById == null)
 			{
 				throw new Exception("GetItemToAssetsXmlDoc is most likely being used incorrectly.  See the not implemented exception error in GetItemToAssetsXmlDoc()");
 			}
			mCallbackForGetItemsById(itemsDoc);
			mCallbackForGetItemsById = null;
		}
		
		public void Dispose()
		{
			GameFacade.Instance.RemoveMediator(typeof(AnimationProxy).Name);
		}
	}
}