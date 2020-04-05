using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
    public interface IAccount
    {
        List<FacebookFriendInfo> HangoutFacebookFriends{ get; }
        long FacebookAccountId { get; }
        AccountId AccountId { get; }
        string Nickname { get;  }
    }
}
