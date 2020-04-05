using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Server
{
    public class UserNotValidatedException : HangoutException
    {
        public UserNotValidatedException( string message )
            : base( message ) 
        {
            this.ErrorType = RESTerror.ErrorType.UserNotValidated;

            // default severity for this error type
            this.Severity = RESTerror.ErrorSeverity.Info;
        }

        public UserNotValidatedException( string message, RESTerror.ErrorSeverity Severity )
            : base( message, Severity ) 
        {
            this.ErrorType = RESTerror.ErrorType.UserNotValidated; 
        }
    }
}
