using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Server
{
    public class HangoutException : System.Exception
    {
        public RESTerror.ErrorSeverity  Severity  = RESTerror.ErrorSeverity.Error;
        public RESTerror.ErrorType      ErrorType = RESTerror.ErrorType.Unknown;

        // additional info about the exception	
        public string   RequestType = "";
        public string   RequestUrl = "";
        public string   ClientIp = "";
        public int      CurrentUserId = 0;
        public string   MachineName = "";
        public DateTime TimeStamp = DateTime.Now;
		private string mStackTrace = "";

        #region Overriding properties

        public override string Message
        {
            get
            {
                if (InnerException != null)
                    return InnerException.Message;
                else
                    return base.Message;
            }
        }

        public override string StackTrace
        {
            get
            {
				return mStackTrace;
            }
        }

        public override string Source
        {
            get
            {
                if (InnerException != null)
                    return InnerException.Source;
                else
                    return base.Source;
            }
            set
            {
                if (InnerException != null)
                    InnerException.Source = value;
                else
                    base.Source = value;
            }
        }

        #endregion

        public HangoutException( string message ) : base( message ) { }

        // this constructor is for carrying a unrecognized exception to the exception handler
		public HangoutException(System.Exception exception) : base(exception.Message, exception.InnerException) 
		{
			mStackTrace = exception.StackTrace;
		}

        public HangoutException( string message, RESTerror.ErrorSeverity Severity )
            : base( message )
        {
            this.Severity = Severity;
        }

        virtual public RESTerror GetRestError( bool debugMode ) 
        {
            RESTerror e = new RESTerror();
            //e.ErrorCode = ErrorType;
			e.InnerException = base.Message;
            e.ErrorMessage = this.Message;

            if (debugMode)
            {
                e.StackTrace = this.StackTrace;
            }

            return e;
        }
    }
}
