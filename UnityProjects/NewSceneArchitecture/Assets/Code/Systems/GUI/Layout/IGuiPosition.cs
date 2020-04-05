/**  --------------------------------------------------------  *
 *   IGuiPosition.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/19/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

namespace Hangout.Client.Gui
{
	/// IGuiSize is a strategy to change the position of an IWidget based off of an arbitrary function.
	/// See FillParent.cs for an example implementation
	public interface IGuiPosition
	{
		void UpdatePosition(IGuiElement element, Vector2 newPosition);
		Vector2 GetPosition(IGuiElement element);
	}
}
