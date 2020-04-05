using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Server
{
    public class ZoneIdManager
    {
        private uint mZoneIdGenerator = 1000;

        public ZoneIdManager()
        {
        }

        public ZoneId GetNewZoneId()
        {
            return new ZoneId(++mZoneIdGenerator);
        }

    }
}
