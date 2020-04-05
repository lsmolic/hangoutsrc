using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
	public interface ILogger
	{
		/// <summary>
		/// Adds a reporter to the collection of reporters in this ILogger
		/// </summary>
		/// <returns>IReceipt that can be used to remove a reporter from this ILogger</returns>
		IReceipt AddReporter(ILogReporter reporter);

		/// <summary>
		/// Override equivalent to Log(message, LogLevel.Info)
		/// </summary>
		/// <param name="message">The message to be logged as 'Info'</param>
		void Log(string message);

		/// <summary>
		/// Logs the specified message at the specified logLevel
		/// </summary>
		/// <param name="message">The message to be logged at the provided LogLevel</param>
		/// <param name="logLevel">The LogLevel to use for this message</param>
		void Log(string message, LogLevel logLevel);

		/// <summary>
		/// Override equivalent to Log(message, LogLevel.Info) as a seperate line
		/// </summary>
		/// <param name="message">The message to be logged as 'Info'</param>
		void LogLine(string message);

		/// <summary>
		/// Logs the specified message at the specified logLevel as a separate line
		/// </summary>
		/// <param name="message">The message to be logged at the provided LogLevel</param>
		/// <param name="logLevel">The LogLevel to use for this message</param>
		void LogLine(string message, LogLevel logLevel);

		/// <summary>
		/// Flush actually sends the messages to the Reporters. If this isn't called externally, it will only be called when the Logger is collected
		/// </summary>
		void Flush();
	}
}
