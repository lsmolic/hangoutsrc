/**  --------------------------------------------------------  *
 *   EnergyTest.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 11/25/2009
 *	 
 *   --------------------------------------------------------  *
 */


using System;
using System.Collections.Generic;
using Hangout.Shared.FashionGame;

namespace Hangout.Shared.UnitTest
{
    [TestFixture]
    public class EnergyTest
    {
        [Test]
		public void CalculateCurrentEnergyVerification1()
        {
			DateTime lastTimeAtMaxEnergy = DateTime.Now.ToUniversalTime() - new TimeSpan(1, 0, 0); // 1 hour ago
			float currentEnergy = Energy.CalculateCurrentEnergy(130.0f, 60.0f, lastTimeAtMaxEnergy);
			Assert.IsWithin(80.0f, currentEnergy, 0.01f);
        }

		[Test]
		public void CalculateCurrentEnergyVerification2()
		{
			DateTime lastTimeAtMaxEnergy = DateTime.Now.ToUniversalTime() - new TimeSpan(56, 45, 23);
			float currentEnergyAtCap = Energy.CalculateCurrentEnergy(75.0f, 60.0f, lastTimeAtMaxEnergy);
			Assert.AreEqual(75.0f, currentEnergyAtCap);
		}

		[Test]
		public void CalculateCurrentEnergyVerification3()
		{
			DateTime lastTimeAtMaxEnergy = DateTime.Now.ToUniversalTime() - new TimeSpan(1, 0, 0); // 1 hour ago
			float currentEnergy = Energy.CalculateCurrentEnergy(100.0f, 83.0f, lastTimeAtMaxEnergy);
			Assert.AreEqual(100.0f, currentEnergy);
		}
    }
}
