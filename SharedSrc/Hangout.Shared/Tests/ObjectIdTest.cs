using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Hangout.Shared.UnitTest
{
	[TestFixture]
	public class ObjectIdTest
	{
		private class MockObjectId : ObjectId
		{
			protected override string SerializationString
			{
				get { return "MockObjectId"; }
			}

			public MockObjectId(string roomIdString) : base(roomIdString)
			{
			}

			public MockObjectId(MockObjectId copyObject) : base(copyObject)
			{
			}

			public MockObjectId(uint idValue)
				: base(idValue)
			{
			}

			public MockObjectId(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}
		}

		[Test]
		public void ObjectEqualsTest()
		{
			MockObjectId mockObjectA5 = new MockObjectId(5);
			MockObjectId mockObjectB5 = new MockObjectId(5);

			Assert.IsTrue(mockObjectA5 == mockObjectB5, "5 equals 5");

			MockObjectId mockObjectANull = null;
			MockObjectId mockObjectBNull = null;

			Assert.IsTrue(mockObjectANull == mockObjectBNull, "null should be null");
		}

		[Test]
		public void ObjectNotEqualsTest()
		{
			MockObjectId mockObjectA5 = new MockObjectId(5);
			MockObjectId mockObjectB6 = new MockObjectId(6);

			Assert.IsTrue(mockObjectA5 != mockObjectB6, "5 does not equal 6");

			MockObjectId mockObjectANull = null;
			MockObjectId mockObjectB7 = new MockObjectId(7);

			Assert.IsTrue(mockObjectANull != mockObjectB7, "null does not equal 7");
		}
	}
}
