using System;
using System.Collections.Generic;

using UnityEngine;

using Hangout.Shared.UnitTest;

namespace Hangout.Shared
{
    [TestFixture]
    public class LinearAlgebraTest
    {
		[Test]
		public void NearestPointOnLineSegment2dVerification()
		{
			Vector2 a = new Vector2(0.0f, 5.0f);
			Vector2 b = new Vector2(5.0f, 0.0f);

			Vector2 pnt = new Vector2(0.0f, 0.0f);

			Vector2 expected = new Vector2(2.5f, 2.5f);

			// Normal case
			UnityAssert.AreClose(expected, LinearAlgebra.NearestPointOnLineSegment(a, b, pnt), 0.0001f);

			// Edge cases
			Vector2 ones = new Vector2(1.0f, 1.0f);
			Vector2 negOnes = new Vector2(-1.0f, -1.0f);
			UnityAssert.AreClose(Vector3.zero, LinearAlgebra.NearestPointOnLineSegment(Vector2.zero, ones, negOnes), 0.0001f);
			UnityAssert.AreClose(Vector3.zero, LinearAlgebra.NearestPointOnLineSegment(Vector2.zero, Vector2.zero, ones), 0.0001f);
		}

		[Test]
		public void NearestPointOnLineSegment3dVerification()
		{
			Vector3 a = new Vector3(0.0f, 0.0f, 5.0f);
			Vector3 b = new Vector3(5.0f, 0.0f, 0.0f);

			Vector3 pnt = new Vector3(0.0f, 0.0f, 0.0f);

			Vector3 expected = new Vector3(2.5f, 0.0f, 2.5f);

			// Normal case
			UnityAssert.AreClose(expected, LinearAlgebra.NearestPointOnLineSegment(a, b, pnt), 0.0001f);

			// Edge cases
			Vector3 ones = Vector3.one;
			Vector3 negOnes = -1.0f * ones;
			UnityAssert.AreClose(Vector3.zero, LinearAlgebra.NearestPointOnLineSegment(Vector3.zero, ones, negOnes), 0.0001f);
			UnityAssert.AreClose(Vector3.zero, LinearAlgebra.NearestPointOnLineSegment(Vector3.zero, Vector3.zero, ones), 0.0001f);
		}
    }
}