using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Server
{
	public class NegativeBalanceException : HangoutException
	{
        public NegativeBalanceException( string message )
            : base( message ) 
        { 
            this.ErrorType = RESTerror.ErrorType.NoDataExists;

            // default severity for this error type
            this.Severity = RESTerror.ErrorSeverity.Info;
        }

		public NegativeBalanceException( string message, RESTerror.ErrorSeverity Severity )
            : base( message, Severity ) 
        { 
            this.ErrorType = RESTerror.ErrorType.NoDataExists; 
        }
	}
}
