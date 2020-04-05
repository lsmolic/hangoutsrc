using System;
using System.Collections.Specialized;
using System.Web;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Hangout.Server
{
	[ConfigurationElementType( typeof( CustomHandlerData ) )]
	public class SendNotificationEmailExceptionHandler : IExceptionHandler
	{
        private static readonly string FromAddress = ConfigurationManager.AppSettings["SupportEmail"];
        private static readonly string ToAddress = ConfigurationManager.AppSettings["WebTeamEmail"];

		public SendNotificationEmailExceptionHandler( NameValueCollection ignore )
		{
		}

		public System.Exception HandleException( System.Exception exception, Guid handlingInstanceId )
		{
			//ExceptionPolicy.HandleException( exception, "Exception Policy" );

			HangoutException hangoutException = exception as HangoutException;
			if ( hangoutException != null )
				SendSupportEmail( hangoutException );
			else
				SendSupportEmail( exception );

			return exception;
		}

		public static bool HandleUnityException( string errorMessage, string stackTrace )
		{
			// Send support email
			MailAddress fromAddress = new MailAddress( FromAddress );
			MailAddress toAddress = new MailAddress( ToAddress );

			MailMessage mail = new MailMessage( fromAddress, toAddress );
			mail.Subject = "Unity System Exception";

			StringBuilder message = new StringBuilder();

			message.AppendLine( String.Format( "Message: {0}", errorMessage ) );
			message.AppendLine( String.Format( "Stack Trace: {0}", stackTrace ) );

			mail.Body = message.ToString();

			SmtpClient smtpClient = new SmtpClient();
			smtpClient.Send( mail );

			return true;
		}

		private void SendSupportEmail( System.Exception exception )
		{
			// Send support email
			MailAddress fromAddress = new MailAddress( FromAddress );
			MailAddress toAddress = new MailAddress( ToAddress );

			MailMessage mail = new MailMessage( fromAddress, toAddress );
			mail.Subject = "System Exception";

			StringBuilder message = new StringBuilder();

			message.AppendLine( String.Format( "Message: {0}", exception.Message ) );
			message.AppendLine( String.Format( "Source: {0}", exception.Source ) );
			if ( exception.InnerException != null )
			{
				message.AppendLine( String.Format( "Inner Exception: {0}", exception.InnerException.Message ) );
			}
			message.AppendLine( String.Format( "Stack Trace: {0}", exception.StackTrace ) );

			mail.Body = message.ToString();

			SmtpClient smtpClient = new SmtpClient();
			smtpClient.Send( mail );
		}

		private void SendSupportEmail( HangoutException exception )
		{
			// ignore the non-critial errors
			if ( exception.Severity != RESTerror.ErrorSeverity.Error && exception.Severity != RESTerror.ErrorSeverity.Critial )
			{
				return;
			}

			// Send support email
			MailAddress fromAddress = new MailAddress( FromAddress );
			MailAddress toAddress = new MailAddress( ToAddress );

			MailMessage mail = new MailMessage( fromAddress, toAddress );
			mail.Subject = "System Exception";

			StringBuilder message = new StringBuilder();

			message.AppendLine( String.Format( "Type: {0}", exception.ErrorType.ToString() ) );
			message.AppendLine( String.Format( "Severity: {0}", exception.Severity.ToString() ) );
			message.AppendLine( String.Format( "Request Type: {0}", exception.RequestType ) );
			message.AppendLine( String.Format( "Request: {0}", exception.RequestUrl ) );
			message.AppendLine( String.Format( "User Host: {0}", exception.ClientIp ) );
			message.AppendLine( String.Format( "Server Host: {0}", exception.MachineName ) );
			message.AppendLine( String.Format( "UserId: {0}", exception.CurrentUserId.ToString() ) );
			message.AppendLine( String.Format( "Time: {0}", exception.TimeStamp.ToString() ) );
			message.AppendLine( String.Format( "Message: {0}", exception.Message ) );
			message.AppendLine( String.Format( "Source: {0}", exception.Source ) );
			if ( exception.InnerException != null )
			{
				message.AppendLine( String.Format( "Inner Exception: {0}", exception.InnerException.Message ) );
			}
			message.AppendLine( String.Format( "Stack Trace: {0}", exception.StackTrace ) );

			mail.Body = message.ToString();

			SmtpClient smtpClient = new SmtpClient();
			smtpClient.Send( mail );
		}

	}
}
