using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using System.Diagnostics;
using System.Reflection;

namespace Hangout.Server
{
	public static class StateServerAssert
	{
		public static void Assert(System.Exception ex)
		{
			StackTrace currentStackTrace = new StackTrace();
			MethodBase methodBase = currentStackTrace.GetFrame(1).GetMethod();
			LogManager.GetLogger(methodBase.ReflectedType.Name + "::" + methodBase.Name).Error("Exception: " + ex);
			ThrowInDebug(ex);
		}

		[Conditional("DEBUG")]
		private static void ThrowInDebug(Exception ex)
		{
			throw ex;
		}
	}
}
