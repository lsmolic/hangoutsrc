/**  --------------------------------------------------------  *
 *   MakeupStation.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/28/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;

using PureMVC.Patterns;
using PureMVC.Interfaces;

using Hangout.Shared;
using Hangout.Client;
using Hangout.Client.Gui;

using UnityEngine;

namespace Hangout.Client.FashionGame
{
	public class MakeupStation : ModelStation
	{
		public MakeupStation(Pair<Vector3> location, string name, Texture2D image, float time, Vector3 guiOffset, GameObject displayObject)
			: base(location, name, image, time, guiOffset, displayObject)
		{
		}
	}
}
