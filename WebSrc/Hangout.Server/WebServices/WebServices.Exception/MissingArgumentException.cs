using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Server
{
    public class MissingArgumentException : HangoutException
    {
        public string ParamName    = "";
        public string ParamError   = "";

        public MissingArgumentException(string paramName, string paramError)
            : base( paramName + ":" + paramError )
        {
            this.ParamName  = paramName;
            this.ParamError = paramError;
            this.ErrorType  = RESTerror.ErrorType.MissingArgument;

            // default severity for this error type
            this.Severity = RESTerror.ErrorSeverity.Info;
        }

        public MissingArgumentException( string paramName, string paramError, RESTerror.ErrorSeverity Severity )
            : base( paramName + ":" + paramError, Severity )
        {
            this.ParamName  = paramName;
            this.ParamError = paramError;
            this.ErrorType  = RESTerror.ErrorType.MissingArgument;
        }

        public override RESTerror GetRestError( bool debugMode )
        {
            RESTerror e     = new RESTerror();
            e.ErrorCode     = this.ErrorType;
            e.ErrorMessage  = this.Message;
            e.ParamName     = this.ParamName;
            e.ParamError    = this.ParamError;

            if (debugMode)
            {
                e.StackTrace = this.StackTrace;
            }

            return e;
        }
    }
}
