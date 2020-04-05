using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine
{
	public class Time
	{
		// Mock data:
		private Dictionary<string, int> m_functionCallCounts;
		public Dictionary<string, int> FunctionCallCounts
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				return m_functionCallCounts;
			}
		}

		public Time()
		{
			//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
			if (!m_functionCallCounts.ContainsKey("Void .ctor()"))
			{
				m_functionCallCounts.Add("Void .ctor()", 0);
			}
			m_functionCallCounts["Void .ctor()"]++;

		}

		public static System.Single time
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public static System.Single timeScale
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
			}
		}

		public static System.Single deltaTime
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public static System.Int32 frameCount
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public static System.Single smoothDeltaTime
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public static System.Int32 renderedFrameCount
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public static System.Single fixedDeltaTime
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
			}
		}

		public static System.Single fixedTime
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public static System.Single timeSinceLevelLoad
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		private static DateTime mStart = DateTime.Now;
		public static System.Single realtimeSinceStartup
		{
			get
			{
				TimeSpan timeSinceStart = DateTime.Now - mStart;
				return ((float)timeSinceStart.Ticks) / 10000000;
			}
		}

		public static System.Int32 captureFramerate
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
			}
		}
	}
}
