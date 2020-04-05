/**  --------------------------------------------------------  *
 *   UnityEngineAsset.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/15/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;

using System;
using System.Collections;

using Hangout.Shared;

namespace Hangout.Client
{
	/// <summary>
	/// Assets that aren't part of the database, but still need to be cached in the ClientAssetRepo
	/// </summary>
	public class UnityEngineAsset : Asset
	{
		public static string UniqueKeyNamespace
		{
			get { return "Path:"; }
		}
		private readonly UnityEngine.Object mUnityObject;
		public UnityEngine.Object UnityObject
		{
			get { return mUnityObject; }
		}

		public override void DestroyUnityResource()
		{
			base.DestroyUnityResource();

			UnityEngine.Object.Destroy(mUnityObject);
		}

		public UnityEngineAsset(UnityEngine.Object unityObject, string path)
			: base(AssetSubType.NotSet, unityObject.name, path, UniqueKeyNamespace + path)
		{
			mUnityObject = unityObject;
		}
	}
}