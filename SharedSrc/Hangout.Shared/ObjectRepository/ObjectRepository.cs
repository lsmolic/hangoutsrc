using System;
using System.Collections;
using System.Collections.Generic;
using Hangout.Shared;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Hangout.Shared {
    public class ObjectRepository {
        // The object repository is responsible for:
        //   Maintaining a centralized place where you can find an object given its ID

        // Maps object ids to the actual objects currently known in memory
        protected Dictionary<DistributedObjectId, IDistributedObject> mObjectIds = new Dictionary<DistributedObjectId, IDistributedObject>();

        // Add this DistributedObject to the repository.
        public void AddObject(IDistributedObject obj)
        {
            DistributedObjectId doId = obj.DistributedObjectId;
            // Store in the dictionary of objects
            mObjectIds[doId] = obj;
        }

        // Remove this DistributedObject from the repository.
        public virtual void RemoveObject(IDistributedObject obj) 
		{
            // Remove from object id dict
            DistributedObjectId doId = obj.DistributedObjectId;
            if (!mObjectIds.Remove(doId))
            {
                Console.WriteLine("Warning!!! We don't have this object!");
                return;
            }

        }

        // Returns true if this state server currently is maintaining this object in memory.
        public bool ContainsObject(DistributedObjectId doId) {
            return mObjectIds.ContainsKey(doId);
        }

        // Returns true if this state server currently is maintaining this object in memory.
        public bool ContainsObject(DistributedObject obj) {
            return mObjectIds.ContainsKey(obj.DistributedObjectId);
        }

        /// <summary>
        ///  Returns the object referenced by the doId or null if not found on this state server.
        /// </summary>
        /// <param name="doId"></param>
        /// <returns></returns>
        public IDistributedObject GetObject(DistributedObjectId doId) {
            IDistributedObject obj = null;
            mObjectIds.TryGetValue(doId, out obj);
            return obj;
        }
    }
}