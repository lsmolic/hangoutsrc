using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
	public class DateTimeYieldForSeconds : IYieldInstruction
	{
		public static int mBGWMainCounter = 0;

		private readonly DateTime mTimeComplete;

		public DateTimeYieldForSeconds(float seconds)
		{
			mTimeComplete = DateTime.Now + new TimeSpan((long)(seconds * 10000000.0f));
		}
		
		public bool Ready
		{
			get 
			{
				return DateTime.Now >= mTimeComplete;
			}
		}

	}
}
