using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
    //WARNING!!! do NOT rearrange this enum because these are stored as ints on the database!
    public enum PrivacyLevel
    {
        Default = -1,
        Public = 0,
        Private = 1
    }
}
