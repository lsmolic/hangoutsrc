using UnityEngine;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared.UnitTest
{
	public static class UnityAssert
	{
		// AreClose for Vector3 also works for Vector2 due to the implicit operator
		public static void AreClose(Vector3 expected, Vector3 actual, float epsilon, string message)
		{
			if( (expected - actual).magnitude > epsilon )
			{
				Assert.Fail(message);
			}
		}
		public static void AreClose(Vector3 expected, Vector3 actual, float epsilon)
		{
			AreClose(expected, actual, epsilon, "Expected " + expected + " was more than " + epsilon + " from " + actual);
		}
	}
}
