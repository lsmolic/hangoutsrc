/*
   Created by Vilas Tewari on 2009-07-24.

	AvatarFaceTexture is an AvatarTexture that maintains a LayeredPalette
	object and assigns it to the avatars face mesh
*/

using UnityEngine;
using System.Collections;
using Hangout.Shared;

namespace Hangout.Client
{
	public class AvatarFaceTexture : AvatarTexture
	{
		// Use this for initialization
		public AvatarFaceTexture(Renderer renderer)
			: base(renderer)
		{
			// Create Zones
			mTexturePalette.CreateTextureZone("Eye", 0, 0, 255, 255);
			mTexturePalette.CreateTextureZone("Eyebrow", 0, 256, 255, 511);
			mTexturePalette.CreateTextureZone("Mouth", 256, 0, 511, 255);
			mTexturePalette.CreateTextureZone("Face", 256, 256, 511, 511);

			//	Create layers and add them to the appropriate zones
			
			// Eye Layers Setup
			mTexturePalette.CreateLayerInZone("EyeShadow", "Eye", Pixel.PixelBlendMode.Layer);
			mTexturePalette.SetLayerFilter("EyeShadow", new HSVShiftFilter());

			mTexturePalette.CreateLayerInZone("EyeLiner", "Eye", Pixel.PixelBlendMode.Layer);
			mTexturePalette.CreateLayerInZone("Eye", "Eye", Pixel.PixelBlendMode.Layer);
			mTexturePalette.SetLayerFilter("Eye", new WhitePreservingColorFilter());

			// Face Layers Setup
			mTexturePalette.CreateLayerInZone("Nose", "Face", Pixel.PixelBlendMode.Multiply);
			mTexturePalette.MoveLayerRelative("Nose", 98, 140);

			mTexturePalette.CreateLayerInZone("Ear", "Face", Pixel.PixelBlendMode.Multiply);
			mTexturePalette.MoveLayerRelative("Ear", 192, 0);

			mTexturePalette.CreateLayerInZone("FaceMark", "Face", Pixel.PixelBlendMode.Multiply);
			mTexturePalette.CreateLayerInZone("FaceBlush", "Face", Pixel.PixelBlendMode.Layer);
			mTexturePalette.SetLayerFilter("FaceBlush", new SolidColorFilter());

			mTexturePalette.CreateLayerInZone("FaceAccesories", "Face", Pixel.PixelBlendMode.Layer);

			// Eyebrow Layers Setup
			mTexturePalette.CreateLayerInZone("Eyebrow", "Eyebrow", Pixel.PixelBlendMode.Layer);
			mTexturePalette.SetLayerFilter("Eyebrow", new SolidColorFilter());

			// Mouth Layers Setup
			mTexturePalette.CreateLayerInZone("Mouth", "Mouth", Pixel.PixelBlendMode.Layer);
			mTexturePalette.SetLayerFilter("Mouth", new WhitePreservingColorFilter());

			mTexturePalette.CreateLayerInZone("MouthHair", "Mouth", Pixel.PixelBlendMode.Layer);
			mTexturePalette.CreateLayerInZone("MouthAccesories", "Mouth", Pixel.PixelBlendMode.Layer);

			// Create Type to Layer Mapping
			mTypeToLayer.Add(AssetSubType.EyeShadowTexture, "EyeShadow");
			mTypeToLayer.Add(AssetSubType.EyeShadowColor, "EyeShadow");
			mTypeToLayer.Add(AssetSubType.EyeShadowAlpha, "EyeShadow");

			mTypeToLayer.Add(AssetSubType.EyeLinerTexture, "EyeLiner");

			mTypeToLayer.Add(AssetSubType.EyeTexture, "Eye");
			mTypeToLayer.Add(AssetSubType.EyeColor, "Eye");

			mTypeToLayer.Add(AssetSubType.NoseTexture, "Nose");
			mTypeToLayer.Add(AssetSubType.EarTexture, "Ear");

			mTypeToLayer.Add(AssetSubType.FaceMarkTexture, "FaceMark");
			mTypeToLayer.Add(AssetSubType.FaceBlushTexture, "FaceBlush");
			mTypeToLayer.Add(AssetSubType.FaceBlushColor, "FaceBlush");
			mTypeToLayer.Add(AssetSubType.FaceBlushAlpha, "FaceBlush");

			mTypeToLayer.Add(AssetSubType.EyebrowTexture, "Eyebrow");
			mTypeToLayer.Add(AssetSubType.EyebrowColor, "Eyebrow");

			mTypeToLayer.Add(AssetSubType.MouthTexture, "Mouth");
			mTypeToLayer.Add(AssetSubType.MouthColor, "Mouth");
		}
	}
}