/**  --------------------------------------------------------  *
 *   Energy.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 11/25/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;

namespace Hangout.Shared.FashionGame
{
	public static class Energy
	{
		public enum UseRequestResult
		{
			Success,
			NotEnoughEnergy
		}

		/// <summary>
		/// How much energy recharges per minute
		/// </summary>
		public const double RECHARGE_RATE = 1.0 / 3.0;
		public const float INITIAL_MAX_ENERGY = 20.0f;
		public const float MAX_ENERGY_INCREASE_PER_LEVEL = 10.0f;

		public static float CalculateCurrentEnergy(float maxEnergy, float lastEnergyAmount, DateTime lastTimeAtMaxEnergy)
		{
			double minutesSinceLastEnergy = (DateTime.UtcNow - lastTimeAtMaxEnergy).TotalMinutes;
			double energyRecharged = RECHARGE_RATE * minutesSinceLastEnergy;
			return (float)Math.Max(0.0, Math.Min((double)maxEnergy, energyRecharged + lastEnergyAmount));
		}

		public static TimeSpan GetTimeToNextRecharge(float maxEnergy, float lastEnergyAmount, DateTime lastTimeAtMaxEnergy)
		{
			double currentEnergy = (double)CalculateCurrentEnergy(maxEnergy, lastEnergyAmount, lastTimeAtMaxEnergy);
			double remainder = currentEnergy - Math.Floor(currentEnergy);
			return TimeSpan.FromMinutes((1.0 - remainder) / RECHARGE_RATE);
		}

	}
}
