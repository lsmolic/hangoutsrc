using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Hangout.Server;
using Hangout.Shared;

namespace Hangout.Server.WebServices
{
    public class CreateHangoutObjectCommand
    {
        public PaymentCommand GetUserBalance(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();

            cmd.Noun = "HangoutUsers";
            cmd.Verb = "GetUserBalance";

            UserId userIdObject = new UserId(userId);

            Type typeUserId  = typeof(UserId);
            bool test  = typeUserId.IsSerializable;

            MethodInfo[] methods = typeUserId.GetMethods();
           // cmd.Parameters.Add("userId", userId);

            return cmd;
        }

    }
}
