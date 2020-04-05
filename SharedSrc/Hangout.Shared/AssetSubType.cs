/*
   Created by Vilas Tewari on 2009-08-11.

	A Class that encapsulates an asset with information like
	a displayName, Id, etc..
*/
using System;
using System.Xml;

namespace Hangout.Shared
{
	public enum AssetSubType
	{
		NotSet,
		TopMesh,
		BottomMesh,
		FootwearMesh,
		HairMesh,
		TopColor,
		BottomColor,
		FootwearColor,
		SkinColor,
		EyeColor,
		EyebrowColor,
		EyeShadowColor,
		EyeShadowAlpha,
		FaceBlushColor,
		FaceBlushAlpha,
		MouthColor,
		HairColor,
		FaceMesh,
		TopSkinnedMesh,
		BottomSkinnedMesh,
		FootwearSkinnedMesh,
		HairSkinnedMesh,
		UpperBodyMesh,
		LowerBodyMesh,
		FullBodyMesh,
		PropMesh,
		TransitionMesh,
		IdleAnimation,
		WalkAnimation,
		SitAnimation,
		EmoteAnimation,
		PropAnimation,
		EyeShadowTexture,
		EyeTexture,
		EyeLinerTexture,
		EyebrowTexture,
		EarTexture,
		FaceMarkTexture,
		FaceBlushTexture,
		NoseTexture,
		MouthTexture,
		HairTexture,
		TopTexture,
		BottomTexture,
		FootwearTexture,
		UpperBodyTexture,
		LowerBodyTexture,
		FullBodyTexture,
		TransitionTexture,
		PropTexture,
		GlassesTexture,
		BagTexture,
		NecklaceTexture,
		EarringsTexture,
		LeftWristTexture,
		RightWristTexture,
		HandsTexture,
		GlassesSkinnedMesh,
		BagSkinnedMesh,
		EarringsSkinnedMesh,
		NecklaceSkinnedMesh,
		LeftWristSkinnedMesh,
		RightWristSkinnedMesh,
		HandsSkinnedMesh,
		// Animation
		UvAnimation,
		ComplexUvAnimation,
		FaceAnimation,
		RigWalkAnimation,
		RigIdleAnimation,
		RigAnimation,
        //Rooms
        RoomBackgroundTexture,
        // Room thumbnails
        RoomBackgroundThumbnail,
        //Sounds
        SoundEffect,
        //Emoticon
        Emoticon,
	}
}