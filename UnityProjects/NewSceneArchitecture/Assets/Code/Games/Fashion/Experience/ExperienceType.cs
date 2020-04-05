/**  --------------------------------------------------------  *
 *   ExperienceType.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 11/24/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using System.Xml;

using Hangout.Client.Gui;
using Hangout.Shared;
using Hangout.Shared.FashionGame;

using PureMVC.Patterns;

using UnityEngine;

namespace Hangout.Client.FashionGame
{
	public enum ExperienceType
	{
		CloseSave,
		FastModelCompletion,
		ClothingSewn,
		MultipleClothingPlacement,
		ModelComplete,
		NextWave,
		PerfectLevel
	}
}
