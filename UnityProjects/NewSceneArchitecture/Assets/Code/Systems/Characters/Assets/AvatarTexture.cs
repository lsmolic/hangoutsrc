/*
   Created by Vilas Tewari on 2009-07-24.

	AvatarTexture is a class that maintains a LayeredPalette
	and assigns it to some renderer that belongs to the avatar
	It also maintains a Skin layer at the bottom of the LayeredPalette
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hangout.Shared;
using System;

namespace Hangout.Client
{
	public abstract class AvatarTexture : IDisposable
	{
		// Face Texture
		protected LayeredPalette mTexturePalette;

		// Face Renderer
		private Renderer mRenderer;

		// Dictionary to Map Asset Types to layers
		protected Dictionary<AssetSubType, string> mTypeToLayer;
		protected Dictionary<AssetSubType, string> mTypeToZone;

		public AvatarTexture(Renderer renderer)
		{
			mRenderer = renderer;
			mTypeToLayer = new Dictionary<AssetSubType, string>();
			mTypeToZone = new Dictionary<AssetSubType, string>();

			// Setup Layered Texture Palette
			mTexturePalette = new LayeredPalette(512, 512, false); // don't want an alpha channel

			// Create Skin layer
			mTexturePalette.CreateLayerInAllZones("Skin", new SolidColorPixelSource(512, 512, Color.white), Pixel.PixelBlendMode.Layer);
			mTexturePalette.SetLayerFilter("Skin", new SolidColorFilter());

			// Create Type to Layer Mapping
			mTypeToLayer.Add(AssetSubType.SkinColor, "Skin");

			// Assign texture to face material
			mRenderer.material.mainTexture = mTexturePalette.Texture2D;
		}

		public virtual void Dispose()
		{
			if (mFlattenLayersTask != null)
			{
				mFlattenLayersTask.Exit();
			}
			mTexturePalette.Dispose();
		}

		public void ApplyChanges()
		{
			mTexturePalette.FlattenLayers();
		}

		ITask mFlattenLayersTask = null;

		public ITask ApplyChangesOverFrames(int frames, Hangout.Shared.Action onTextureFlattenComplete)
		{
			IScheduler scheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			mFlattenLayersTask = scheduler.StartCoroutine(mTexturePalette.FlattenLayersOverFramesCoroutine(frames));
			mFlattenLayersTask.AddOnExitAction(onTextureFlattenComplete);
			return mFlattenLayersTask;
		}

		public Rect GetTextureZoneUvArea(string zoneName)
		{
			return mTexturePalette.GetTextureZoneUvArea(zoneName);
		}

		public Rect GetTextureZoneUvArea(AssetSubType assetType)
		{
			if (!mTypeToZone.ContainsKey(assetType))
			{
				throw new Exception("AvatarTexture does not have zone for asset type " + assetType);
			}

			return mTexturePalette.GetTextureZoneUvArea(mTypeToZone[assetType]);
		}

		/// <summary>
		/// Set the PixelSource of a layer. Get the Source from the AssetRepository
		/// </summary>
		public void SetLayerSource(AssetSubType type, PixelSource source)
		{
			if (!mTypeToLayer.ContainsKey(type))
			{
				throw new Exception("AvatarTexture does not have layer for asset type " + type);
			}				

			mTexturePalette.SetLayerPixelSource(mTypeToLayer[type], source);
		}

		/// <summary>
		/// Set Color property on a Layer's ColorFilter
		/// </summary>
		public void SetLayerFilterColor(AssetSubType type, Color color)
		{
			if (!mTypeToLayer.ContainsKey(type))
			{
				throw new Exception("AvatarTexture does not have layer for asset type " + type);
			}
			mTexturePalette.SetLayerFilterColor(mTypeToLayer[type], color);
		}

		/// <summary>
		/// Set Color property on a Layer's ColorFilter
		/// </summary>
		public void SetLayerFilterColorOnly(AssetSubType type, Color color)
		{
			if (!mTypeToLayer.ContainsKey(type))
			{
				throw new Exception("AvatarTexture does not have layer for asset type " + type);
			}

			Color c = mTexturePalette.GetLayerFilterColor(mTypeToLayer[type]);
		    c = new Color(color.r, color.g, color.b, 0.0f);
		    mTexturePalette.SetLayerFilterColor(mTypeToLayer[type], c);
		}

		/// <summary>
		/// Set Color property on a Layer's ColorFilter with alpha
		/// </summary>
		public void SetLayerFilterAlpha(AssetSubType type, Color color)
		{
			if (!mTypeToLayer.ContainsKey(type))
			{
				throw new Exception("AvatarTexture does not have layer for asset type " + type);
			}
			Color c = mTexturePalette.GetLayerFilterColor(mTypeToLayer[type]);
			c = new Color(c.r, c.g, c.b, color.a);
			mTexturePalette.SetLayerFilterColor(mTypeToLayer[type], c);
		}
	}
}