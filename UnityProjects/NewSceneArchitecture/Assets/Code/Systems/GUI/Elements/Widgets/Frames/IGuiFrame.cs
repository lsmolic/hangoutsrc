/**  --------------------------------------------------------  *
 *   IGuiFrame.cs  
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
	
	/// An IGuiFrame is an element that contains other IWidgets into a displayable region.
	public interface IGuiFrame : IWidget, IGuiContainer
	{
		int ChildCount { get; }
		void ClearChildWidgets();
		void AddChildWidget(IWidget child, IGuiPosition position);
		void RemoveChildWidget(IWidget child);	
	}
}