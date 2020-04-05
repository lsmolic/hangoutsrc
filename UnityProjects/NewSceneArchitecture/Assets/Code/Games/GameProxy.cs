/**  --------------------------------------------------------  *
 *   GameProxy.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 07/08/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using PureMVC.Interfaces;
using PureMVC.Patterns;

using Hangout.Client.Gui;
using Hangout.Shared;

namespace Hangout.Client
{
	public class GameProxy : Proxy, IProxy, IGameProxy
	{ 

		public void InitializeScene()
		{
			RenderSettings.ambientLight = new Color(0.74f, 0.74f, 0.74f, 1.0f);
		}
	}
}
