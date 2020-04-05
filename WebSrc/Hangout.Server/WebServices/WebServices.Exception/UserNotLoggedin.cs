using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Server
{
    public class UserNotLoggedin : HangoutException
    {
        public UserNotLoggedin( string message )
            : base( message ) 
        { 
            this.ErrorType = RESTerror.ErrorType.UserNotLoggedin;

            // default severity for this error type
            this.Severity = RESTerror.ErrorSeverity.Warming;
        }

        public UserNotLoggedin( string message, RESTerror.ErrorSeverity Severity )
            : base( message, Severity ) 
        {
            this.ErrorType = RESTerror.ErrorType.UserNotLoggedin; 
        }
    }
}
