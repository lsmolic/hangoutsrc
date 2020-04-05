/*
   Created by Vilas Tewari on 2009-07-24.

	AvatarBodyTexture is an AvatarTexture that maintains a LayeredPalette
	object and assigns it to the avatars body mesh
*/

using UnityEngine;
using System.Collections;

using Hangout.Shared;

namespace Hangout.Client
{
	public class AvatarBodyTexture : AvatarTexture
	{
		public AvatarBodyTexture( Renderer renderer ) : base( renderer )
		{
			// Create Zones
			mTexturePalette.CreateTextureZone( "Bottom", 0, 0, 256, 256 );
			mTexturePalette.CreateTextureZone( "Top", 0, 256, 256, 256 );
			mTexturePalette.CreateTextureZone( "Footwear", 256, 0, 128, 128 );
			mTexturePalette.CreateTextureZone( "Glasses", 384, 384, 128, 128 );
			mTexturePalette.CreateTextureZone( "Necklace", 384, 320, 128, 64 );
			mTexturePalette.CreateTextureZone( "Earrings", 384, 256, 128, 64 );
			mTexturePalette.CreateTextureZone( "Hands", 256, 128, 128, 128 );
			mTexturePalette.CreateTextureZone( "LeftWrist", 384, 192, 128, 64 );
			mTexturePalette.CreateTextureZone( "RightWrist", 384, 128, 128, 64 );
			mTexturePalette.CreateTextureZone( "Bag", 384, 0, 128, 128 );
			mTexturePalette.CreateTextureZone( "Hair", 256, 256, 128, 256 );
			
			
			//	Create layers and add them to the appropriate zones
			mTexturePalette.CreateLayerInZone( "Hands", "Hands", Pixel.PixelBlendMode.Layer );
			
			// Hair Layers Setup
			mTexturePalette.CreateLayerInZone( "Hair", "Hair", Pixel.PixelBlendMode.Layer );
			mTexturePalette.SetLayerFilter( "Hair", new SimpleColorFilter() );
			
			// Top Layers Setup
			mTexturePalette.CreateLayerInZone( "Top", "Top", Pixel.PixelBlendMode.Layer );
			
			// Bottom Layers Setup
			mTexturePalette.CreateLayerInZone( "Bottom", "Bottom", Pixel.PixelBlendMode.Layer );
			
			// Footwear Layers Setup
			mTexturePalette.CreateLayerInZone( "Footwear", "Footwear", Pixel.PixelBlendMode.Layer );
			
			// Accessories Layers
			mTexturePalette.CreateLayerInZone( "Glasses", "Glasses", Pixel.PixelBlendMode.Layer );
			mTexturePalette.CreateLayerInZone( "Necklace", "Necklace", Pixel.PixelBlendMode.Layer );
			mTexturePalette.CreateLayerInZone( "Earrings", "Earrings", Pixel.PixelBlendMode.Layer );
			mTexturePalette.CreateLayerInZone( "LeftWrist", "LeftWrist", Pixel.PixelBlendMode.Layer );
			mTexturePalette.CreateLayerInZone( "RightWrist", "RightWrist", Pixel.PixelBlendMode.Layer );
			mTexturePalette.CreateLayerInZone( "Bag", "Bag", Pixel.PixelBlendMode.Layer );
			
			
			// Create Type to Layer Mapping
			mTypeToLayer.Add(AssetSubType.HairTexture, "Hair");
			mTypeToLayer.Add(AssetSubType.HairColor, "Hair");

			mTypeToLayer.Add(AssetSubType.TopTexture, "Top");
			mTypeToLayer.Add(AssetSubType.BottomTexture, "Bottom");
			mTypeToLayer.Add(AssetSubType.FootwearTexture, "Footwear");
			mTypeToLayer.Add(AssetSubType.HandsTexture, "Hands");
			mTypeToLayer.Add(AssetSubType.GlassesTexture, "Glasses");
			mTypeToLayer.Add(AssetSubType.BagTexture, "Bag");
			mTypeToLayer.Add(AssetSubType.NecklaceTexture, "Necklace");
			mTypeToLayer.Add(AssetSubType.EarringsTexture, "Earrings");
			mTypeToLayer.Add(AssetSubType.LeftWristTexture, "LeftWrist");
			mTypeToLayer.Add(AssetSubType.RightWristTexture, "RightWrist");
			mTypeToLayer.Add(AssetSubType.FootwearColor, "Footwear");
			mTypeToLayer.Add(AssetSubType.TopColor, "Top");
			mTypeToLayer.Add(AssetSubType.BottomColor, "Bottom");
			
			// Create Type to Zone Mapping for Uv atlasing
			mTypeToZone.Add(AssetSubType.TopSkinnedMesh, "Top");
			mTypeToZone.Add(AssetSubType.HairSkinnedMesh, "Hair");
			mTypeToZone.Add(AssetSubType.BottomSkinnedMesh, "Bottom");
			mTypeToZone.Add(AssetSubType.FootwearSkinnedMesh, "Footwear");
			mTypeToZone.Add(AssetSubType.HandsSkinnedMesh, "Hands");
			mTypeToZone.Add(AssetSubType.GlassesSkinnedMesh, "Glasses");
			mTypeToZone.Add(AssetSubType.BagSkinnedMesh, "Bag");
			mTypeToZone.Add(AssetSubType.EarringsSkinnedMesh, "Earrings");
			mTypeToZone.Add(AssetSubType.NecklaceSkinnedMesh, "Necklace");
			mTypeToZone.Add(AssetSubType.LeftWristSkinnedMesh, "LeftWrist");
			mTypeToZone.Add(AssetSubType.RightWristSkinnedMesh, "RightWrist");
		}
	}
}