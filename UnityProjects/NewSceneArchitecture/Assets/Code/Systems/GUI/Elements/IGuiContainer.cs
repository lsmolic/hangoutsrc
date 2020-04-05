/**  --------------------------------------------------------  *
 *   IGuiContainer.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/19/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public interface IGuiContainer
	{
		void UpdateChildPosition(IGuiElement childElement, Vector2 position);
		Vector2 GetChildPosition(IGuiElement childElement);
		IGuiPosition GetChildGuiPosition(IGuiElement childElement);

		IEnumerable<IGuiElement> Children { get; }
		string ContainerName { get; }

		T SelectSingleElement<T>(string guiPath) where T : IGuiElement; 
		IEnumerable<T> SelectElements<T>(string guiPath) where T : IGuiElement;
	}
}
