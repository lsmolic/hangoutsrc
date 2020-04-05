using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Server
{
	public static class HangoutConversions
	{
		private static uint kConversionRate = 3;

		public static uint MiniGameExperienceToVCoin(uint experiencePoints)
		{
			uint returnUint = (experiencePoints / kConversionRate);
			return returnUint;
		}
	}
}
