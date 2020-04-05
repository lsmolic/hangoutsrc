/**  --------------------------------------------------------  *
 *   Window.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/15/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;
using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client.Gui
{
	/// Describes which part of the window can be clicked-and-dragged to reposition the window
	public enum DragBehavior 
	{
		NotDraggable, HeaderOnly, EntireWindow
	}
	
	public class Window : TopLevel 
	{
		public const DragBehavior DefaultDragBehavior = DragBehavior.HeaderOnly;
		private readonly ILogger mLogger;
		private DragBehavior mDraggable;
		private KeyValuePair<IGuiFrame, IGuiPosition> mHeaderFrame;
		
		public override object Clone()
		{
			return new Window
			(
				this.Name, 
				this.GuiSize, 
				this.GuiPosition, 
				this.Manager, 
				new KeyValuePair<IGuiFrame, IGuiPosition>
				(
					(GuiFrame)this.MainFrame.Key.Clone(),
					this.MainFrame.Value
				), 
				new KeyValuePair<IGuiFrame, IGuiPosition>
				(
					(GuiFrame)this.HeaderFrame.Key.Clone(),
					this.HeaderFrame.Value
				),
				mDraggable,
				this.Style
			);
		}
		
		public Window( string name,
					   IGuiSize size, 
					   IGuiPosition position,
					   IGuiManager manager,
					   KeyValuePair<IGuiFrame, IGuiPosition> mainFrame, 
					   KeyValuePair<IGuiFrame, IGuiPosition> headerFrame,
					   DragBehavior isDraggable,
					   IGuiStyle style )
		: base(	name, size, position, manager, mainFrame, style ) 
		{
			mLogger = manager.Logger;
			mDraggable = isDraggable;
			mHeaderFrame = headerFrame;
			if( mHeaderFrame.Key != null ) 
			{
				mHeaderFrame.Key.Parent = this;
			}
			
			if( mHeaderFrame.Value == null ) 
			{
				mHeaderFrame = new KeyValuePair<IGuiFrame, IGuiPosition>
				(
					mHeaderFrame.Key, 
					new HeaderFrameSizePosition()
				);
			}
		}
			
		public Window( string name,
					   IGuiSize size, 
					   IGuiManager manager,
					   KeyValuePair<IGuiFrame, IGuiPosition> mainFrame, 
					   KeyValuePair<IGuiFrame, IGuiPosition> headerFrame,
					   DragBehavior isDraggable,
					   IGuiStyle style )
		: this(name, size, new FixedPosition(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f)), manager, mainFrame, headerFrame, isDraggable, style) 
		{
		}
		
		public Window( string name,
					   IGuiSize size,
					   IGuiManager manager,
					   KeyValuePair<IGuiFrame, IGuiPosition> mainFrame, 
					   KeyValuePair<IGuiFrame, IGuiPosition> headerFrame,
					   IGuiStyle style )
		: this(name, size, manager, mainFrame, headerFrame, Window.DefaultDragBehavior, style ) 
		{
		}
		
		public Window( string name,
					   IGuiSize size,
					   IGuiManager manager,
					   IGuiStyle style )
		: this(name, size, manager, new KeyValuePair<IGuiFrame, IGuiPosition>(null, null), new KeyValuePair<IGuiFrame, IGuiPosition>(null, null), style) 
		{
		}
		
		public Window( string name,
					   IGuiSize size,
					   IGuiManager manager,
					   KeyValuePair<IGuiFrame, IGuiPosition> mainFrame,
					   IGuiStyle style )
		: this(name, size, manager, mainFrame, new KeyValuePair<IGuiFrame, IGuiPosition>(null, null), style) 
		{
		}
		
		public Window( string name,
					   IGuiSize size,
					   IGuiManager manager,
					   IGuiFrame mainFrame,
					   IGuiStyle style )
		: this(name, size, manager, new KeyValuePair<IGuiFrame, IGuiPosition>(mainFrame, null), new KeyValuePair<IGuiFrame, IGuiPosition>(null, null), style)
		{
		}
		
		
		public Window( string name,
					   IGuiSize size,
					   IGuiManager manager,
					   IGuiFrame mainFrame,
					   IGuiFrame headerFrame,
					   IGuiStyle style )
		: this(name, size, manager, new KeyValuePair<IGuiFrame, IGuiPosition>(mainFrame, null), new KeyValuePair<IGuiFrame, IGuiPosition>(headerFrame, null), style) 
		{
		}
		
		public Window( string name,
					   IGuiSize size,
					   IGuiManager manager,
					   IGuiFrame mainFrame )
		: this(name, size, manager, new KeyValuePair<IGuiFrame, IGuiPosition>(mainFrame, null), new KeyValuePair<IGuiFrame, IGuiPosition>(null, null), null) 
		{
		}
		
		
		public Window( string name,
					   IGuiSize size,
					   IGuiManager manager )
		: this(name, size, manager, new KeyValuePair<IGuiFrame, IGuiPosition>(null, null), new KeyValuePair<IGuiFrame, IGuiPosition>(null, null), null) 
		{
		}

		public KeyValuePair<IGuiFrame, IGuiPosition> HeaderFrame 
		{
			get { return mHeaderFrame; }
			set { mHeaderFrame = value; }
		}
		
		// Weird hack-bugfix, Window.Children can't access base.Children and yield (Mono bug?)
		private IEnumerable<IGuiElement> BaseChildren
		{
			get
			{
				return base.Children;
			}
		}

		public override IEnumerable<IGuiElement> Children 
		{
			get 
			{
				foreach( IGuiElement element in BaseChildren ) 
				{
					yield return element;
				}
				if (mHeaderFrame.Key != null)
				{
					yield return mHeaderFrame.Key;
				}
			}
		}

		public override void Draw(IGuiContainer container, Vector2 position) 
		{
			Rect coords = new Rect(position.x, position.y, this.Size.x, this.Size.y);
			Rect newCoords;
			if (this.Style != null)
			{
				newCoords = this.GuiWindowCall(this.WindowId, coords, DrawContents, "", this.Style.GenerateUnityGuiStyle());
			}
			else
			{
				newCoords = this.GuiWindowCallNoStyle(this.WindowId, coords, DrawContents, "");
			}

			if (newCoords.x != coords.x || newCoords.y != coords.y)
			{
				container.UpdateChildPosition(this, new Vector2(newCoords.x, newCoords.y));
			}
		}

        private bool mInFront = false;
        public bool InFront
        {
            get { return mInFront; }
            set { mInFront = value; }
        }

		protected void DrawContents(int id) 
		{
            if (mInFront)
            {
                GUI.BringWindowToFront(id);
                //GUI.FocusWindow(id);
            }

			try
			{
				if (this.HeaderFrame.Key != null)
				{
					this.HeaderFrame.Key.Draw(this, this.HeaderFrame.Value.GetPosition(this.HeaderFrame.Key));
				}
				
				if (this.MainFrame.Key != null)
				{
					this.MainFrame.Key.Draw(this, this.MainFrame.Value.GetPosition(this.MainFrame.Key));
				}

				DragWindow();
			}
			catch (System.Exception e)
			{
				mLogger.Log(e.ToString(), LogLevel.Error);
			}
		}
		
		protected virtual void DragWindow() 
		{
			if( mDraggable == DragBehavior.HeaderOnly && mHeaderFrame.Key != null ) 
			{
				Rect headerCoords = new Rect(this.InternalSize.x, this.InternalSize.y, mHeaderFrame.Key.ExternalSize.x, mHeaderFrame.Key.ExternalSize.y);
				GUI.DragWindow(headerCoords);
			}
			else if( mDraggable == DragBehavior.EntireWindow ) 
			{
				GUI.DragWindow();
			}
		}

        public void FocusWindow()
        {
            GUI.FocusWindow(WindowId);
        }

        public void UnfocusWindow()
        {
            GUI.UnfocusWindow();
        }

        public void BringWindowToFront()
        {
            GUI.BringWindowToFront(WindowId);
        }

        public void BringWindowToBack()
        {
            GUI.BringWindowToBack(WindowId);
        }
	}
}