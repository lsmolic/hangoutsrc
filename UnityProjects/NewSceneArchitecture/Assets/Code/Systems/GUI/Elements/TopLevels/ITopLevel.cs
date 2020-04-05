/**  --------------------------------------------------------  *
 *   ITopLevel.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/15/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

using Hangout.Shared;

namespace Hangout.Client.Gui 
{
	public interface ITopLevel : IGuiElement, IGuiContainer 
	{
		IGuiManager Manager { get; }
		void Draw(IGuiContainer container, Vector2 position);
		void Close();
		void AddOnCloseAction(Hangout.Shared.Action onCloseCallback);
	}	
}
