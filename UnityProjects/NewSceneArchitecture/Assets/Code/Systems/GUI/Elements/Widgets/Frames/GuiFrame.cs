/**  --------------------------------------------------------  *
 *   GuiFrame.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/18/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client.Gui
{
	public class GuiFrame : Widget, IGuiFrame 
	{
		public string ContainerName 
		{ 
			get { return this.Name; } 
		}
		
		/**
		 * Key = Widget, Value = position
		 * mWidgets is the list of widgets in draw order. This is the same order that the frame was constructed with
		 */
		private readonly List<KeyValuePair<IWidget, IGuiPosition>> mWidgets;
		protected IEnumerable<KeyValuePair<IWidget, IGuiPosition>> Widgets 
		{
			get { return mWidgets; }
		}
		
		public int ChildCount
		{
			get { return mWidgets.Count;  }
		}

		private bool mAutoLayoutNeedsUpdate = true;
		
		
		public override object Clone()
		{
			GuiFrame result = new GuiFrame
			(
				this.Name, 
				this.GuiSize, 
				this.Style
			);
			
			foreach(KeyValuePair<IWidget, IGuiPosition> widget in mWidgets)
			{
				result.AddChildWidget((IWidget)widget.Key.Clone(), widget.Value);
			}
			
			return result;
		}
		
		public void ClearChildWidgets() 
		{
			mWidgets.Clear();
		}
		
		public void AddChildWidget(IWidget child, IGuiPosition position) 
		{
			mWidgets.Add(new KeyValuePair<IWidget, IGuiPosition>(child, position));
			child.Parent = this;
			
			if( position is IAutoLayout ) 
			{
				mAutoLayoutNeedsUpdate = true;
			}
		}
		
		public void RemoveChildWidget(IWidget child)
		{
			int childIndex = 0;
			for(; childIndex < mWidgets.Count; ++childIndex )
			{
				if( mWidgets[childIndex].Key == child )
				{
					break;
				}
			}
			
			if( childIndex < mWidgets.Count )
			{
				
				if( mWidgets[childIndex].Value is IAutoLayout )
				{
					mAutoLayoutNeedsUpdate = true;
				}

				mWidgets.RemoveAt(childIndex);
				child.Parent = null;
			}
			else
			{
				throw new ArgumentException("IWidget (" + child.Name + ") was not found in GuiFrame (" + this.Name + ")");
			}
		}
		
		public IEnumerable<IGuiElement> Children 
		{
			get 
			{ 
				foreach(KeyValuePair<IWidget, IGuiPosition> kvp in mWidgets)
				{
					yield return kvp.Key;
				}
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
		
		/**
		 * /param[frameStyle] Can be null for an invisible GuiFrame
		 * /param[widgets] All the widgets for this frame. The list of widgets can be a Dictionary if the ordering doesn't matter. If ordering is required (ex. if the widgets use IAutoLayouts), then pass an ordered IEnumerator (List or Array).
		 */
		public GuiFrame( string name,
						 IGuiSize size,
						 IEnumerable<KeyValuePair<IWidget, IGuiPosition>> widgets,
						 IGuiStyle frameStyle )
		: base(name, size, frameStyle) 
		{
			if( widgets == null ) 
			{
				throw new ArgumentNullException("widgets");
			}
			mWidgets = new List<KeyValuePair<IWidget, IGuiPosition>>(widgets);
					
			RegisterChildrenWidgets();
		}
		
		public GuiFrame( string name,
						 IGuiSize size,
						 IGuiStyle frameStyle )
		: this(name, size, new List<KeyValuePair<IWidget, IGuiPosition>>(), frameStyle) 
		{
		}
		
		public GuiFrame(string name, IGuiSize size)
			: this(name, size, new List<KeyValuePair<IWidget, IGuiPosition>>(), (IGuiStyle)null)
		{
		}
		
		
		/**
		 * /param[widgets] All the widgets for this frame. The list of widgets can be a Dictionary if the ordering doesn't matter. If ordering is required (ex. if the widgets use IAutoLayouts), then pass an ordered IEnumerator (List or Array).
		 */
		public GuiFrame( string name,
						 IGuiSize size,
						 IEnumerable<KeyValuePair<IWidget, IGuiPosition>> widgets )
		: this( name, size, widgets, null ) {}
		
		private void RegisterChildrenWidgets() 
		{
			foreach( KeyValuePair<IWidget, IGuiPosition> widget in mWidgets ) 
			{
				widget.Key.Parent = this;
			}
		}

		private readonly List<Hangout.Shared.Func<Vector2, Vector2>> mModifyWidgetPositionCallbacks = new List<Hangout.Shared.Func<Vector2, Vector2>>();
		public IReceipt AddModifyPositionCallback(Hangout.Shared.Func<Vector2, Vector2> modifyPositionCallback)
		{
			mModifyWidgetPositionCallbacks.Add(modifyPositionCallback);
			return new Receipt(delegate()
			{
				mModifyWidgetPositionCallbacks.Remove(modifyPositionCallback);
			});
		}

		private readonly List<IWidget> mIgnoreModifyPosition = new List<IWidget>();
		public void IgnoreModifyPosition(IWidget widget)
		{
			mIgnoreModifyPosition.Add(widget);
		}
		
		public override void Draw(IGuiContainer container, Vector2 position) 
		{
			Rect coords = new Rect(position.x, position.y, this.Size.x, this.Size.y);
			AutoLayoutUpdate();
			
			if( this.Style != null ) 
			{
				GUI.BeginGroup( coords, this.Style.GenerateUnityGuiStyle() );
			} 
			else 
			{
				GUI.BeginGroup( coords );
			}

			try 
			{
				// The copy here is so that child widgets can modify the list of widgets here if necessary
				IEnumerable<KeyValuePair<IWidget, IGuiPosition>> widgets = new List<KeyValuePair<IWidget, IGuiPosition>>(mWidgets);
				foreach( KeyValuePair<IWidget, IGuiPosition> widget in widgets ) 
				{
					if( widget.Key.Showing )
					{
						Vector2 widgetPosition = widget.Value.GetPosition(widget.Key);
						if (!mIgnoreModifyPosition.Contains(widget.Key))
						{
							foreach (Hangout.Shared.Func<Vector2, Vector2> modifyPositionCallback in mModifyWidgetPositionCallbacks)
							{
								widgetPosition = modifyPositionCallback(widgetPosition);
							}
						}
						widget.Key.Draw(this, widgetPosition);
					}
				}

			} 
			finally 
			{
				GUI.EndGroup();
			}
		}
		
		public void AutoLayoutUpdate() 
		{
			if( mAutoLayoutNeedsUpdate ) 
			{
				List<Rect> reservedSpace = new List<Rect>(); 
				foreach( KeyValuePair<IWidget, IGuiPosition> widget in mWidgets ) 
				{
					widget.Key.Parent = this;

					// Add all the non-autoLayout widget locations to the reservedSpace
					if( !(widget.Value is IAutoLayout) )
					{
						Vector2 widgetPosition = widget.Value.GetPosition(widget.Key);
						reservedSpace.Add( new Rect(widgetPosition.x, widgetPosition.y, widget.Key.ExternalSize.x, widget.Key.ExternalSize.y) );
					}
				}

				IList<KeyValuePair<IWidget, IAutoLayout>> autolayoutWidgets =
				Functionals.Reduce<KeyValuePair<IWidget, IGuiPosition>, List<KeyValuePair<IWidget, IAutoLayout>>>
				(
					delegate(List<KeyValuePair<IWidget, IAutoLayout>> accumulator, KeyValuePair<IWidget, IGuiPosition> element)
					{
						if( element.Value is IAutoLayout )
						{
							accumulator.Add(new KeyValuePair<IWidget,IAutoLayout>(element.Key, (IAutoLayout)element.Value));
						}
						return accumulator;
					},
					mWidgets
				);

				// Assign coordinates to any IAutoLayout Positions
				foreach (KeyValuePair<IWidget, IAutoLayout> widget in autolayoutWidgets) 
				{
					reservedSpace.Add( widget.Value.NextPosition(widget.Key, reservedSpace, this.InternalSize, autolayoutWidgets) );
				}
				
				mAutoLayoutNeedsUpdate = false;	
			}
		}
		
		public void UpdateChildPosition(IGuiElement element, Vector2 position) 
		{
			if( !(element is IWidget) ) 
			{
				throw new ArgumentException("GuiFrames can only contain widgets", "element");
			}
			
			foreach( KeyValuePair<IWidget, IGuiPosition> kvp in mWidgets) 
			{
				if( kvp.Key == element ) 
				{
					kvp.Value.UpdatePosition(element, position);
				}
			}

			mAutoLayoutNeedsUpdate = true;
		}

		/// Get a bounding rectangle around all the widgets that have been added to this frame 
		public Rect CalculateWidgetBounds() 
		{ 
			if( mAutoLayoutNeedsUpdate ) 
			{
				AutoLayoutUpdate();
			}
			
			Rect? bounds = null;
			foreach( KeyValuePair<IWidget, IGuiPosition> widget in mWidgets ) 
			{
				if( widget.Key != null )
				{
					
					Vector2 position = widget.Value.GetPosition(widget.Key);
					Vector2 size = widget.Key.ExternalSize;
					Rect widgetBounds = new Rect(position.x, position.y, size.x, size.y);
					if( bounds == null )
					{
						bounds = widgetBounds;
					}
					else
					{
						bounds = RectUtility.BoundingRect(widgetBounds, (Rect)bounds);
					}
				}
			}

			if( bounds == null ) 
			{
				bounds = new Rect();
			}

			return (Rect)bounds;
		}

		public Vector2 GetChildPosition(IGuiElement childElement)
		{
			return GetChildGuiPosition(childElement).GetPosition(childElement);
		}
		public virtual IGuiPosition GetChildGuiPosition(IGuiElement childElement)
		{	
			foreach(KeyValuePair<IWidget, IGuiPosition> child in mWidgets)
			{
				if(child.Key == childElement)
				{
					return child.Value;
				}
			}
			throw new ArgumentException("Child Element not found in children.");
		}
	}	
}
