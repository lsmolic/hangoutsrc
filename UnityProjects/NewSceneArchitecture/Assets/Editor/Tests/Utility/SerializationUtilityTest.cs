/**  --------------------------------------------------------  *
 *   SerializationUtility.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/21/2009
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
	public class SerializationUtilityTest
	{
		[Test]
		public void Vector3ToStringVerification()
		{
			// Test that whatever these values round to is preserved thru the serialization
			Vector3 original = new Vector3(17.25340897239867045987648f, -42.9992342154325342523454545444444111110001f, 0.0f);
			string serialization = SerializationUtility.ToString(original);
			Vector3 serialized = SerializationUtility.ToVector3(serialization);

			Assert.AreEqual(original, serialized);
		}

		
	}

}
