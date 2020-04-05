/*
	Created by Vilas Tewari
	
	A PixelSource is a class that takes indicies x and y and returns a pixel ( Color )
	It has dimensions of with and height. You cannot change pixels ( Set pixels ) in a source
	
	PixelSources can be combined with other PixelSources and stored in a PixelStore
	using the static 
*/


using UnityEngine;
using System;

namespace Hangout.Client
{
	public abstract class PixelSource
	{
		private int mHeight;
		private int mWidth;

		public int Width
		{
			get { return mWidth; }
		}
		public int Height
		{
			get { return mHeight; }
		}
		public int Length
		{
			get { return mHeight * mWidth; }
		}

		public HColor this[int x, int y]
		{
			get
			{
				if (!IndexInBounds(x, y))
					return new HColor(0, 0, 0, 0);
				else
					return GetPixel(x, y);
			}
		}

		public PixelSource(int width, int height)
		{
			mWidth = width;
			mHeight = height;

			if (mWidth < 1 || mHeight < 1)
			{
				throw new ArgumentOutOfRangeException("A PixelSource cannot have 0 or negative dimensions");
			}
		}

		public override string ToString()
		{
			string result = "Pixel Layer:\n";

			for (int x = 0; x < mWidth; ++x)
			{
				for (int y = 0; y < mHeight; ++y)
					result += this[x, y].ToString() + " ";

				result += "\n";
			}
			return result;
		}

		/*
			Is this Pixel clear i.e. should we ignore it
		*/
		public abstract bool IsPixelClear(int x, int y);

		/*
			Is this Pixel clear i.e. should we ignore it for Multiply
		*/
		public abstract bool IsPixelWhite(int x, int y);

		/*
			Is Index in Bounds
		*/
		public bool IndexInBounds(int x, int y)
		{
			return (x >= 0 && x < Width && y >= 0 && y < Height);
		}

		/*
			Get Pixel is the heart of PixelSource Class. GetPixel could return pixels based on a texture, algorithm, solid color etc..
		*/
		protected abstract HColor GetPixel(int x, int y);

		/*
			[x, y] to linear array co-ordinates
		*/
		protected int ToArrayCoords(int x, int y)
		{
			return (y * Width) + x;
		}
	}
}