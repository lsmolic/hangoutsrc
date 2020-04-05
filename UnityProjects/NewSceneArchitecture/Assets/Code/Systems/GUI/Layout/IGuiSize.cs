/**  --------------------------------------------------------  *
 *   IGuiSize.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/15/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

namespace Hangout.Client.Gui
{
	/// <summary>
	/// IGuiSize is a strategy to change the size of an IWidget based off of an arbitrary function
	/// See FillParent.cs for an example implementation
	/// </summary>
	public interface IGuiSize
	{
		Vector2 GetSize(IGuiElement widget);
	}
}