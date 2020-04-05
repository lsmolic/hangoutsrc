/**  --------------------------------------------------------  *
 *   ObjectRepositoryTest.cs  
 *
 *   Author: Joe Shochet, Hangout Industries
 *   Date: 08/31/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Shared.UnitTest
{

    public class DistributedTestObject : DistributedObject
    {
        public DistributedTestObject(DistributedObjectId doId) : base(doId) { }
        protected override void RegisterMessageActions() { }
        public override void ProcessMessage(Message msg) { }
		public override void Dispose() { }
    }

    [TestFixture]
    public class ObjectRepositoryTest
    {

        [Test]
        public void AddAndRemoveObject()
        {
            ObjectRepository objRepo = new ObjectRepository();

            // Create a distributed object to play with
            DistributedObjectId doId = new DistributedObjectId(100);
            DistributedTestObject obj = new DistributedTestObject(doId);

            // Object should not be present
            Assert.IsFalse(objRepo.ContainsObject(obj));
            Assert.IsFalse(objRepo.ContainsObject(doId));
            Assert.IsTrue(objRepo.GetObject(doId) == null);

            // Add obj
            objRepo.AddObject(obj);
            Assert.IsTrue(objRepo.ContainsObject(obj));
            Assert.IsTrue(objRepo.ContainsObject(doId));
			Assert.AreEqual((IDistributedObject)objRepo.GetObject(doId), (IDistributedObject)obj);

            // Remove obj
            objRepo.RemoveObject(obj);
            Assert.IsFalse(objRepo.ContainsObject(obj));

            // Try to remove an object already removed
            objRepo.RemoveObject(obj);
        }

    }
}
