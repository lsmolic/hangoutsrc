/**  --------------------------------------------------------  *
 *   IGuiFactory.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/02/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public interface IGuiFactory
	{
		IEnumerable<IGuiElement> ConstructAllElements();
		IEnumerable<IGuiStyle> ConstructAllStyles();
	}
}
