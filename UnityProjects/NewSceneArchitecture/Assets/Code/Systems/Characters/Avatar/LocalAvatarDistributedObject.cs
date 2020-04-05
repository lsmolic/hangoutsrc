/**  --------------------------------------------------------  *
 *   LocalAvatarDistributedObejct.cs
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 07/22/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client
{
	public class LocalAvatarDistributedObject : AvatarDistributedObject
	{
		private Vector3 mLastPosition;
		private Quaternion mLastRotation;

		private LocalAvatarStateMachine mLocalAvatarStateMachine;
		
		private LocalAvatarEntity mLocalAvatarEntity;
		
        public LocalAvatarDistributedObject(SendMessageCallback sendMessage, DistributedObjectId doId, List<object> messageData)
			: base(sendMessage, doId, messageData)
		{
		}

		public override void BuildEntity()
		{
			base.BuildEntity();
			mLocalAvatarEntity = new LocalAvatarEntity(mAvatar, this.LoadAvatarAndHeadGameObjects().Second);
			this.AvatarEntity = mLocalAvatarEntity;
			this.AvatarEntity.AvatarName = this.AvatarName;
			
			GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(WaitForAnimationProxyToLoadThenLoadAvatar(this.AvatarDna, mLocalAvatarEntity));
		}

		protected override void AvatarEntityLoadedCallback(AvatarEntity avatar)
		{
			Console.WriteLine("LocalAvatarEntityLoadedCallback");
			
			base.AvatarEntityLoadedCallback(avatar);

			mLocalAvatarEntity = avatar as LocalAvatarEntity;
			if (mLocalAvatarEntity == null)
			{
				throw new Exception("Could not cast avatarEntity as LocalAvatarEntity");
			}
			
			PhysicMaterial physMaterial = new PhysicMaterial("Main Character Physics Material");
			physMaterial.bouncyness = 0.0f;
			physMaterial.dynamicFriction = 0.0f;
			physMaterial.dynamicFriction2 = 0.0f;
			physMaterial.frictionDirection = Vector3.zero;
			physMaterial.frictionDirection2 = Vector3.zero;
			physMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
			physMaterial.staticFriction = 0.0f;
			physMaterial.staticFriction2 = 0.0f;

			CapsuleCollider capsuleCollider = mAvatar.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider;
			capsuleCollider.material = physMaterial;
			
			Rigidbody rigidbody = mAvatar.GetComponent(typeof(Rigidbody)) as Rigidbody;
			if (rigidbody == null)
			{
				rigidbody = mAvatar.AddComponent(typeof(Rigidbody)) as Rigidbody;
			}
			rigidbody.freezeRotation = true;
			rigidbody.detectCollisions = true;
			rigidbody.angularDrag = 0.0f;

			Component[] renderers = mAvatar.GetComponentsInChildren(typeof(Renderer));
			foreach (Component c in renderers)
			{
				Renderer r = c as Renderer;
				for (int x = 0; x < r.materials.Length; ++x)
				{
					Texture2D tex = r.materials[x].mainTexture as Texture2D;
					Shader shader = Shader.Find("Avatar/Saturation Shader");
					r.materials[x] = new Material(shader);
					r.materials[x].mainTexture = tex;
				}
			}

			XmlDocument avatarProperties = XmlUtility.LoadXmlDocument("resources://XmlDocuments/FirstMilestone/PrototypeAvatarProperties");

			try
			{
				// Randomize the starting position in X and Z
				List<float> xPositions = new List<float>();
				List<float> zPositions = new List<float>();
				for (int i = -2; i <= 2; ++i)
				{
					xPositions.Add(0.5f * i);
				}
				for (int j = 0; j <= 6; ++j)
				{
					zPositions.Add(0.5f * j);
				}
				int xIndex = new System.Random().Next(xPositions.Count);
				int zIndex = new System.Random().Next(zPositions.Count);
				float xPosition = xPositions[xIndex];
				float zPosition = zPositions[zIndex];
				float yPosition = 0.02221f;

				mAvatar.transform.position = new Vector3(xPosition, yPosition, zPosition);
				
				XmlNode colliderNode = avatarProperties.SelectSingleNode("/Avatar/collider");

				capsuleCollider.center = new Vector3(float.Parse(colliderNode.SelectSingleNode("center/x").InnerText),
													  float.Parse(colliderNode.SelectSingleNode("center/y").InnerText),
													  float.Parse(colliderNode.SelectSingleNode("center/z").InnerText));

				capsuleCollider.height = float.Parse(colliderNode.SelectSingleNode("height").InnerText);
				capsuleCollider.radius = float.Parse(colliderNode.SelectSingleNode("radius").InnerText);
			}
			catch (NullReferenceException e)
			{
				throw new Exception("Error loading XML from 'resources://XmlDocuments/FirstMilestone/PrototypeAvatarProperties'", e);
			}
			catch (FormatException e)
			{
				throw new Exception("Error loading XML from 'resources://XmlDocuments/FirstMilestone/PrototypeAvatarProperties'", e);
			}
			catch (XmlException e)
			{
				throw new Exception("Error loading XML from 'resources://XmlDocuments/FirstMilestone/PrototypeAvatarProperties'", e);
			}

			this.Entity = mLocalAvatarEntity;

			this.Coroutines.Add(Scheduler.StartCoroutine(TelemetryBroadcast()));

			GameFacade.Instance.RetrieveMediator<AvatarMediator>().AddLocalAvatarEntity(mLocalAvatarEntity, this.DistributedObjectId);
			
			mLocalAvatarStateMachine = new LocalAvatarStateMachine(mLocalAvatarEntity);
			GameFacade.Instance.RegisterMediator(mLocalAvatarStateMachine);

			GameFacade.Instance.SendNotification(GameFacade.LOCAL_AVATAR_LOADED);
			
			base.AvatarEntityLoadedRunBufferedMessages();
		}
		
		protected override void LoadAssetsCoroutine(XmlDocument assetItemsDoc, AvatarEntity avatarEntity)
		{
			LocalAvatarEntity localAvatarEntity = avatarEntity as LocalAvatarEntity;
			ClientAssetRepository clientAssetRepository = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			if (clientAssetRepository == null)
			{
				throw new Exception("Cannot LoadAssets until the ClientAssetRepository is set up (it's set up during the APPLICATION_INIT notification.");
			}

			List<AssetInfo> requestedAssetInfos = new List<AssetInfo>(ParseAssetInfos(assetItemsDoc));
			
			clientAssetRepository.GetAssets<Asset>(requestedAssetInfos, delegate(IEnumerable<Asset> assets)
			{
				localAvatarEntity.SetDnaAndBuildAvatar(requestedAssetInfos as IEnumerable<AssetInfo>, AvatarEntityLoadedCallback);
			});
		}

		private IEnumerator<IYieldInstruction> TelemetryBroadcast()
		{
			while (true)
			{
				if (mLastPosition != this.Entity.UnityGameObject.transform.position || mLastRotation != this.Entity.UnityGameObject.transform.rotation)
				{
					mLastPosition = this.Entity.UnityGameObject.transform.position;
					mLastRotation = this.Entity.UnityGameObject.transform.rotation;
					SendTelemetryUpdate();
				}
				yield return new YieldForSeconds(TELEMETRY_UPDATE_RATE);
			}
		}

		private void SendTelemetryUpdate()
		{
			Message telemetryUpdateMessage = new Message();
			List<object> data = new List<object>();

			data.Add(this.Entity.UnityGameObject.transform.position.x);
			data.Add(this.Entity.UnityGameObject.transform.position.y);
			data.Add(this.Entity.UnityGameObject.transform.position.z);

			data.Add(this.Entity.UnityGameObject.transform.rotation.x);
			data.Add(this.Entity.UnityGameObject.transform.rotation.y);
			data.Add(this.Entity.UnityGameObject.transform.rotation.z);
			data.Add(this.Entity.UnityGameObject.transform.rotation.w);

			telemetryUpdateMessage.UpdateObjectMessage(true, false, mDistributedObjectId, (int)MessageSubType.Telemetry, data);
			this.SendMessage(telemetryUpdateMessage);
		}

		public void SendEmoteUpdate(string emoteName)
		{
			Message emoteUpdateMessage = new Message();

			List<object> data = new List<object>();
			RigAnimationName emoteRigAnimationName = (RigAnimationName)Enum.Parse(typeof(RigAnimationName), emoteName);
			string emoteAssetInfoNode = GameFacade.Instance.RetrieveProxy<AnimationProxy>().EmoteToAssetInfoLookUpTable[emoteRigAnimationName].AssetNode.OuterXml;
			
			data.Add(emoteAssetInfoNode);

			emoteUpdateMessage.UpdateObjectMessage(true, false, mDistributedObjectId, (int)MessageSubType.Emote, data);
			this.SendMessage(emoteUpdateMessage);
		}

        public void SendChat(string chatText)
        {
            if (mDisposed)
            {
                throw new Exception("Trying to SendChat after the AvatarDistributedObject has been disposed.");
            }
            Message updateMessage = new Message();
            List<object> data = new List<object>();
            data.Add(chatText);
            updateMessage.UpdateObjectMessage(true, false, mDistributedObjectId, (int)MessageSubType.Chat, data);
            this.SendMessage(updateMessage);
        }

		public void SendEmoticon(string emoticonPath)
		{
			if (mDisposed)
			{
				throw new Exception("Trying to SendEmoticon after the AvatarDistributedObject has been disposed.");
			}
			Message updateMessage = new Message();
			List<object> data = new List<object>();
			data.Add(emoticonPath);
			updateMessage.UpdateObjectMessage(true, false, mDistributedObjectId, (int)MessageSubType.Emoticon, data);
			this.SendMessage(updateMessage);
		}
        
        public void SaveDna()
        {
			SendDnaUpdate(mLocalAvatarEntity.GetDnaXml());
        }

        /// <summary>
        /// Send DNA to server to broadcast and save out to db
        /// </summary>
        /// <param name="newDna">expected format: <AvatarDna AvatarName="white"><Items>162,139,140,143,144,145,164,165,148,149,150,151,152,153,154,155,156,157,158,159</Items></AvatarDna ></param>
        private void SendDnaUpdate(XmlDocument newDna)
        {
            Message dnaUpdateMessage = new Message();

            List<object> data = new List<object>();
            data.Add(newDna.OuterXml);

            dnaUpdateMessage.UpdateObjectMessage(true, true, mDistributedObjectId, (int)MessageSubType.UpdateDna, data);
            this.SendMessage(dnaUpdateMessage);
        }

		public override void Dispose()
		{
			mLocalAvatarStateMachine.Dispose();
			base.Dispose();
			GameFacade.Instance.RetrieveMediator<AvatarMediator>().RemoveLocalAvatarEntity(this.DistributedObjectId);
			GameFacade.Instance.RemoveMediator(typeof(LocalAvatarStateMachine).Name);
		}
	}
}
