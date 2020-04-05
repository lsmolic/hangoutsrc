/**  --------------------------------------------------------  *
 *   IGuiWidget.cs  
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
	/// An IWidget is an IGuiElement that is visible and exists in an ITopLevel
	public interface IWidget : IGuiElement
	{
		void Draw(IGuiContainer container, Vector2 position);
		ITopLevel GetContainingTopLevel();
		T GetContainer<T>() where T : IGuiContainer;
        Vector2 GetScreenPosition();
		Vector2 GetWidgetPosition(Vector2 screenCoords);
	}
}