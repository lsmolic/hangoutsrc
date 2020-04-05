using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Server {

    public class ContactImportException : HangoutException {
        public ContactImportException( string message )
            : base( message ) {
            this.ErrorType = RESTerror.ErrorType.ContactImport;

            // default severity for this error type
            this.Severity = RESTerror.ErrorSeverity.Info;
        }

        public ContactImportException( string message, RESTerror.ErrorSeverity Severity )
            : base( message, Severity ) {
            this.ErrorType = RESTerror.ErrorType.ContactImport;
        }
    }
}
