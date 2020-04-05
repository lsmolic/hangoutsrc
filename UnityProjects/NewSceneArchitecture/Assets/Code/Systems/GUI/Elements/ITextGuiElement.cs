/**  --------------------------------------------------------  *
 *   ITextGuiElement.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/29/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public interface ITextGuiElement : IGuiElement
	{
		string Text
		{
			get;
			set;
		}
	}	
}
