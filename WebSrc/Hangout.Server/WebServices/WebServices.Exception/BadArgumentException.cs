using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Server
{
    public class BadArgumentException : HangoutException
    {
        public string ParamName    = "";
        public string ParamError   = "";

        public BadArgumentException( string paramName, string paramError )
            : base( paramName + ":" + paramError )
        {
            this.ParamName  = paramName;
            this.ParamError = paramError;
            this.ErrorType  = RESTerror.ErrorType.BadArgumentValue;

            // default severity for this error type
            this.Severity   = RESTerror.ErrorSeverity.Info;
        }

        public BadArgumentException( string paramName, string paramError, RESTerror.ErrorSeverity Severity )
            : base( paramName + ":" + paramError, Severity )
        {
            this.ParamName  = paramName;
            this.ParamError = paramError;
            this.ErrorType  = RESTerror.ErrorType.BadArgumentValue;
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
