/**  --------------------------------------------------------  *
 *   RectUtility.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/18/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

namespace Hangout.Client
{
	public static class RectUtility
	{
		public static Rect Expand(Rect rect, float amount)
		{
			float doubleAmount = amount * 2.0f;
			
			rect.x -= amount;
			rect.y += amount;
			rect.width += doubleAmount;
			rect.height += doubleAmount;

			return rect;
		}

		public static Rect BoundingRect(Rect a, Rect b)
		{
			float xMin = Mathf.Min(a.xMin, b.xMin);
			float xMax = Mathf.Max(a.xMax, b.xMax);
			float yMin = Mathf.Min(a.yMin, b.yMin);
			float yMax = Mathf.Max(a.yMax, b.yMax);

			return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
		}

		public static Rect BoundingRect(Vector2 pnt, Rect rct)
		{
			float xMin = Mathf.Min(pnt.x, rct.xMin);
			float xMax = Mathf.Max(pnt.x, rct.xMax);
			float yMin = Mathf.Min(pnt.y, rct.yMin);
			float yMax = Mathf.Max(pnt.y, rct.yMax);

			return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
		}

		public static bool Contains(Rect outer, Rect inner)
		{
			return outer.Contains(new Vector2(inner.xMin, inner.yMin)) &&
				   outer.Contains(new Vector2(inner.xMax, inner.yMax)) &&
				   outer.Contains(new Vector2(inner.xMin, inner.yMax)) &&
				   outer.Contains(new Vector2(inner.xMax, inner.yMin));
		}

		public static bool Contains(Rect r, Vector2 pnt)
		{
			return pnt.x > r.x && pnt.x < (r.x + r.width) &&
				   pnt.y > r.y && pnt.y < (r.y + r.height);
		}

		public static Rect PixelIntersection(Rect a, Rect b)
		{
			float xMin = Mathf.Max(a.xMin, b.xMin);
			float xMax = Mathf.Min(a.xMax, b.xMax);
			float yMin = Mathf.Max(a.yMin, b.yMin);
			float yMax = Mathf.Min(a.yMax, b.yMax);
			return new Rect(xMin, yMin, xMax - xMin + 1, yMax - yMin + 1);
		}
		
		public static Rect Intersection(Rect a, Rect b)
		{
			float xMin = Mathf.Max(a.xMin, b.xMin);
			float xMax = Mathf.Min(a.xMax, b.xMax);
			float yMin = Mathf.Max(a.yMin, b.yMin);
			float yMax = Mathf.Min(a.yMax, b.yMax);
			return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
		}

		public static bool Overlaps(Rect a, Rect b)
		{
			return Contains(a, new Vector2(b.xMin, b.yMin)) ||
				   Contains(a, new Vector2(b.xMax, b.yMax)) ||
				   Contains(a, new Vector2(b.xMax, b.yMin)) ||
				   Contains(a, new Vector2(b.xMin, b.yMax)) ||
				   ((a.x == b.x && (a.width * b.width) > 0.0f) &&
					(a.y == b.y && (a.height * b.height) > 0.0f));
		}
	}
}