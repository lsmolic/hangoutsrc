/**  --------------------------------------------------------  *
 *   FashionGameProxy.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/01/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;

using UnityEngine;

namespace Hangout.Shared
{
	public static class LinearAlgebra
	{
		public static Vector3 Mult3(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public static Vector3 Div3(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
		}

		public static Vector3 NearestPointOnLineSegment(Vector3 a, Vector3 b, Vector3 pnt)
		{
			float bxMinusAx = b.x - a.x;
			float byMinusAy = b.y - a.y;
			float bzMinusAz = b.z - a.z;

			float rNumerator = (pnt.x - a.x) * bxMinusAx + (pnt.y - a.y) * byMinusAy + (pnt.z - a.z) * bzMinusAz;
			float rDenomenator = bxMinusAx * bxMinusAx + byMinusAy * byMinusAy + bzMinusAz * bzMinusAz;

			float r = rNumerator / rDenomenator;

			Vector3 result;
			if ((r >= 0) && (r <= 1))
			{
				result = Vector3.Lerp(a, b, r);
			}
			else
			{
				float distA = (pnt - a).magnitude;
				float distB = (pnt - b).magnitude;
				if (distA < distB)
				{
					result = a;
				}
				else
				{
					result = b;
				}
			}

			return result;
		}

		public static Vector2 NearestPointOnLineSegment(Vector2 a, Vector2 b, Vector2 pnt)
		{
			float bxMinusAx = b.x - a.x;
			float byMinusAy = b.y - a.y;

			float rNumerator = (pnt.x - a.x) * bxMinusAx + (pnt.y - a.y) * byMinusAy;
			float rDenomenator = bxMinusAx * bxMinusAx + byMinusAy * byMinusAy;
			float r = rNumerator / rDenomenator;

			Vector2 result;
			if ((r >= 0) && (r <= 1))
			{
				result = new Vector2(Mathf.Lerp(a.x, b.x, r), Mathf.Lerp(a.y, b.y, r));
			}
			else
			{
				float distA = (pnt - a).magnitude;
				float distB = (pnt - b).magnitude;
				if (distA < distB)
				{
					result = a;
				}
				else
				{
					result = b;
				}
			}

			return result;
		}
	}
}
