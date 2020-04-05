using System;
using System.Collections.Generic;
using System.Text;

using PureMVC.Patterns;

namespace Hangout.Client
{
    public class LocalAvatarProxy : Proxy
    {
        private readonly LocalAvatarDistributedObject mLocalAvatar;
        public LocalAvatarDistributedObject LocalAvatarDistributedObject
        { 
            get {return mLocalAvatar;}
        }
        public LocalAvatarProxy(LocalAvatarDistributedObject localAvatar)
        {
            mLocalAvatar = localAvatar;
        }
        
        public void SaveDna()
        {
			mLocalAvatar.SaveDna();
        }
    }
}
