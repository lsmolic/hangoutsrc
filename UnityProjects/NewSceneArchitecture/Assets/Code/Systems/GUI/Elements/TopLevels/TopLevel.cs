/**  --------------------------------------------------------  *
 *   GuiTopLevel.cs  
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
	public abstract class TopLevel : GuiElement, ITopLevel
	{
		private readonly IGuiManager mManager;

		public string ContainerName 
		{ 
			get { return this.Name; } 
		}
		
		/// Unity Window function calls require unique IDs, this manager creates them.
		private static class UnityWindowIdManager 
		{
			private static int mNextId = 0;
		
			public static int RequestNewWindowId() 
			{
				return ++mNextId;
			}
		}
		
		public override IGuiElement Parent 
		{
			get { return null; }
			set { throw new Exception("TopLevels can't have a parent."); }
		}
		
		private KeyValuePair<IGuiFrame, IGuiPosition> mMainFrame;
		public KeyValuePair<IGuiFrame, IGuiPosition> MainFrame 
		{
			get { return mMainFrame; }
			set { mMainFrame = value; }
		}
		
		private readonly IGuiPosition mPosition;
		public IGuiPosition GuiPosition
		{
			get { return mPosition; }
		}
	
		public TopLevel( string name,
					     IGuiSize size,
					     IGuiPosition position,
					     IGuiManager manager,
					     KeyValuePair<IGuiFrame, IGuiPosition> mainFrame,
					     IGuiStyle style ) 
			: base( name, size, style ) 
		{
			if( manager == null ) 
			{
				throw new ArgumentNullException("manager");
			}
			
			if( position == null ) 
			{
				throw new ArgumentNullException("position");
			}
			mPosition = position;
			
			mWindowId = UnityWindowIdManager.RequestNewWindowId();
			mManager = manager;
			mManager.RegisterTopLevel( this, position );
			mMainFrame = mainFrame;
			if( mMainFrame.Value == null ) 
			{
				mMainFrame = new KeyValuePair<IGuiFrame, IGuiPosition>(mainFrame.Key, new MainFrameSizePosition());
			}
			
			if( mMainFrame.Key != null ) 
			{
				mMainFrame.Key.Parent = this;
			}
		}
		
		public virtual IEnumerable<IGuiElement> Children 
		{
			get 
			{
				yield return mMainFrame.Key; 
			}
		}
		
		public Vector2 GetChildPosition(IGuiElement childElement)
		{
			return GetChildGuiPosition(childElement).GetPosition(childElement);
		}

		public IGuiPosition GetChildGuiPosition(IGuiElement childElement)
		{
			if( childElement != mMainFrame.Key )
			{
				throw new Exception("GuiElement(" + childElement.Name + ") isn't a child of TopLevel(" + this.Name + ")");
			}
			return mMainFrame.Value;
		}
	
		public IGuiManager Manager
		{
			get { return mManager; }
		}
	
		// For subclasses that use unity's GUI.Window function
		private int mWindowId;
		protected int WindowId 
		{
			get { return mWindowId; }
		}
	
		public delegate Rect GuiWindowCall1(int id, Rect coords, GUI.WindowFunction windowCallback, string windowTitle, GUIStyle windowStyle);
		public delegate Rect GuiWindowCall2(int id, Rect coords, GUI.WindowFunction windowCallback, string windowTitle);
	
		private GuiWindowCall1 mGuiWindowCall = GUI.Window;
		private GuiWindowCall2 mGuiWindowCallNoStyle = GUI.Window;

		public GuiWindowCall1 GuiWindowCall 
		{
			get { return mGuiWindowCall; }
			set { mGuiWindowCall = value; }
		}
		
		public GuiWindowCall2 GuiWindowCallNoStyle 
		{
			get { return mGuiWindowCallNoStyle; }
			set { mGuiWindowCallNoStyle = value; }
		}

		public T SelectSingleElement<T>(string guiPath) where T : IGuiElement
		{
			GuiPath path = new GuiPath(guiPath);
			return path.SelectSingleElement<T>(this);
		}

		public IEnumerable<T> SelectElements<T>(string guiPath) where T : IGuiElement
		{
			GuiPath path = new GuiPath(guiPath);
			return path.SelectElements<T>(this);
		}
	
		private Hangout.Shared.Action mOnClose = null;
		public void AddOnCloseAction( Hangout.Shared.Action onClose ) 
		{
			mOnClose += onClose;
		}
	
		public void Close() 
		{
			if( mOnClose != null ) 
			{
				mOnClose();
			}
		
			mManager.UnregisterTopLevel( this );
		}
		
		public override ITopLevel GetTopLevel() 
		{
			return this;
		}
		
		public virtual void UpdateChildPosition(IGuiElement frame, Vector2 position) 
		{
			throw new Exception("TopLevel frames are not moveable. This happend in Window (" + this.Name + ") on Frame (" + frame.Name + ")");
		}
		
		public abstract void Draw(IGuiContainer container, Vector2 position);
	}
}

