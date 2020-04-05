/**  --------------------------------------------------------  *
 *   ForeignAvatarDistributedObject.cs
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
	public class ForeignAvatarDistributedObject : AvatarDistributedObject
	{        
        private ITask mTelemetrySpeedFilter;
        
        private ForeignAvatarEntity mForeignAvatarEntity;
        
        private float TELEMETRY_SPEED_FILTER_PADDING = 0.1f;

		public ForeignAvatarDistributedObject(SendMessageCallback sendMessage, DistributedObjectId doId, List<object> messageData)
			: base(sendMessage, doId, messageData)
		{
        }

		public override void BuildEntity()
		{
			base.BuildEntity();
			mForeignAvatarEntity = new ForeignAvatarEntity(mAvatar, this.LoadAvatarAndHeadGameObjects().Second);
			this.AvatarEntity = mForeignAvatarEntity;
			this.AvatarEntity.AvatarName = this.AvatarName;
			GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(WaitForAnimationProxyToLoadThenLoadAvatar(this.AvatarDna, mForeignAvatarEntity));
		}

		protected override void RegisterMessageActions()
		{
			base.RegisterMessageActions();
            this.RegisterMessageAction((int)MessageSubType.UpdateDna, RecvDnaUpdate);
            this.RegisterMessageAction((int)MessageSubType.Telemetry, RecvTelemetry);
            this.RegisterMessageAction((int)MessageSubType.SetPosition, SetPosition);
			this.RegisterMessageAction((int)MessageSubType.Emote, ReceiveEmote);
		}

        private void RecvDnaUpdate(Message message)
        {
		   XmlDocument assetsToSetDoc = new XmlDocument();
		   assetsToSetDoc.LoadXml(message.Data[0].ToString());
		
		   List<AssetInfo> assetInfos = new List<AssetInfo>();
		   foreach (XmlNode assetInfoNode in assetsToSetDoc.SelectNodes("Items/Item/Assets/Asset"))
		   {
               AssetInfo assetInfo = new ClientAssetInfo(assetInfoNode);
			   assetInfos.Add(assetInfo);
		   }
		   GameFacade.Instance.RetrieveProxy<ClientAssetRepository>().GetAssets<Asset>(assetInfos, RetrieveAssetsFromClientAssetRepo);
        }

		private void ReceiveEmote(Message message)
		{
			// Parse out the emote info to load from the repo to make sure the emote is loaded before we try to play it.
			XmlDocument emoteDoc = new XmlDocument();
			emoteDoc.LoadXml((string)message.Data[0]);
			XmlNode emoteNode = emoteDoc.SelectSingleNode("Asset");
			ClientAssetInfo emoteInfo = new ClientAssetInfo(emoteNode);
			GameFacade.Instance.RetrieveProxy<ClientAssetRepository>().GetAsset<RigAnimationAsset>(emoteInfo, delegate(RigAnimationAsset rigAnimationAsset)
			{
				if (this.StateMachine.CurrentState is DefaultForeignAvatarState)
				{
					DefaultForeignAvatarState state = (DefaultForeignAvatarState)this.StateMachine.CurrentState;
					state.PlayEmote(rigAnimationAsset.AnimationName.ToString());
				}	
			});
		}

        private void SetPosition(Message message)
        {
            Vector3 position = new Vector3((float)message.Data[0], (float)message.Data[1], (float)message.Data[2]);
            Quaternion rotation = new Quaternion((float)message.Data[3], (float)message.Data[4], (float)message.Data[5], (float)message.Data[6]);

            this.Entity.UnityGameObject.transform.position = position;
            this.Entity.UnityGameObject.transform.rotation = rotation;
        }

		private void RecvTelemetry(Message message)
		{
			try
			{
				Vector3 position = new Vector3((float)message.Data[0], (float)message.Data[1], (float)message.Data[2]);
				Quaternion rotation = new Quaternion((float)message.Data[3], (float)message.Data[4], (float)message.Data[5], (float)message.Data[6]);

				if (mUpdateTelemetry != null )
				{
					mUpdateTelemetry.Exit();
				}
				mUpdateTelemetry = Scheduler.StartCoroutine(DeadReckonTelemetry(position, rotation));
				mUpdateTelemetry.AddOnExitAction(delegate()
				{
					mLastPosition = position;
					mLastRotation = rotation;
					if (mTelemetrySpeedFilter != null)
					{
						mTelemetrySpeedFilter.Exit();
					}
					mTelemetrySpeedFilter = Scheduler.StartCoroutine(TelemtrySpeedFilter());
				});
			}
			catch (NullReferenceException e)
			{
				Console.LogError("Telemetry update data is not formatted correctly\n" + e);
			}
		}
		
		private void RetrieveAssetsFromClientAssetRepo(IEnumerable<Asset> assetsToSet)
		{
			this.AvatarEntity.SetAssetsOverFrames(assetsToSet as List<Asset>);
		}

		private ITask mUpdateTelemetry = null;
		private Vector3 mLastPosition = Vector3.zero;
		private Quaternion mLastRotation = Quaternion.identity;

		private uint mSpeedWindow = 10;
		private List<float> mSpeeds = new List<float>();

		private IEnumerator<IYieldInstruction> DeadReckonTelemetry(Vector3 position, Quaternion rotation)
		{
			float step = 1.0f / (TELEMETRY_UPDATE_RATE * 1.1f);
			Vector3 lastFixedUpdatePosition;


			for (float time = 0.0f; time < (TELEMETRY_UPDATE_RATE * 1.1f); time += Time.fixedDeltaTime)
			{
				float t = time * step;

				lastFixedUpdatePosition = this.Entity.UnityGameObject.transform.position;

				yield return new YieldUntilNextFrame();

				this.Entity.UnityGameObject.transform.position = Vector3.Lerp(mLastPosition, position, t);
				this.Entity.UnityGameObject.transform.rotation = Quaternion.Slerp(mLastRotation, rotation, t);
	
				if( this.StateMachine.CurrentState is DefaultForeignAvatarState )
				{
					DefaultForeignAvatarState state = (DefaultForeignAvatarState)this.StateMachine.CurrentState;
					
					// Add this frame's speed to the list of recent speeds
					float thisFrameSpeed = (lastFixedUpdatePosition - this.Entity.UnityGameObject.transform.position).magnitude / Time.fixedDeltaTime;

					mSpeeds.Add(thisFrameSpeed);
					if( mSpeeds.Count > mSpeedWindow )
					{
						mSpeeds.RemoveAt(0);
					}

					// Average the speeds
					float smoothSpeed = 0.0f;
					foreach( float speed in mSpeeds )
					{
						smoothSpeed += speed;
					}
					smoothSpeed /= (float)mSpeeds.Count;

					state.SetCurrentAvatarSpeed(smoothSpeed);
				}

				lastFixedUpdatePosition = this.Entity.UnityGameObject.transform.position;
			}
		}
		
		private IEnumerator<IYieldInstruction> TelemtrySpeedFilter()
		{
			float time = 0.0f;
			while(time < (TELEMETRY_UPDATE_RATE + TELEMETRY_SPEED_FILTER_PADDING))
			{
				time += Time.deltaTime;
				yield return new YieldUntilNextFrame();
			}
			if (this.StateMachine.CurrentState is DefaultForeignAvatarState)
			{
				DefaultForeignAvatarState state = (DefaultForeignAvatarState)this.StateMachine.CurrentState;
				state.SetCurrentAvatarSpeed(0.0f);
			}
		}

		protected override void AvatarEntityLoadedCallback(AvatarEntity avatar)
		{
			base.AvatarEntityLoadedCallback(avatar);
			
			this.Entity = avatar;

			IState avatarStartState = new DefaultForeignAvatarState(avatar as ForeignAvatarEntity);

			this.StateMachine.CurrentState.AddTransition(avatarStartState);
			this.StateMachine.TransitionToState(avatarStartState);

			// TODO: This next line might not be doing anything.
			mAvatar.SetActiveRecursively(true);

			GameFacade.Instance.RetrieveMediator<AvatarMediator>().RegisterForeignAvatar(this.AvatarEntity, this.DistributedObjectId);
			
			base.AvatarEntityLoadedRunBufferedMessages();
		}

        public override void Dispose()
        {
			base.Dispose();

            if (mUpdateTelemetry != null)
            {
                mUpdateTelemetry.Exit();
            }
			GameFacade.Instance.RetrieveMediator<AvatarMediator>().RemoveForeignAvatarEntity(this.DistributedObjectId);
        }
	}
}