using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using PureMVC.Patterns;
using PureMVC.Interfaces;

using Hangout.Shared;
using Hangout.Client.Gui;

namespace Hangout.Client
{
	public abstract class AvatarDistributedObject : ClientDistributedObject
	{
		public const float TELEMETRY_UPDATE_RATE = 0.2f;

		private string mPath;
		protected string Path
		{
			get { return mPath; }
		}
		private readonly AvatarStateMachine mStateMachine = new AvatarStateMachine();
		protected AvatarStateMachine StateMachine
		{
			get { return mStateMachine; }
		}

		protected bool mDisposed = false;

		// Chat bubble variables
		private TextBubble mTextBubble = null;
		private ITask mTextBubbleShowTask = null;
		private float mMinTextBubbleLiveTime = 5.0f;
		private float mMaxTextBubbleLiveTime = 12.0f;
		private float mTextBubbleTimePerChar = 0.1f;

		protected GameObject mAvatar = null;
        protected AvatarEntity AvatarEntity = null;

		private XmlDocument mAvatarDna = new XmlDocument();
		protected XmlDocument AvatarDna
		{
			get { return mAvatarDna; }
		}

        private string mAvatarName = "";
        public string AvatarName
        {
            get { return mAvatarName; }
        }

		private IScheduler mScheduler;
		protected IScheduler Scheduler
		{
			get { return mScheduler; }
		}

		//Store messages when entity is not loaded, then fire them off.
		private bool mAvatarLoaded = false;
		private List<Message> mMessagesStoredDuringLoading = new List<Message>();

		public AvatarDistributedObject(SendMessageCallback sendMessage, DistributedObjectId doId, List<object> messageData)
			: base(sendMessage, doId)
		{
			mScheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			mPath = (string)messageData[1];

			string xmlString = (string)messageData[3];
			mAvatarDna.LoadXml(xmlString);

            mAvatarName = (string)messageData[4];

			if (this.AvatarDna == null)
			{
				throw new Exception("this.AvatarDna is null when trying to Build Local Avatar Entity.");
			}
			
			BuildEntity();
		}

		protected IEnumerator<IYieldInstruction> WaitForAnimationProxyToLoadThenLoadAvatar(XmlDocument assetItemsDoc, AvatarEntity avatarEntity)
		{
			yield return new YieldWhile(delegate()
			{
				return !GameFacade.Instance.RetrieveProxy<ClientAssetRepository>().Loaded;
			});
			LoadAssetsCoroutine(assetItemsDoc, avatarEntity);
		}

		protected virtual void LoadAssetsCoroutine(XmlDocument assetItemsDoc, AvatarEntity avatarEntity)
		{
			ClientAssetRepository clientAssetRepository = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			if (clientAssetRepository == null)
			{
				throw new Exception("Cannot LoadAssets until the ClientAssetRepository is set up (it's set up during the APPLICATION_INIT notification.");
			}

			List<AssetInfo> requestedAssetInfos = new List<AssetInfo>(ParseAssetInfos(assetItemsDoc));

			clientAssetRepository.GetAssets<Asset>(requestedAssetInfos, delegate(IEnumerable<Asset> assets)
			{
				avatarEntity.SetAssetsWithCallback(requestedAssetInfos as IEnumerable<AssetInfo>, AvatarEntityLoadedCallback);
			});
		}

		protected virtual void AvatarEntityLoadedCallback(AvatarEntity avatar)
		{
			// TODO: Make these paths less hard coded.
			//CharacterMediator.AddAnimationClipToRig(avatar.UnityGameObject, "Avatar/F_walk_normal_loop");
			//CharacterMediator.AddAnimationClipToRig(avatar.UnityGameObject, "Avatar/F_idle_normal_loop");
		}

		protected List<AssetInfo> ParseAssetInfos(XmlDocument itemsDoc)
		{
			List<AssetInfo> assetInfoList = new List<AssetInfo>();
			
			foreach (XmlNode itemNode in itemsDoc.SelectNodes("Items/Item"))
			{
				foreach (XmlNode assetNode in itemNode.SelectNodes("Assets/Asset"))
				{
                    assetInfoList.Add(new ClientAssetInfo(assetNode));
				}
			}
			return assetInfoList;
		}

		public override void ProcessMessage(Message message)
		{
			if (!mAvatarLoaded)
			{
				mMessagesStoredDuringLoading.Add(message);
				return;
			}
			base.ProcessMessage(message);
		}

		protected void AvatarEntityLoadedRunBufferedMessages()
		{
			mAvatarLoaded = true;
			foreach (Message storedMessage in mMessagesStoredDuringLoading)
			{
				this.ProcessMessage(storedMessage);
			}
		}

		protected override void RegisterMessageActions()
		{
			RegisterMessageAction((int)MessageSubType.Chat, RecvChat);
			RegisterMessageAction((int)MessageSubType.Emoticon, RecvEmoticon);
        }

		private void RecvChat(Message message)
		{
			if (mDisposed)
			{
				throw new Exception("Trying to RecvChat after the AvatarDistributedObject has been disposed.");
			}
			string chatText = (string)message.Data[0];

			object[] args = { this, chatText };
			GameFacade.Instance.SendNotification(GameFacade.RECV_CHAT, args);
		}

		private void RecvEmoticon(Message message)
		{
			if (mDisposed)
			{
				throw new Exception("Trying to RecvEmoticon after the AvatarDistributedObject has been disposed.");
			}
			string emoticonPath = (string)message.Data[0];

			object[] args = { this, emoticonPath };
			GameFacade.Instance.SendNotification(GameFacade.RECV_EMOTICON, args);
		}

		public void ShowChat(string chatText, Camera camera, IGuiManager guiManager)
		{
			if (mDisposed)
			{
				throw new Exception("Trying to ShowChat after the AvatarDistributedObject has been disposed.");
			}
			if (mTextBubbleShowTask != null)
			{
				mTextBubbleShowTask.Exit();
			}
            // Hide nametag while chat is showing
			AvatarEntity.Nametag.Visible = false;
            
            IScheduler scheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			mTextBubbleShowTask = scheduler.StartCoroutine(SpawnTextBubble(chatText, camera, guiManager));
			mTextBubbleShowTask.AddOnExitAction(CleanupTextBubble);
		}

		public void ShowChat(Texture2D texture, Camera camera, IGuiManager guiManager)
		{
			if (mDisposed)
			{
				throw new Exception("Trying to ShowChat after the AvatarDistributedObject has been disposed.");
			}
			if (mTextBubbleShowTask != null)
			{
				mTextBubbleShowTask.Exit();
			}
			// Hide nametag while chat is showing
			AvatarEntity.Nametag.Visible = false;

			IScheduler scheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			mTextBubbleShowTask = scheduler.StartCoroutine(SpawnTextBubble(texture, camera, guiManager));
			mTextBubbleShowTask.AddOnExitAction(CleanupTextBubble);
		}

		private void CleanupTextBubble()
		{
			if (mTextBubble != null)
			{
				mTextBubble.Dispose();
				mTextBubble = null;
			}
            // Reshow nametag
			AvatarEntity.Nametag.Visible = true;
		}

		private IEnumerator<IYieldInstruction> SpawnTextBubble(string chatMessage, Camera camera, IGuiManager guiManager)
		{
			mTextBubble = new TextBubble(guiManager, chatMessage, this.Entity.UnityGameObject.transform, camera);
			// Add some time for long chat messages
			float displayTime = Math.Min(mMaxTextBubbleLiveTime, mMinTextBubbleLiveTime + (chatMessage.Length * mTextBubbleTimePerChar));
			yield return new YieldForSeconds(displayTime);
		}

		private IEnumerator<IYieldInstruction> SpawnTextBubble(Texture2D chatImage, Camera camera, IGuiManager guiManager)
		{
			mTextBubble = new TextBubble(guiManager, chatImage, this.Entity.UnityGameObject.transform, camera);
			// Add some time for long chat messages
			float displayTime = mMaxTextBubbleLiveTime;
			yield return new YieldForSeconds(displayTime);
		}

		public override void BuildEntity()
		{
			mAvatar = GameObject.Instantiate(Resources.Load(Path)) as GameObject;
			if (mAvatar == null)
			{
				throw new Exception("Avatar gameobject could not be instantiated at path: " + Path);
			}
		}

		protected Pair<GameObject> LoadAvatarAndHeadGameObjects()
		{
			// TODO: Hard coded value
			GameObject head = GameObjectUtility.GetNamedChildRecursive("Head", mAvatar);
			if (head == null)
			{
				throw new Exception("Cannot find GameObject named 'head' in the hierarchy under avatar GameObject");
			}
			return new Pair<GameObject>(mAvatar, head);
		}

		public override void Dispose()
		{
			mDisposed = true;
			//Tasks in Scheduler
			foreach (ITask task in Coroutines)
			{
				task.Exit();
			}
			//Gui Tasks
			CleanupTextBubble();
			if (mTextBubbleShowTask != null)
			{
				mTextBubbleShowTask.Exit();
			}
			mStateMachine.Dispose();

			//this disposes the entity if one exists
			base.Dispose();

			//if the entity was never created, then there's nothing around to destroy the mAvatar game object
			// we need to check for this and delete it if the reference is still around
			if(mAvatar != null)
			{
				GameObject.Destroy(mAvatar);
				mAvatar = null;
			}
		}
	}
}