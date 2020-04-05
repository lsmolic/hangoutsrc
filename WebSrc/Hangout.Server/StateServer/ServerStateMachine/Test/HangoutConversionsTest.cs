using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Server;

namespace Hangout.Shared.UnitTest
{
	[TestFixture]
	class HangoutConversionsTest
	{

		[Test]
		public void MiniGameExperienceToVCoinTest()
		{
			uint experiencePoints = 3;
			uint coinValue = HangoutConversions.MiniGameExperienceToVCoin(experiencePoints);
			Assert.IsTrue(coinValue == 1);

			experiencePoints = 5;
			coinValue = HangoutConversions.MiniGameExperienceToVCoin(experiencePoints);
			Assert.IsTrue(coinValue == 1);

			experiencePoints = 10;
			coinValue = HangoutConversions.MiniGameExperienceToVCoin(experiencePoints);
			Assert.IsTrue(coinValue == 3);

		}
	}
}
