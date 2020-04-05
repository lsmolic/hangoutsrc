/**  --------------------------------------------------------  *
 *   ScreenEdgeTest.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/15/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

namespace Hangout.Client
{
	/// Tests if a set of coordinates are within the global Screen
	public struct ScreenEdgeTest
	{
		/// true means 'in bounds', false is 'out of bounds'
		private bool mLeft;
		private bool mRight;
		private bool mTop;
		private bool mBottom;

		public bool Left
		{
			get { return mLeft; }
		}
		public bool Right
		{
			get { return mRight; }
		}
		public bool Top
		{
			get { return mTop; }
		}
		public bool Bottom
		{
			get { return mBottom; }
		}

		public bool IsAnySideOut()
		{
			return !(this.Left && this.Right && this.Top && this.Bottom);
		}

		public bool IsEitherDimensionLargerThanScreen()
		{
			return (!this.Left && !this.Right) || (!this.Top && !this.Bottom);
		}

		public ScreenEdgeTest(Rect coordinates)
		{
			mTop = coordinates.y >= 0;
			mRight = coordinates.xMax <= Screen.width;
			mLeft = coordinates.x >= 0;
			mBottom = coordinates.yMax <= Screen.height;
		}
	}
}