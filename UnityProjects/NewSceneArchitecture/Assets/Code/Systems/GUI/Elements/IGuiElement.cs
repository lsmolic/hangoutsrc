/**  --------------------------------------------------------  *
 *   IGuiElement.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/15/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public interface IGuiElement : ICloneable
	{
		/// Heiarchy information, a Parent is usually a containing IGuiFrame or ITopLevel
		IGuiElement Parent { get; set; }

		string Name { get; }

		IGuiStyle Style { get; set; }

		Vector2 ExternalSize { get; } /// Get the ExternalSize of this Element, including any margins defined in an IGuiStyle
		Vector2 InternalSize { get; } /// Get the InternalSize of this Element, including any margins defined in an IGuiStyle
		Vector2 Size { get; }	/// Get the base size of the element before any margins are applied.
		IGuiSize GuiSize { get; set; }

		bool Showing { get; set; }

		ITopLevel GetTopLevel();
	}
}

