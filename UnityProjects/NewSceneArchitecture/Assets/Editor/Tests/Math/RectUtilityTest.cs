/**  --------------------------------------------------------  *
 *   RectUtilityTest.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/03/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

using Hangout.Client;
using Hangout.Shared.UnitTest;

namespace Hangout.Client.UnitTest
{
	[TestFixture]
	public class RectUtilityTest
	{
		[Test]
		public void MockRectWorksLikeUnityRect()
		{
			Rect rect = new Rect(1.0f, 2.0f, 3.0f, 4.0f);

			Assert.AreEqual(1.0f, rect.x);
			Assert.AreEqual(2.0f, rect.y);
			Assert.AreEqual(3.0f, rect.width);
			Assert.AreEqual(4.0f, rect.height);

			Assert.AreEqual(4.0f, rect.xMax);
			Assert.AreEqual(6.0f, rect.yMax);
			Assert.AreEqual(1.0f, rect.xMin);
			Assert.AreEqual(2.0f, rect.yMin);

			Assert.IsTrue(rect.Contains(new Vector2(2.5f, 4.0f)));
		}

		[Test]
		public void BoundingRectVerifivation()
		{
			Rect testRect1 = new Rect(0.0f, 0.0f, 30.0f, 30.0f);
			Rect testRect2 = new Rect(10.0f, 10.0f, 10.0f, 10.0f);

			Rect testRect3 = new Rect(10.0f, 10.0f, 100.0f, 100.0f);
			Vector2 testPnt1 = new Vector2(0.0f, 0.0f);
			Vector2 testPnt2 = new Vector2(-100.0f, 100.0f);

			Rect bounds1 = RectUtility.BoundingRect(testRect1, testRect2);
			Assert.AreEqual(0.0f, bounds1.xMin);
			Assert.AreEqual(0.0f, bounds1.yMin);
			Assert.AreEqual(30.0f, bounds1.xMax);
			Assert.AreEqual(30.0f, bounds1.yMax);

			Rect bounds2 = RectUtility.BoundingRect(bounds1, testRect3);
			Assert.AreEqual(0.0f, bounds2.x);
			Assert.AreEqual(0.0f, bounds2.y);
			Assert.AreEqual(110.0f, bounds2.width);
			Assert.AreEqual(110.0f, bounds2.height);

			Rect bounds3 = RectUtility.BoundingRect(testPnt1, testRect2);
			Assert.AreEqual(0.0f, bounds3.x);
			Assert.AreEqual(0.0f, bounds3.y);
			Assert.AreEqual(20.0f, bounds3.width);
			Assert.AreEqual(20.0f, bounds3.height);

			Rect bounds4 = RectUtility.BoundingRect(testPnt2, testRect1);
			Assert.AreEqual(-100.0f, bounds4.x);
			Assert.AreEqual(0.0f, bounds4.y);
			Assert.AreEqual(130.0f, bounds4.width);
			Assert.AreEqual(100.0f, bounds4.height);
		}

		[Test]
		public void ContainsVerifivation()
		{
			Rect testRect1 = new Rect(0.0f, 0.0f, 30.0f, 30.0f);
			Rect testRect2 = new Rect(10.0f, 10.0f, 10.0f, 10.0f);
			Rect testRect3 = new Rect(10.0f, 10.0f, 100.0f, 100.0f);
			Rect testRect4 = new Rect(-10.0f, 10.0f, 1.0f, 1.0f);

			Assert.IsTrue(RectUtility.Contains(testRect1, testRect2));  // Contains
			Assert.IsFalse(RectUtility.Contains(testRect1, testRect4)); // Outside
			Assert.IsFalse(RectUtility.Contains(testRect1, testRect3)); // Outside and smaller
			Assert.IsFalse(RectUtility.Contains(testRect2, testRect3)); // Smaller
			Assert.IsTrue(RectUtility.Contains(testRect3, testRect2));  // Borders
		}

		[Test]
		public void ContainsPointVerifivation()
		{
			Rect testRect1 = new Rect(0.0f, 0.0f, 30.0f, 30.0f);

			Assert.IsTrue(RectUtility.Contains(testRect1, new Vector2(1.0f, 1.0f)));    // Contains
			Assert.IsFalse(RectUtility.Contains(testRect1, new Vector2(30.0f, 10.0f))); // Borders
			Assert.IsFalse(RectUtility.Contains(testRect1, new Vector2(31.0f, 15.0f))); // Does not Contain
		}

		[Test]
		public void IntersectionVerification()
		{
			Rect testRect1 = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
			Rect testRect2 = new Rect(0.5f, 0.5f, 1.5f, 1.5f);

			Assert.AreEqual(new Rect(0.5f, 0.5f, 0.5f, 0.5f), RectUtility.Intersection(testRect1, testRect2));
			Assert.AreEqual(testRect1, RectUtility.Intersection(testRect1, testRect1));
		}

		[Test]
		public void OverlapsVerifivation()
		{
			Rect testRect1 = new Rect(32.0f, 0.0f, 32.0f, 36.0f);
			Rect testRect2 = new Rect(0.0f, 0.0f, 32.0f, 56.0f);
			Rect testRect3 = new Rect(1.0f, 1.0f, 1.0f, 1.0f);

			Assert.IsFalse(RectUtility.Overlaps(testRect1, testRect2), "Bordering rectangles should return false"); // Borders
			Assert.IsFalse(RectUtility.Overlaps(testRect1, testRect3), "Bordering rectangles should return false"); // No Overlap
			Assert.IsTrue(RectUtility.Overlaps(testRect2, testRect3), "Overlapping rectangles should return true"); // Overlaps
			Assert.IsTrue(RectUtility.Overlaps(testRect1, testRect1), "Overlapping rectangles should return true"); // Overlaps
		}

	}
}
