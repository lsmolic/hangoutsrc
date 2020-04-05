using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Hangout.Shared.UnitTest
{
	public class AssertionFailedException : System.Exception
	{
		private string mFilename;
		private string mFunc;
		private int mLineNumber;

		public AssertionFailedException(string message, string function, string file, int line)
			: base(message)
		{
			mFunc = function;
			mFilename = file;
			mLineNumber = line;
		}

		public string File
		{
			get { return mFilename; }
		}

		public int Line
		{
			get { return mLineNumber; }
		}

		public string Function
		{
			get { return mFunc; }
		}
	}

	public static class Assert
	{
		private static List<string> mDontFailAgain = null;

		private static void AddSuppressFail(string failMessage)
		{
			if (mDontFailAgain == null)
			{
				mDontFailAgain = new List<string>();
			}

			if (!mDontFailAgain.Contains(failMessage))
			{
				mDontFailAgain.Add(failMessage);
			}
		}

		private static bool IsAssertionSuppressed(string failMessage)
		{
			if (mDontFailAgain != null)
			{
				return mDontFailAgain.Contains(failMessage);
			}
			return false;
		}

		public static void FailOnce(string failMessage)
		{
			if (IsAssertionSuppressed(failMessage))
			{
				Fail(failMessage);
			}
		}
		public static void Fail()
		{
			Fail("No Failure message");
		}
		public static void Fail(string failMessage)
		{
			if (IsAssertionSuppressed(failMessage))
			{
				// If this message is suppressed, don't do anything for it
				return;
			}

			// Find the function that called Assert
			StackTrace stackTrace = new StackTrace();
			StackFrame thisFunctionsFrame = stackTrace.GetFrame(0);
			StackFrame functionThatEnteredAssert = null;
			foreach( StackFrame frame in stackTrace.GetFrames() )
			{
				if (!IsATestMethod(frame.GetMethod()))
				{
					functionThatEnteredAssert = frame;
					break;
				}
			}

			throw new AssertionFailedException
			(
				failMessage, 
				functionThatEnteredAssert.GetMethod().Name, 
				functionThatEnteredAssert.GetFileName(), 
				functionThatEnteredAssert.GetFileLineNumber()
			);
		}
		private static bool IsATestMethod(System.Reflection.MethodBase method)
		{
			foreach (Attribute attrib in Attribute.GetCustomAttributes(method))
			{
				if( attrib.GetType() == typeof(Test) )
				{
					return true;
				}
			}
			return false;
		}
		public static void IsNotNull(object o, string message)
		{
			if (o == null)
			{
				Fail(message);
			}
		}
		public static void IsNotNull(object o)
		{
			IsNotNull(o, "Argument is null");
		}
		public static void IsNull(object o)
		{
			IsNull(o, "Expected: null");
		}
		public static void IsNull(object o, string message)
		{
			if (o != null)
			{
				Fail(message);
			}
		}
		public static void IsTrue(bool Condition)
		{
			IsTrue(Condition, "Assert.IsTrue failed");
		}
		public static void IsTrue(bool Condition, string message)
		{
			if (!Condition)
			{
				Fail(message);
			}
		}
		public static void IsFalse(bool Condition)
		{
			IsFalse(Condition, "Assert.IsFalse failed");
		}
		public static void IsFalse(bool Condition, string message)
		{
			if (Condition)
			{
				Fail(message);
			}
		}
		public static void AreEqual<T>(T expected, T actual)
		{
			AreEqual(expected, actual, "Expected: " + expected + ", Actual: " + actual);
		}
		public static void AreEqual<T>(T expected, T actual, string message)
		{
			if (!expected.Equals(actual))
			{
				Fail(message);
			}
		}
		public static void ArraysAreEqual<T>(IEnumerable<T> arrayExpected, IEnumerable<T> arrayActual)
		{
			List<T> expected = new List<T>(arrayExpected);
			List<T> actual = new List<T>(arrayActual);
			Assert.AreEqual(expected.Count, actual.Count, "Arrays are different lengths");
			for (int i = 0; i < expected.Count; ++i)
			{
				Assert.AreEqual(expected[i], actual[i], "Element " + i + ": Expected: " + expected[i].ToString() + ", Actual: " + actual[i].ToString());
			}
		}
		public static void AreNotEqual<T>(T expected, T actual)
		{
			AreNotEqual(expected, actual, "Both arguments are equal: " + expected);
		}
		public static void AreNotEqual<T>(T expected, T actual, string message)
		{
			if (expected.Equals(actual))
			{
				Fail(message);
			}
		}
		public static void AreNotEqual(int expected, int actual, string message)
		{
			if (expected == actual)
			{
				Fail(message);
			}
		}
		public static void AreNotEqual(float expected, float actual, string message)
		{
			if (expected == actual)
			{
				Fail(message);
			}
		}
		public static void AreNotEqual(string expected, string actual, string message)
		{
			if (expected == actual)
			{
				Fail(message);
			}
		}

		public static void IsNotWithin(float expected, float actual, float epsilon)
		{
			IsNotWithin(expected, actual, epsilon, "Expected: > " + (expected + epsilon) + " or < " + (expected - epsilon) + " Actual: " + actual);
		}

		public static void IsNotWithin(float expected, float actual, float epsilon, string message)
		{
			if ((expected + epsilon) > actual && (expected - epsilon) < actual)
			{
				Fail(message);
			}
		}
		public static void IsWithin(float expected, float actual, float epsilon)
		{
			IsWithin(expected, actual, epsilon, "Expected: " + expected + " +- " + epsilon + " Actual: " + actual);
		}
		public static void IsWithin(float expected, float actual, float epsilon, string message)
		{
			if ((expected + epsilon) < actual || (expected - epsilon) > actual)
			{
				Fail(message);
			}
		}
		public static void IsEmptyString(string s)
		{
			IsEmptyString(s, "Expected '" + s + "' to be an empty string");
		}
		public static void IsEmptyString(string s, string message)
		{
			if (!string.IsNullOrEmpty(s))
			{
				Fail(message);
			}
		}
		public static void IsNotEmptyString(string s)
		{
			IsNotEmptyString(s, "String is an empty string.");
		}
		public static void IsNotEmptyString(string s, string message)
		{
			if (string.IsNullOrEmpty(s))
			{
				Fail(message);
			}
		}
		public static void ContainsString(string data, string searchStr, string message)
		{
			if (!data.Contains(searchStr))
			{
				Fail(message);
			}
		}
		public static void ContainsString(string data, string searchStr)
		{
			ContainsString(data, searchStr, "String '" + data + "' does not contain string '" + searchStr + "'");
		}
	}
}
