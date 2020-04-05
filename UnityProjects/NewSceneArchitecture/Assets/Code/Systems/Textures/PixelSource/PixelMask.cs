/*
	Created by Vilas Tewari
	
	A PixelStore is used to store the result of PixelSource Combinations
	It has dimensions of with and height
*/

using System;
using UnityEngine;
using System.Collections;

namespace Hangout.Client
{
	public class PixelMask
	{

		private int mHeight;
		private int mWidth;

		private bool[] mPixels;

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
		public bool[] Pixels
		{
			get { return mPixels; }
		}

		public bool this[int x, int y]
		{
			set { SetPixel(x, y, value); }
			get { return mPixels[ToArrayCoords(x, y)]; }
		}

		public PixelMask(int width, int height)
		{

			mWidth = width;
			mHeight = height;

			if (mWidth < 1 || mHeight < 1)
			{
				throw new ArgumentOutOfRangeException("A PixelStore cannot have 0 or negative dimensions");
			}

			mPixels = new bool[width * height];


		}

		public PixelMask(PixelMask copy)
		{
			mWidth = copy.mWidth;
			mHeight = copy.mHeight;

			mPixels = new bool[mWidth * mHeight];
			copy.mPixels.CopyTo(mPixels, 0);
		}

		public void Clear()
		{
			mPixels = new bool[mWidth * mHeight];
		}

		protected void SetPixel(int x, int y, bool pixel)
		{
			if (IndexInBounds(x, y))
			{
				mPixels[ToArrayCoords(x, y)] = pixel;
			}
		}

		protected int ToArrayCoords(int x, int y)
		{
			return (y * Width) + x;
		}

		protected bool IndexInBounds(int x, int y)
		{
			return (x >= 0 && x < Width && y >= 0 && y < Height);
		}
	}
}