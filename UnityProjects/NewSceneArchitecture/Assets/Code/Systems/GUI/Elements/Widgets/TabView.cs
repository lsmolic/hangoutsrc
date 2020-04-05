/**  --------------------------------------------------------  *
 *   TabView.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/18/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public class TabView : Widget, IGuiContainer
	{		
		private readonly List<TabButton> mTabs;	
		private readonly GuiFrame mButtonFrame;
		public GuiFrame ButtonFrame
		{
			get { return mButtonFrame; }
		}

		private readonly IGuiPosition mButtonFramePosition;
		public IGuiPosition ButtonFramePosition
		{
			get { return mButtonFramePosition; }
		}

		private readonly GuiFrame mContextFrame;
		public GuiFrame ContextFrame
		{
			get { return mContextFrame; }
		}

		private readonly IGuiPosition mContextFramePosition;
		public IGuiPosition ContextFramePosition
		{
			get { return mContextFramePosition; }
		}

		
		private bool mAllowEmpty;
		public bool AllowEmpty
		{
			get { return mAllowEmpty; }
			set { mAllowEmpty = value; }
		}

		
		private static readonly FillParent mActiveFramePosition = new FillParent();
		
		
		public override object Clone()
		{
			return new TabView(this);
		}
		
		public void ActivateTab( TabButton toActivate ) 
		{
			foreach( TabButton tab in mTabs ) 
			{
				if( tab != toActivate )
				{
					tab.Activated = false;
				}
			}
			
			mContextFrame.ClearChildWidgets();
			mContextFrame.AddChildWidget(toActivate.Frame, mActiveFramePosition);
		}
		
		public void ClearContextFrame() 
		{
			if( mAllowEmpty ) 
			{
				mContextFrame.ClearChildWidgets();
			}
		}
		
		public TabView(TabView copy)
		: this
		(
			copy.Name, 
			copy.GuiSize, 
			(GuiFrame)copy.ButtonFrame.Clone(), 
			copy.ButtonFramePosition,
			(GuiFrame)copy.ContextFrame.Clone(),
			copy.ContextFramePosition,
			copy.AllowEmpty
		)
		{
			
		}
		public TabView(string name,
					   IGuiSize size,
					   GuiFrame buttonFrame,
					   IGuiPosition buttonFramePosition,
					   GuiFrame contextFrame,
					   IGuiPosition contextFramePosition,
					   bool allowEmpty ) 
		: base(name, size, null) 
		{
			mAllowEmpty = allowEmpty;
			
			if( buttonFrame == null ) 
			{
				throw new ArgumentNullException("buttonFrame");
			}
			mButtonFrame = buttonFrame;
			mButtonFrame.Parent = this;
			
			if( buttonFramePosition == null ) 
			{
				throw new ArgumentNullException("buttonFramePosition");
			}
			mButtonFramePosition = buttonFramePosition;

			if( contextFrame == null ) 
			{
				throw new ArgumentNullException("contextFrame");
			}
			mContextFrame = contextFrame;
			mContextFrame.Parent = this;
			
			if( contextFramePosition == null ) 
			{
				throw new ArgumentNullException("contextFramePosition");
			}
			mContextFramePosition = contextFramePosition;
			
			
			// Get all the elements of type TabButton in the buttonsFrame and store in mTabs
			mTabs = new List<TabButton>();
			foreach( IGuiElement buttonFrameElement in buttonFrame.SelectElements<IGuiElement>("**/*") ) 
			{
				if( buttonFrameElement is TabButton ) 
				{
					TabButton tabButton = (TabButton)buttonFrameElement;
					
					mTabs.Add(tabButton);
					tabButton.RegisterWithTabView(this);
				}
			}
			
			if( mTabs.Count < 1 ) 
			{
				throw new ArgumentException("Cannot create a TabView without any specified tabs.", "tabs");
			}
			
			if( !mAllowEmpty ) 
			{
				ActivateTab(mTabs[0]);
			}
		}

		
		public override void Draw(IGuiContainer parent, Vector2 position) 
		{
			mButtonFrame.Draw(this, mButtonFramePosition.GetPosition(mButtonFrame));
			mContextFrame.Draw(this, mContextFramePosition.GetPosition(mContextFrame));
		}
		
		public void UpdateChildPosition(IGuiElement element, Vector2 position) 
		{
			if( element == mButtonFrame ) 
			{
				mButtonFramePosition.UpdatePosition(element, position);
			}
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
		
		public IEnumerable<IGuiElement> Children 
		{
			get 
			{
				yield return mButtonFrame;
				yield return mContextFrame;
			}
		}
		
		public Vector2 GetChildPosition(IGuiElement childElement)
		{
			return GetChildGuiPosition(childElement).GetPosition(childElement);
		}

		public IGuiPosition GetChildGuiPosition(IGuiElement childElement)
		{
			IGuiPosition result;
			if(childElement == mButtonFrame)
			{
				result = mButtonFramePosition;
			}
			else if(childElement == mContextFrame)
			{
				result = mContextFramePosition;
			}
			else
			{
				throw new Exception("Attempted to get a position for a child(" + childElement.Name + ") that isn't part of the IGuiContainer(" + this.ContainerName + ")");
			}
			return result;
		}
		
		public string ContainerName 
		{
			get { return this.Name; }
		}
	}
}
