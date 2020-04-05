using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Server
{
    //
    // Errors are returned from REST services by setting the HTTP error code to 404 and XML
    // serializing an instance of the following class to the result stream
    //
    public class RESTerror
    {
        public enum ErrorSeverity
        {
            Info = 1,
            Warming = 2,
            Error = 3,
            Critial = 4
        };

        public enum ErrorType 
		{ 
            BadArgumentValue = 1, 
            MissingArgument = 2,
            UserNotLoggedin = 3,
            NoDataExists = 4,
            UserNotValidated = 5, 
            ContactImport = 6,
            RoomNotEditable = 7,
            Unknown = 0 
        };

		private ErrorType _ErrorCode;

		public ErrorType ErrorCode
		{
			get { return _ErrorCode; }
			set { _ErrorCode = value; }
		}

		private string _ErrorMessage;

		public string ErrorMessage
		{
			get { return _ErrorMessage; }
			set { _ErrorMessage = value; }
		}

		private string _InnerException;

		public string InnerException
		{
			get { return _InnerException; }
			set { _InnerException = value; }
		}

		private string _ParamError;

		public string ParamError
		{
			get { return _ParamError; }
			set { _ParamError = value; }
		}

		private string _ParamName;

		public string ParamName
		{
			get { return _ParamName; }
			set { _ParamName = value; }
		}
	
		private string _StackTrace;

		public string StackTrace
		{
			get { return _StackTrace; }
			set { _StackTrace = value; }
		}

    }

}
