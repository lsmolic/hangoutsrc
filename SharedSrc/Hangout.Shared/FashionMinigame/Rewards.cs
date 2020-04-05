
using System;
using System.Collections.Generic;

namespace Hangout.Shared.FashionGame
{
	public static class Rewards
	{
		// We divide your XP gained per level by this number
		// to determine how many coins you are awarded.
		private const float XP_PER_COIN = 1.5f;

		// Divide the coins earned on each level by this number and give
		// those coins to all the friends hired that worked in the show.
		private const float COINS_FOR_FRIENDS_DIVISOR = 10.0f;

		// Gives a percentage bonus per entourage member
		private const float COIN_BONUS_PER_ENTOURAGE_MEMBER = 0.05f;
		private const float MAX_COIN_BONUS = 2.0f;

		private const float XP_BONUS_PER_ENTOURAGE_MEMBER = 0.05f;
		private const float MAX_XP_BONUS = 2.0f;

		public static int GetCoinsFromExperience(int experience)
		{
			return (int)Math.Floor(experience / XP_PER_COIN);
		}

		/// <summary>
		/// Returns the number of coins each hired worker should get
		/// </summary>
		/// <param name="experience"></param>
		/// <param name="entourageSize"></param>
		/// <returns></returns>
		public static int GetFriendCoins(int experience, int entourageSize)
		{
			return (int)Math.Floor(GetCoinsFromExperience(experience) / COINS_FOR_FRIENDS_DIVISOR);
		}

		/// <summary>
		///  Returns the xp bonus percent. 0.0 = no bonus, 1.0 = 100% bonus. Value can go over 1.0
		/// </summary>
		/// <param name="entourageSize"></param>
		/// <returns></returns>
		public static float GetEntourageExperienceBonusPercent(int entourageSize)
		{
			return Math.Min(MAX_XP_BONUS, entourageSize * XP_BONUS_PER_ENTOURAGE_MEMBER);
		}

		public static int GetEntourageExperienceBonus(int experience, int entourageSize)
		{
			return (int)Math.Floor(experience * GetEntourageExperienceBonusPercent(entourageSize));
		}


	}


}
