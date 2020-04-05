/**  --------------------------------------------------------  *
 *   LevelGui.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/27/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Shared;
using Hangout.Client.Gui;

using UnityEngine;


namespace Hangout.Client.FashionGame
{
	public class ModelWave
	{
		// List of the models' names in this Wave
		private readonly List<Pair<string, FashionModelNeeds>> mModels = new List<Pair<string, FashionModelNeeds>>();
		public List<Pair<string, FashionModelNeeds>> Models
		{
			get { return mModels; }
		}
		private int mSpawnedModels = 0;
		public bool AllModelsSpawned
		{
			get { return mSpawnedModels == mModels.Count; }
		}
		public void ModelSpawned()
		{
			mSpawnedModels++;
		}
		public IEnumerable<FashionModelNeeds> Needs
		{
			get
			{
				foreach (Pair<string, FashionModelNeeds> modelListing in mModels)
				{
					yield return modelListing.Second;
				}
			}
		}

		public IEnumerable<string> ModelTypes
		{
			get
			{
				foreach (Pair<string, FashionModelNeeds> modelListing in mModels)
				{
					yield return modelListing.First;
				}
			}
		}

		private readonly float mTimeBetweenSpawns;
		public float TimeBetweenSpawns
		{
			get { return mTimeBetweenSpawns; }
		}

		public ModelWave(XmlNode waveNode, FashionNpcMediator factory, IEnumerable<ModelStation> modelStations)
		{
			if (waveNode == null)
			{
				throw new ArgumentNullException("waveNode");
			}

			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}

			XmlAttribute timeAttrib = waveNode.Attributes["timeBetweenSpawns"];
			if (timeAttrib == null)
			{
				throw new Exception("Unable to find 'timeBetweenSpawns' attribute in XmlNode (" + waveNode.Name + ")");
			}

			if (!float.TryParse(timeAttrib.InnerText, out mTimeBetweenSpawns))
			{
				throw new Exception("Unable to parse the number (" + timeAttrib.InnerText + ") in the 'timeBetweenSpawns' attribute in XmlNode (" + waveNode.Name + ")");
			}

			foreach (XmlNode modelNode in waveNode.SelectNodes("Model"))
			{
				XmlAttribute typeAttrib = modelNode.Attributes["type"];
				if (typeAttrib == null)
				{
					throw new Exception("Unable to find 'type' attribute in XmlNode (" + modelNode.Name + ")");
				}
				string type = typeAttrib.InnerText;
				mModels.Add(new Pair<string, FashionModelNeeds>(type, factory.BuildNeedsForType(type, modelStations)));
			}
		}
	}
}
