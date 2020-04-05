/**  --------------------------------------------------------  *
 *   GuiRange.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 11/06/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client.Gui
{
	public sealed class Scrollbar : GuiRange, IGuiContainer
	{
		private readonly ScrollbarThumb mThumb;

		private readonly PushButton mTrough;
		private readonly string mPathToFrame;

		private ScrollFrame mFrame = null;
		private ScrollFrame Frame
		{
			get
			{
				if( mFrame == null )
				{
					GuiPath path = new GuiPath(mPathToFrame);
					mFrame = path.SelectSingleElement<ScrollFrame>(this);
					if( mFrame == null )
					{
						throw new Exception("Cannot find ScrollFrame at path(" + mPathToFrame + ") relative to Scrollbar(" + Name + ")");
					}
					mFrame.Scrollbar = this;
				
					if(!(Parent is IGuiFrame))
					{
						// The reason for this is that the mousePosition values are in relation to the
						// container (Frame) that the widget is drawn in. We could make this more sophisticated
						// later if we need to.
						throw new Exception("Scrollbars aren't supported outside of frames");
					}

					Vector2 scrollBarStart = ((IGuiContainer)Parent).GetChildPosition(this);
					Vector2 scrollBarEnd = scrollBarStart + Size;
					Rangef scrollRange = new Rangef(scrollBarStart.y, scrollBarEnd.y);
					mTrough.AddMouseDragCallback(delegate(Vector2 mousePosition)
					{
						Percent = Mathf.Clamp01(scrollRange.ParametricValue(mousePosition.y));
					});
				}
				return mFrame;
			}
		}

		public override object Clone()
		{
			return new Scrollbar(this);
		}
		
		public Scrollbar( Scrollbar copy )
		: base(copy)
		{
			mThumb = new ScrollbarThumb(copy.mThumb);
			mTrough = new PushButton(copy.mTrough);

			mFrame = copy.mFrame;
			mPathToFrame = copy.mPathToFrame;
		}

		public Scrollbar
		(
			string name,
			IGuiStyle troughStyle,
			IGuiStyle thumbStyle,
			IGuiSize size,
			string pathToFrame
		)
			: base(name, troughStyle, size, new Rangef(0.0f, 1.0f), 0.0f)
		{
			if (String.IsNullOrEmpty(pathToFrame))
			{
				throw new ArgumentNullException("pathToFrame");
			}
			mPathToFrame = pathToFrame;

			mThumb = new ScrollbarThumb("ScrollThumb", thumbStyle, new ProceduralSize(SizeThumb), this);
			mThumb.Parent = this;

			mTrough = new PushButton("ScrollTrough", size, troughStyle);
			mTrough.Parent = this;
		}


		private Rect? mOldWidgetBounds = null;

		public Vector2 SizeThumb(IGuiElement thumb)
		{
			if(thumb != mThumb)
			{
				throw new Exception("ProceduralSize is applying the SizeThumb function to the wrong widget (" + thumb.Name + ")");
			}

			float minHeight = 0.0f;
			if (thumb.Style != null)
			{
				// Keep the thumb graphic from overlapping itself
				minHeight = thumb.Style.NinePartScale.GetSizeDifference().y;
			}

			Vector2 result = InternalSize;
			float maxHeight = result.y;
			Rect widgetBounds = Frame.CalculateWidgetBounds();
			widgetBounds = RectUtility.BoundingRect(Vector2.zero, widgetBounds);
			float percentInView = Frame.InternalSize.y / widgetBounds.yMax;
			// If the entire frame fits, make sure we are viewing the top of the frame
			if (percentInView >= 1.0f)
			{
				Frame.ResetScroll();
			}
			// Refresh the frame to make sure the scrollbar's percent is again accurate,
			// but only if the bounds changed (as an optimization)
			if (mOldWidgetBounds != null && (Rect)mOldWidgetBounds != widgetBounds)
			{
				Refresh();
			}
			// Remember this run's bounds for next time
			mOldWidgetBounds = widgetBounds;
			result.y = Mathf.Lerp(minHeight, maxHeight, percentInView);

			return result;
		}
		
		public override void Draw(IGuiContainer container, Vector2 position)
		{
			mTrough.Draw(container, position);
			mThumb.Draw(container, position + new Vector2(0.0f, Mathf.Lerp(0.0f, Size.y - mThumb.ExternalSize.y, this.Percent)));
		}

		#region IGuiContainer Members

		public void UpdateChildPosition(IGuiElement childElement, Vector2 position)
		{
			throw new NotImplementedException();
		}

		public Vector2 GetChildPosition(IGuiElement childElement)
		{
			throw new NotImplementedException();
		}

		public IGuiPosition GetChildGuiPosition(IGuiElement childElement)
		{
			throw new NotImplementedException();
		}

		public System.Collections.Generic.IEnumerable<IGuiElement> Children
		{
			get 
			{
				yield return mThumb;
				yield return mTrough;
			}
		}

		public string ContainerName
		{
			get { return Name; }
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

		#endregion
	}	
}

