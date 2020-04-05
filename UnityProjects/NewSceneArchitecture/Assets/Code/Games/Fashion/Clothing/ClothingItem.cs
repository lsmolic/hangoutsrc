/**  --------------------------------------------------------  *
 *   ClothingItem.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/02/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using UnityEngine;

using System.Collections.Generic;

using Hangout.Shared;
using Hangout.Client.Gui;

namespace Hangout.Client.FashionGame
{
	public class ClothingItem : IEntity
	{
		private const float BILLBOARD_SIZE = 0.025f;
		private const float BILLBOARD_ASPECT_RATIO = 1.0f;

		public float OriginalBilboardSize
		{
			get { return BILLBOARD_SIZE; }
		}

		private Pair<Texture2D> mNormalTexture;
		public Texture2D NormalTexture
		{
			get { return mNeedsFixin ? mNormalTexture.Second : mNormalTexture.First; }
		}

		private Texture2D mThumbnail;
		public Texture2D Thumbnail
		{
			get { return mThumbnail; }
		}

		private readonly string mItemType;
		public string ItemType
		{
			get { return mItemType; }
		}

		private bool mNeedsFixin = false;
		public bool NeedsFixin
		{
			get { return mNeedsFixin; }
		}

		private ItemId mItemId;
		public ItemId ItemId
		{
			get { return mItemId; }
		}

		public Vector3 RightVector
		{
			get
			{
				Vector3 result = Vector3.right;
				if( mBillboard != null && mBillboard.First.Transform != null )
				{
					result = mBillboard.First.Transform.right;
				}
				return result;
			}
		}

		public Vector3 UpVector
		{
			get
			{
				Vector3 result = Vector3.up;
				if (mBillboard != null && mBillboard.First.Transform != null)
				{
					result = mBillboard.First.Transform.up;
				}
				return result;
			}
		}

		private Pair<Billboard, StyledQuad> mBillboard = null;
		public Vector3 BillboardPosition
		{
			get 
			{
				Vector3 result;
				
				if( mBillboard == null || mBillboard.First.Transform == null )
				{
					result = Vector3.zero;
				}
				else
				{
					result = mBillboard.First.Transform.position; 
				}

				return result;
			}
		}

		private IEnumerable<Asset> mAssets = new List<Asset>();
		public IEnumerable<Asset> Assets
		{
			get { return mAssets; }
		}

		private readonly string mStyleName;
		public string StyleName
		{
			get { return mStyleName; }
		}
		public ClothingItem(ItemId itemId, string itemType, Pair<Texture2D> normal, Texture2D thumbnail, string styleName, IEnumerable<Asset> assets)
		{
			mNeedsFixin = false;

			if (normal == null)
			{
				throw new ArgumentNullException("normal");
			}
			mNormalTexture = normal;

			if (thumbnail == null)
			{
				throw new ArgumentNullException("thumbnail");
			}
			mThumbnail = thumbnail;

			if (assets == null)
			{
				throw new ArgumentNullException("assets");
			}
			mAssets = assets;

			if (String.IsNullOrEmpty(styleName))
			{
				throw new ArgumentException("styleName cannot be empty string or null.");
			}
			mStyleName = styleName;

			if (itemId == null)
			{
				throw new ArgumentNullException("itemId");
			}
			mItemId = itemId;

			if (String.IsNullOrEmpty(itemType))
			{
				throw new ArgumentException("itemType cannot be empty string or null.");
			}
			mItemType = itemType;
		}

		public void Fix()
		{
			mNeedsFixin = false;
		}

		public void MakeNeedFixin()
		{
			mNeedsFixin = true;
		}

		public ClothingItem(ClothingItem copy)
		{
			mItemId = copy.ItemId;
			mNormalTexture = copy.mNormalTexture;
			mThumbnail = copy.mThumbnail;
			mNeedsFixin = false;
			mAssets = copy.Assets;
			mStyleName = copy.StyleName;
		}

		/// <summary>
		/// Builds a Billboard object
		/// </summary>
		public void Show()
		{
			if (mBillboard == null)
			{
				StyledQuad cardBackground = new StyledQuad
				(
					"Quad", 
					GameFacade.Instance.RetrieveMediator<FashionGameGui>().GetNamedStyle(StyleName)
				);

				FashionCameraMediator cameraMediator = GameFacade.Instance.RetrieveMediator<FashionCameraMediator>();

				Billboard billboard = new Billboard();
				billboard.BillboardGameObject.layer = FashionMinigame.CLOTHING_LAYER;
				Material billboardMat = new Material(Shader.Find("GUI/Flat Color"));
				billboardMat.mainTexture = this.NormalTexture;
				billboard.SetMaterial(billboardMat);
				billboard.BillboardToCamera(cameraMediator.Camera);

				// Hard coded values: a bunch of them
				cardBackground.Size = new Vector2(4.0f, 4.0f);
				cardBackground.Transform.parent = billboard.Transform;
				cardBackground.Transform.localPosition = new Vector3(-1.1f, 1.1f, -0.05f);
				cardBackground.Transform.localScale = new Vector3(BILLBOARD_ASPECT_RATIO, 1.0f, 1.0f) * 0.55f;
				cardBackground.Transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);

				mBillboard = new Pair<Billboard, StyledQuad>(billboard, cardBackground);
				SetBillboardSize(BILLBOARD_SIZE);
			}
		}

		public void SetBillboardSize(float size)
		{
			if(mBillboard != null)
			{
				mBillboard.First.Transform.localScale = new Vector3(BILLBOARD_ASPECT_RATIO, 1.0f, 1.0f) * size;
			}
		}

		public void SetBillboardPosition(Vector3 worldSpacePosition)
		{
			if( mBillboard != null && mBillboard.First.Transform != null )
			{
				mBillboard.First.Transform.position = worldSpacePosition;
			}
		}

		/// <summary>
		/// Destroys the Billboard object
		/// </summary>
		public void Hide()
		{
			if (mBillboard != null)
			{
				mBillboard.First.Dispose();
				mBillboard.Second.Dispose();
				mBillboard = null;
			}
		}

		public UnityEngine.GameObject UnityGameObject
		{
			get
			{
				GameObject result = null;
				if (mBillboard != null)
				{
					result = mBillboard.First.BillboardGameObject;
				}
				return result;
			}
		}

		public void Dispose()
		{
			Hide();

			Texture.Destroy(mNormalTexture.First);
			Texture.Destroy(mNormalTexture.Second);
		}
	}
}
