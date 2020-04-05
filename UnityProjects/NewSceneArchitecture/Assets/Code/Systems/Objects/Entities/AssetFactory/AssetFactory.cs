/**  --------------------------------------------------------  *
 *   ObjectFactory.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 07/07/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Hangout.Client
{
	public class AssetFactory
	{
		private Dictionary<string, List<KeyValuePair<uint, string>>> mLookUpTable = null;

		public AssetFactory(XmlDocument xmlLookUpTable)
		{
			mLookUpTable = new Dictionary<string, List<KeyValuePair<uint, string>>>();
			ParseLookUpTableDocument(xmlLookUpTable);
		}

		//The xmldoc passed in is the xml representation of the lookup table.  We parse that all
		//at once so we don't have to go trapsing through it every time we want to instantiate
		//the gameobject version of a distributed object.
		private void ParseLookUpTableDocument(XmlDocument lookUpTable)
		{
			foreach (XmlNode xmlNode in lookUpTable.SelectNodes("/Asset"))
			{
				string type = xmlNode.SelectSingleNode("type").InnerText;
				uint index = uint.Parse(xmlNode.SelectSingleNode("index").InnerText);
				string path = xmlNode.SelectSingleNode("path").InnerText;
				KeyValuePair<uint, string> indexAndPath = new KeyValuePair<uint, string>(index, path);
				if (!mLookUpTable.ContainsKey(type))
				{
					List<KeyValuePair<uint, string>> newIndexAndPathList = new List<KeyValuePair<uint, string>>();
					newIndexAndPathList.Add(indexAndPath);
					mLookUpTable.Add(type, newIndexAndPathList);
				}
				else if (!mLookUpTable[type].Contains(indexAndPath))
				{
					mLookUpTable[type].Add(indexAndPath);
				}
			}

			if (mLookUpTable.Count == 0)
			{
				Console.LogError("Asset factory could not parse any instructions out of the xml passed into it.  The format of the xml is most likely incorrect.");
			}
		}

		//We pass in the type and index which correlates to the look up table and use that
		//to figure out what path we'll be using to load the asset.
		public GameObject CreateAsset(KeyValuePair<string, uint> assetTypeAndIndex)
		{
			foreach (KeyValuePair<uint, string> kvp in mLookUpTable[assetTypeAndIndex.Key])
			{
				if (kvp.Key == assetTypeAndIndex.Value)
				{
					if (PathIsAUrl(kvp.Value))
					{
						return DownloadAsset();
					}
					else
					{
						//This is placeholder instantiation code since The prototype will just be pulling things
						//outta the resource folder.
						GameObject newAsset = GameObject.Instantiate(Resources.Load(kvp.Value)) as GameObject;
						return newAsset;
					}
				}
			}
			Console.LogError("The asset you are looking up is not in the lookup table.");
			return null;
		}

		private bool PathIsAUrl(string path)
		{
			//This function will determine whether we have the object cached or if we need to download the asset.
			//Code to come later.
			return false;
		}

		private GameObject DownloadAsset()
		{
			throw new NotImplementedException();
		}
	}
}