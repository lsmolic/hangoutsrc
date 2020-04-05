/**  --------------------------------------------------------  *
 *   ProtocolUtility.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/09/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Hangout.Shared;

namespace Hangout.Client
{
	public class ProtocolUtility
	{
		// Parses a string in the form protocol://Path/MorePath/File into 'protocol' and 'Path/MorePath/File'
		private static readonly Regex mProtocolParser = new Regex(@"(?<1>.+)://(?<2>.+)", RegexOptions.Compiled);

		public static Pair<string> SplitProtocol(string pathWithProtocol)
		{
			Match protocolMatch = mProtocolParser.Match(pathWithProtocol);

			string protocol;
			string path;
			if (protocolMatch.Success)
			{
				protocol = protocolMatch.Groups[1].ToString();
				path = protocolMatch.Groups[2].ToString();
			}
			else
			{
				throw new Exception("Unrecognized protocol in path: " + pathWithProtocol);
			}

			return new Pair<string>(protocol, path);
		}

		public static bool HasProtocol(string path)
		{
			return mProtocolParser.IsMatch(path);
		}

		// Instance variable for the AssetsRoot property
		private static string mAssetsRoot = null;

		public static string GetAssetDataPath()
		{
            if (Application.isEditor)
            {
                return "file://" + Application.dataPath + "/../ExportedAssets/";
            }
            else
            {
                return Application.dataPath + "/../ExportedAssets/";
            }


            //if (!Application.dataPath.Contains("file:///"))
            //{
            //    return "file://" + Application.dataPath + "/../ExportedAssets/";
            //}
            //else
            //{
            //    return Application.dataPath + "/../ExportedAssets/";
            //}
		}

		/// <summary>
		/// The root directory for the assets:// protocol
		/// </summary>
		private static string AssetsRoot
		{
			get
			{
				if (mAssetsRoot == null)
				{
					if( Application.isEditor )
					{
						mAssetsRoot = GetAssetDataPath();
					}
					else
					{
						ConnectionProxy connectionProxy = GameFacade.Instance.RetrieveProxy<ConnectionProxy>();
						mAssetsRoot = connectionProxy.AssetBaseUrl;
					}
				}
				return mAssetsRoot;
			}
		}

		/// <summary>
		/// Converts a string with a protocol to the path used in loading from whatever source it will need
		/// </summary>
		public static Pair<string> SplitAndResolve(string pathWithProtocol)
		{
			string result;
			Pair<string> protocolPath = SplitProtocol(pathWithProtocol);
			switch (protocolPath.First.ToLower())
			{
				case "resources":
					result = protocolPath.Second;
					break;
				case "file":
				case "http":
					result = pathWithProtocol;
					break;
				case "assets":
					result = ProtocolUtility.AssetsRoot + protocolPath.Second;
					break;
				default:
					throw new Exception("Unrecognized protocol (" + protocolPath.First + ") in path: " + pathWithProtocol);
			}

			protocolPath.Second = result;
			return protocolPath;
		}

		public static string ResolvePath(string pathWithProtocol)
		{
			return SplitAndResolve(pathWithProtocol).Second;
		}

		public static string ConvertToFilePath(string path)
		{
			string result;
			Pair<string> splitAndResolved = SplitAndResolve(path);
			if( splitAndResolved.First == "resources" )
			{
				result = Application.dataPath + "/Resources/" + splitAndResolved.Second;
			}
			else if( splitAndResolved.First == "file" )
			{
				result = SplitProtocol(path).Second;
			}
			else
			{
				throw new Exception("ConvertToFileProtocol only works on file:// and resources:// paths (Invalid Input: " + path + ")");
			}

			return result;
		}
	}
}