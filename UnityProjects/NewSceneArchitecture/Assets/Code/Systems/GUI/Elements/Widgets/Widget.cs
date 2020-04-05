/**  --------------------------------------------------------  *
 *   Widget.cs  
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
	public abstract class Widget : GuiElement, IWidget 
	{
		public Widget( string name,
					   IGuiSize size,
					   IGuiStyle style )
		: base( name, size, style ) { }

		public abstract void Draw(IGuiContainer container, Vector2 position);

        
        public ITopLevel GetContainingTopLevel() 
		{
			ITopLevel result = GetContainer<ITopLevel>();

			// Everything should be in an ITopLevel at some point
			if(result == null)
			{
				throw new System.Exception("The parent structure for this GUI Element (" + this.Name + ") is corrupted, Widgets should always be in a TopLevel.");
			}

			return result;
		}

		public T GetContainer<T>() where T : IGuiContainer
		{
			IGuiElement parent = this.Parent;
			while (!(parent is T))
			{
				if (parent == null)
				{
					return default(T);
				}
				parent = parent.Parent;
			}

			return (T)parent;
		}
	}
}