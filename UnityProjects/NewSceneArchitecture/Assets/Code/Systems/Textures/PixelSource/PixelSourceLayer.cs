/*
   Created by Vilas Tewari on 2009-08-03.
   
	A PixelSourceLayer encapsulates a PixelSource with other data like:
	a PixelBlendmode, offset and  mask
	
	LayeredTextures use PixelSourceLayers
*/

using System;
using UnityEngine;

namespace Hangout.Client
{
	public class PixelSourceLayer
	{
		/*
			What LayeredTexture do we belong to
			A PixelSourceLayer needs to notify the LayeredTexture if
			certain settings have been changed
		*/
		protected LayeredTexture mParent;

		/* The pixel data within this layer */
		protected PixelSource mPixelSource;

		/* How to process pixels sampled from the pixel source */
		protected PixelFilter mFilter;

		private Pixel.PixelBlendMode mPixelBlendMode;

		/* Offset will translate the pixelsource */
		protected int mOffsetX;
		protected int mOffsetY;

		/*
			Layer Accessor wraps the Pixel Source accessor but adds an offset
		*/
		public virtual HColor this[int x, int y]
		{
			get { return mFilter.FilterPixel(mPixelSource[x - mOffsetX, y - mOffsetY]); }
		}
		public PixelSource PixelSource
		{
			get { return mPixelSource; }
			set { mPixelSource = value; }
		}
		public Pixel.PixelBlendMode PixelBlendMode
		{
			get{ return mPixelBlendMode; }
			set{ mPixelBlendMode = value; }
		}
		public int OffsetX
		{
			get { return mOffsetX; }
			set { mOffsetX = value; }
		}
		public int OffsetY
		{
			get { return mOffsetY; }
			set { mOffsetY = value; }
		}
		public PixelFilter Filter
		{
			get { return mFilter; }
			set { mFilter = value; }
		}
		/*
			Constructor
		*/
		public PixelSourceLayer(LayeredTexture parent, PixelSource ps, Pixel.PixelBlendMode blendMode, int xOffset, int yOffset)
		{
			mPixelSource = ps;
			mPixelBlendMode = blendMode;
			mOffsetX = xOffset;
			mOffsetY = yOffset;
			mParent = parent;
			mFilter = new ClearFilter();
		}

		public bool IsPixelClear(int x, int y)
		{
			if (mPixelBlendMode == Pixel.PixelBlendMode.Layer)
				return mPixelSource.IsPixelClear(x - mOffsetX, y - mOffsetY);
			else
				return mPixelSource.IsPixelWhite(x - mOffsetX, y - mOffsetY);
		}

		public bool IndexInBounds(int x, int y)
		{
			return mPixelSource.IndexInBounds(x - mOffsetX, y - mOffsetY);
		}

		public void MarkAsDirty()
		{
			mParent.MarkAsDirty(this);
		}

		public override string ToString()
		{
			return "Layer: " + (mPixelSource == null ? "null" : mPixelSource.GetType().Name) + " " + mPixelBlendMode + " Offset(" + mOffsetX + "," + mOffsetY + ")";
		}
	}
}