/**  --------------------------------------------------------  *
 *   ProgressIndicator.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/21/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public class ProgressIndicator : Widget, IGuiContainer
	{
		public enum Orientation { Horizontal, Vertical }
		private readonly Orientation mOrientation;

		private readonly IGuiFrame mTroughFrame;
		private readonly IGuiFrame mProgressFrame;

		public override object Clone()
		{
			return new ProgressIndicator(this);
		}

		public ProgressIndicator(ProgressIndicator copy)
			: base(copy.Name, copy.GuiSize, copy.Style)
		{
		}

		public ProgressIndicator(string name,
								IGuiSize size,
								IGuiStyle troughStyle,
								IGuiStyle progressStyle,
								Orientation orientation)
			: base(name, size, troughStyle)
		{
			if (progressStyle == null)
			{
				throw new ArgumentNullException("progressStyle");
			}
			mOrientation = orientation;

			mTroughFrame = new GuiFrame("ProgressTrough", size, troughStyle);
			mProgressFrame = new GuiFrame("Progress", new FixedSize(Vector2.zero), progressStyle);

			mTroughFrame.AddChildWidget(mProgressFrame, new FixedPosition(Vector2.zero, GuiAnchor.BottomLeft));
			mTroughFrame.Parent = this;
		}

		public void SetProgress(float progress)
		{
			if (progress < 0.0f || progress > 1.0f)
			{
				throw new ArgumentOutOfRangeException("progress must be in the range [0..1]");
			}

			Vector2 progressStyleSizeDiff = mProgressFrame.Style.NinePartScale.GetSizeDifference();
			if (mOrientation == Orientation.Horizontal)
			{
				float progressFrameSize = Mathf.Lerp(progressStyleSizeDiff.x, mTroughFrame.InternalSize.x, progress);
				mProgressFrame.GuiSize = new FixedSize(progressFrameSize, mTroughFrame.InternalSize.y);
			}
			else
			{
				float progressFrameSize = Mathf.Lerp(progressStyleSizeDiff.y, mTroughFrame.InternalSize.y, progress);
				mProgressFrame.GuiSize = new FixedSize(mTroughFrame.InternalSize.x, progressFrameSize);
			}
		}

		public override void Draw(IGuiContainer container, Vector2 position)
		{
			mTroughFrame.Draw(this, position);
		}

		public void UpdateChildPosition(IGuiElement childElement, Vector2 position)
		{
			throw new Exception("ProgressIndicators manage their children positions.");
		}

		public Vector2 GetChildPosition(IGuiElement childElement)
		{
			if (childElement == mTroughFrame)
			{
				return Vector2.zero;
			}
			throw new Exception("childElement is not a child of ProgressIndicator (" + this.Name + ")");
		}

		public IGuiPosition GetChildGuiPosition(IGuiElement childElement)
		{
			if (childElement == mTroughFrame)
			{
				return null;
			}
			throw new Exception("childElement is not a child of ProgressIndicator (" + this.Name + ")");
		}

		public System.Collections.Generic.IEnumerable<IGuiElement> Children
		{
			get { yield return mTroughFrame; }
		}

		public string ContainerName
		{
			get { return this.Name; }
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
	}
}

