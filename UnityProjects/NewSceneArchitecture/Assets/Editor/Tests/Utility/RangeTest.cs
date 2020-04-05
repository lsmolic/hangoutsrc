/**  --------------------------------------------------------  *
 *   RangeTest.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/12/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

using Hangout.Client;
using Hangout.Shared.UnitTest;

namespace Hangout.Client.UnitTest
{
	[TestFixture]
	public class RangeTest
	{
		[Test]
		public void RangefParseVerification()
		{
			string rangeString = "-12098.43 234987.4293";
			string rangeString2 = "0";

			Rangef range1 = Rangef.Parse(rangeString);
			Rangef range2 = Rangef.Parse(rangeString2);

			Assert.IsWithin(-12098.43f, range1.Low, 0.01f);
			Assert.IsWithin(234987.4293f, range1.High, 0.1f);

			Assert.AreEqual(range2.Low, range2.High);
		}

		[Test]
		public void RangefLerpVerification()
		{
			Rangef range = new Rangef(0.0f, 2.0f);
			Rangef range2 = new Rangef(0.0f, 10.0f);

			Assert.IsWithin(1.0f, range.Lerp(0.5f), 0.00001f);
			Assert.IsWithin(-1.0f, range.Lerp(-0.5f), 0.00001f);

			Assert.IsWithin(10.0f, range2.Lerp(1.0f), 0.00001f);
			Assert.IsWithin(20.0f, range2.Lerp(2.0f), 0.00001f);
		}

		[Test]
		public void RangefParametricValueVerification()
		{
			Rangef range = new Rangef(44.0f, 300.0f);

			Assert.IsWithin(0.0f, range.ParametricValue(44.0f), 0.00001f);
			Assert.IsWithin(1.0f, range.ParametricValue(300.0f), 0.00001f);
			Assert.IsWithin(0.5f, range.ParametricValue(172.0f), 0.00001f);
		}
	}

}
