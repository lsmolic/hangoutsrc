/**  --------------------------------------------------------  *
 *   RoomDistributedObject.cs
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 07/23/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;
using System.Xml;
using Hangout.Shared;

namespace Hangout.Client
{
	public class ClientDistributedGreenScreenRoom : ClientDistributedObject, IClientDistributedRoom
	{
        private string mRoomName = string.Empty;
        private RoomId mRoomId = null;
        private GameObject mImageBackground = null;
		private AccountId mRoomOwnerAccountId = null;

		public bool IsLocalClientOwnedRoom
		{
			get
			{
				AccountId localAccountId = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>().LocalAccountId;
				return localAccountId == mRoomOwnerAccountId;
			}
		}

        public PrivacyLevel PrivacyLevel
        {
            get { throw new NotImplementedException(); }
        }

        public string RoomName
        {
            get { return mRoomName; }
        }

        public RoomId RoomId
        {
            get { return mRoomId; }
        }

        public RoomType RoomType
        {
            get { return RoomType.GreenScreenRoom; }
        }

		//This list holds all the non-entity game objects.  So when a delete gets called
		//the entity manager will erase the entity and this class will clean up the objects
		//not manages by the entity manager.
		private readonly List<GameObject> mGameObjects = new List<GameObject> ();
        public ClientDistributedGreenScreenRoom(SendMessageCallback sendMessage, DistributedObjectId doId, List<object> messageData)
            : base(sendMessage, doId)
        {
            mRoomName = CheckType.TryAssignType<string>(messageData[5]);
            mRoomId = CheckType.TryAssignType<RoomId>(messageData[7]);
			mRoomOwnerAccountId = CheckType.TryAssignType<AccountId>(messageData[8]);

            BuildEntity();

            string roomItemsXmlString = CheckType.TryAssignType<string>(messageData[3]);
            SetupAssetsFromItemsXml(roomItemsXmlString);
        }

        protected override void RegisterMessageActions()
        {
            RegisterMessageAction((int)MessageSubType.ChangeBackground, ReceiveBackgroundChangeFromServer);
        }

        private void ReceiveBackgroundChangeFromServer(Message receivedMessage)
        {
            string newBackgroundXmlString = CheckType.TryAssignType<string>(receivedMessage.Data[0]);
            SetupAssetsFromItemsXml(newBackgroundXmlString);
        }

        private void SetupAssetsFromItemsXml(string roomItemsXmlString)
        {
            XmlDocument roomDna = new XmlDocument();
            roomDna.LoadXml(roomItemsXmlString);

            ClientAssetRepository clientAssetRepository = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
            XmlNodeList itemXmlNodes = roomDna.SelectNodes("Items/Item");
            foreach (XmlNode itemXmlNode in itemXmlNodes)
            {
                XmlNodeList assetXmlNodes = itemXmlNode.SelectNodes("Assets/Asset");
                foreach (XmlNode assetXmlNode in assetXmlNodes)
                {
                    AssetInfo assetInfo = new ClientAssetInfo(assetXmlNode);
                    clientAssetRepository.GetAsset<Asset>(assetInfo, ApplyAssetToType);
                }
            }
        }

        private void ApplyAssetToType(Asset asset)
        {
            if (this.Entity.UnityGameObject == null)
            {
                throw new System.Exception("Error: game object for green screen room entity is not set up.");
            }
            switch (asset.Type)
            {
                case AssetSubType.RoomBackgroundTexture:
                    ImageAsset roomBackgroundTextureAsset = asset as ImageAsset;
                    mImageBackground.renderer.material.mainTexture = roomBackgroundTextureAsset.Texture2D;
                    break;
            }
        }

		//All these game objects will be parented until one main game object "Room".
		public override void BuildEntity()
		{
            GameObject room = new GameObject(DistributedObjectTypes.Room.ToString());
			GameObject ground = BuildGround();
			GameObject lighting = BuildSceneLighting();
            mImageBackground = BuildImageBackground();
            BuildTransitionWalls(room);
			
			ground.transform.parent = room.transform;
			lighting.transform.parent = room.transform;
			mImageBackground.transform.parent = room.transform;
			
			this.Entity = new RoomEntity( room );
		}

		private GameObject BuildGround()
		{
			GameObject colliders = new GameObject("Colliders");
			
			GameObject ground = new GameObject("Ground") as GameObject;
            BoxCollider groundCollider = ground.AddComponent(typeof(BoxCollider)) as BoxCollider;
			ground.gameObject.layer = GameFacade.GROUND_LAYER;
			ground.transform.position = new Vector3 ( 0.0f, -0.475f, 1.0f );
            groundCollider.size = new Vector3(10.0f, 1.0f, 5.0f);
			
			ground.transform.parent = colliders.transform;

			GameObject frontWall = new GameObject("FrontWall");
			frontWall.layer = GameFacade.WALL_LAYER;
			BoxCollider frontWallCollider = frontWall.AddComponent(typeof(BoxCollider)) as BoxCollider;
			frontWallCollider.size = new Vector3(8.0f, 4.0f, 1.0f);
			frontWall.transform.position = new Vector3(0.0f, 2.0f, 3.8f);
			frontWall.transform.parent = colliders.transform;
			
			GameObject backWall = new GameObject("BackWall");
			backWall.layer = GameFacade.WALL_LAYER;
			BoxCollider backWallCollider = backWall.AddComponent(typeof(BoxCollider)) as BoxCollider;
			backWallCollider.size = new Vector3(10.0f, 4.0f, 1.0f);
			backWall.transform.position = new Vector3(0.0f, 2.0f, -1.8f);
			backWall.transform.parent = colliders.transform;
			
			GameObject rightWall = new GameObject("RightWall");
			rightWall.layer = GameFacade.WALL_LAYER;
			BoxCollider rightWallCollider = rightWall.AddComponent(typeof(BoxCollider)) as BoxCollider;
			rightWallCollider.size = new Vector3(3.0f, 4.0f, 6.0f);
			rightWall.transform.position = new Vector3(-4.45f, 2.0f, 1.327f);
			rightWall.transform.eulerAngles = new Vector3(0.0f, 19.7f, 0.0f);
			rightWall.transform.parent = colliders.transform;
			
			GameObject leftWall = new GameObject("LeftWall");
			leftWall.layer = GameFacade.WALL_LAYER;
			BoxCollider leftWallCollider = leftWall.AddComponent(typeof(BoxCollider)) as BoxCollider;
			leftWallCollider.size = new Vector3(3.0f, 4.0f, 6.0f);
			leftWall.transform.position = new Vector3(4.45f, 2.0f, 1.327f);
			leftWall.transform.eulerAngles = new Vector3(0.0f, -19.7f, 0.0f);
			leftWall.transform.parent = colliders.transform;
			
			
			mGameObjects.Add(ground);
			mGameObjects.Add(frontWall);
			mGameObjects.Add(backWall);
			mGameObjects.Add(rightWall);
			mGameObjects.Add(leftWall);
			return colliders;
		}

		private GameObject BuildSceneLighting()
		{
			GameObject sceneLight = new GameObject("Light");
			Light directionalLight = sceneLight.AddComponent( typeof(Light) ) as Light;
			directionalLight.type = LightType.Directional;
			directionalLight.attenuate = false;
			directionalLight.shadows = LightShadows.Soft;
            directionalLight.shadowStrength = 0.3f;
			
			sceneLight.transform.position = Vector3.zero;
			sceneLight.transform.eulerAngles = new Vector3( 60.0f, 202.0f, 235.0f );
			
			directionalLight.color = new Color( 0.92f, 0.92f, 0.78f );
			directionalLight.intensity = 0.25f;
			directionalLight.range = 30.0f;

			mGameObjects.Add(sceneLight);
			
			return sceneLight;
		}

		private GameObject BuildImageBackground() 
		{
			GameObject imageBackground = GameObject.Instantiate( Resources.Load("Room Image Backdrop/Image Background Model") ) as GameObject;
			imageBackground.transform.position = new Vector3( 0.0f, 0.0f, 0.0f );
			imageBackground.transform.rotation = Quaternion.Euler( 0.0f, 0.0f, 0.0f );
			mGameObjects.Add(imageBackground);
			return imageBackground;
		}

        private void BuildTransitionWalls(GameObject room)
        {
            GameObject leftWall = GameObject.Instantiate(Resources.Load("Room Image Backdrop/Room Transition Model")) as GameObject;
            leftWall.transform.localPosition = new Vector3(4.43f, 0.0f, 3.9f);
            leftWall.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            leftWall.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            leftWall.transform.parent = room.transform;
            leftWall.renderer.castShadows = false;
            leftWall.renderer.receiveShadows = false;
            mGameObjects.Add(leftWall);

            GameObject rightWall = GameObject.Instantiate(Resources.Load("Room Image Backdrop/Room Transition Model")) as GameObject;
            rightWall.transform.localPosition = new Vector3(-4.43f, 0.0f, 3.9f);
            rightWall.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            rightWall.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            rightWall.transform.parent = room.transform;
            rightWall.renderer.castShadows = false;
            rightWall.renderer.receiveShadows = false; 
            mGameObjects.Add(rightWall);

            Texture2D wallTex = Resources.Load("Room Image Backdrop/Brick Wall Transition") as Texture2D;
            leftWall.renderer.material.mainTexture = wallTex;
            rightWall.renderer.material.mainTexture = wallTex;

        }

		public override void Dispose()
		{
            foreach (GameObject go in mGameObjects)
            {
                GameObject.Destroy(go);
            }

            mGameObjects.Clear();
            GameObject.Destroy(Entity.UnityGameObject);
		}

        public void UpdateBackgroundImage(ItemId newImageItemId)
        {
            Message updateRoomItemsMessage = new Message();
            List<object> roomItemsData = new List<object>();
            roomItemsData.Add(newImageItemId);
            updateRoomItemsMessage.UpdateObjectMessage(true, false, this.DistributedObjectId, (int)MessageSubType.ChangeBackground, roomItemsData);
            this.SendMessage(updateRoomItemsMessage);
        }
	}	
}
