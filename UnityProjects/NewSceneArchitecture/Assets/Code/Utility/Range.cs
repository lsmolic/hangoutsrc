/**  --------------------------------------------------------  *
 *   Range.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/14/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client
{

	public class Range<T>
	{
		private readonly T mLow;
		private readonly T mHigh;

		public T Low 
		{
			get { return mLow; }
		}

		public T High
		{
			get { return mHigh; }
		}

		public Range(T low, T high)
		{
			mLow = low;
			mHigh = high;
		}
	}

	public class Rangef : Range<float>
	{
		public static Rangef Parse(string rangeStr)
		{
			float min;
			float max;

			string[] split = rangeStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (split.Length == 2)
			{
				min = float.Parse(split[0]);
				max = float.Parse(split[1]);
			}
			else
			{
				max = float.Parse(split[0]);
				min = max;
			}

			return new Rangef(min, max);
		}

		public Rangef(float low, float high)
			: base(low, high)
		{
		}

		/// <summary>
		/// Gets the parametric value of this range evaluated at the given value
		/// </summary>
		public float Lerp(float t)
		{
			return (t * (High - Low)) + Low;
		}

		public float ParametricValue(float x)
		{
			float result = 0.0f;
			if( High != Low )
			{
				result = (x - Low) / (High - Low);
			}
			return result;
		}
		
		public float RandomValue()
		{
			return UnityEngine.Random.Range(this.Low, this.High);
		}

		public override string ToString()
		{
			return String.Format("Range(A: {0}, B: {1})", Low, High);
		}
	}
}