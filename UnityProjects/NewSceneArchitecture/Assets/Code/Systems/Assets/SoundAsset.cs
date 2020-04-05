/*
 * Pherg made this.
 * 10/06/09
 */

using System;
using System.Collections.Generic;
using System.Text;

using Hangout.Shared;

using UnityEngine;

namespace Hangout.Client
{
	public class SoundAsset : Asset
	{
		public static string UniqueKeyNamespace
		{
			get { return "Path:"; }
		}
		
		private AudioClip mAudioClip;
		public AudioClip AudioClip
		{
			get { return mAudioClip; }
		}

		public override void DestroyUnityResource()
		{
			base.DestroyUnityResource();

			UnityEngine.Object.Destroy(mAudioClip);
		}

		public SoundAsset(AudioClip audioClip, string path)
			: base(AssetSubType.SoundEffect, audioClip.name, path, UniqueKeyNamespace + path)
		{
			if (audioClip == null)
			{
				throw new ArgumentNullException("audioClip");
			}
			mAudioClip = audioClip;
		}
	}
}
