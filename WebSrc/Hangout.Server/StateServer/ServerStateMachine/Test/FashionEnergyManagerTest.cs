using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Server;
using Hangout.Shared.FashionGame;

namespace Hangout.Shared.UnitTest
{
    [TestFixture]
    public class FashionEnergyManagerTest : MockFashionSuite
    {
		private Action<Message> mClientMessageFunc = null;
		private readonly FashionEnergyManager mEnergyManager;

		public FashionEnergyManagerTest()
		{
			mEnergyManager = new FashionEnergyManager
			(
				null, 
				delegate(Guid localSession)
				{
					// All the localSession lookups resolve to the MockServerAccount
					return MockServerAccount;
				},
				delegate(Message msg, Guid localSession)
				{
					// Pass thru the result message intended for the client to
					//  whatever each test sets mClientMessageFunc to be
					if (mClientMessageFunc != null)
					{
						mClientMessageFunc(msg);
					}
				}
			);
		}
		
		private void SetResult(XmlDocument xml) { }

		private void SetupEnergyData(uint energy, uint maxEnergy, DateTime lastTimeFull)
		{
			FashionMinigameServiceAPI.SetGameData
			(
				MockServerAccount,
				FashionEnergyManager.PLAYER_ENERGY_KEY,
				energy,
				SetResult
			);

			FashionMinigameServiceAPI.SetGameData
			(
				MockServerAccount,
				FashionEnergyManager.ENERGY_REGEN_DATE,
				lastTimeFull,
				SetResult
			);

			FashionMinigameServiceAPI.SetGameData
			(
				MockServerAccount,
				FashionEnergyManager.PLAYER_MAX_ENERGY,
				maxEnergy,
				SetResult
			);
		}

		[Test]
		public void GetCurrentEnergyVerification()
		{
			// User went from 100 to 0 an hour ago
			SetupEnergyData(0, 100, DateTime.UtcNow - new TimeSpan(1, 0, 0));

			bool callbackCalledImmediately = false;
			mEnergyManager.GetCurrentEnergyData
			(
				MockServerAccount, 
				delegate(float lastEnergy, float maxEnergy, DateTime rechargeDate)
				{
					callbackCalledImmediately = true;
					float currentEnergy = Energy.CalculateCurrentEnergy(maxEnergy, lastEnergy, rechargeDate);
					Assert.IsWithin(60.0f * (float)Energy.RECHARGE_RATE, currentEnergy, 0.01f);
				}
			);
			Assert.IsTrue(callbackCalledImmediately);
		}

		// Test Case:
		// - Start with fresh user data
		// - Use 1/2 their energy
		// - Test that the energy was used
		// - Test that the 'last time at max energy' field has been updated
		[Test]
		public void UsingEnergyStartingAtMaxUpdatesRechargeDate()
		{
			// Fresh user (from yesterday)
			SetupEnergyData(100, 100, DateTime.UtcNow - new TimeSpan(24, 0, 0));

			Message useEnergyMessage = new Message
			(
				MessageType.FashionMinigame,
				new List<object>(new object[]
				{
					"0",		// callbackId
					"50"		// energy to use
				})
			);

			bool responseCalled = false;

			mClientMessageFunc = delegate(Message clientResponse)
			{
				responseCalled = true;

				Energy.UseRequestResult useEnergyResult = (Energy.UseRequestResult)Enum.Parse(typeof(Energy.UseRequestResult), (string)clientResponse.Data[0]);
				Assert.AreEqual(Energy.UseRequestResult.Success, useEnergyResult);

				bool getEnergyCalled = false;
				mEnergyManager.GetCurrentEnergyData
				(
					MockServerAccount,
					delegate(float lastEnergy, float maxEnergy, DateTime rechargeDate)
					{
						getEnergyCalled = true;
						float currentEnergy = Energy.CalculateCurrentEnergy(maxEnergy, lastEnergy, rechargeDate);
						Assert.IsWithin(50.0f, currentEnergy, 0.01f, "Energy should be removed from database when UseEnergy is called");
						Assert.IsTrue
						(
							(DateTime.UtcNow - rechargeDate).TotalHours < 1.0, 
							"The recharge date was not updated when UseEnergy was called with full energy (Expected: " + DateTime.UtcNow + ", Actual: " + rechargeDate + ")"
						);
					}
				);
				Assert.IsTrue(getEnergyCalled);
			};

			mEnergyManager.UseEnergy(useEnergyMessage, Guid.Empty);

			// make sure all the necessary parts are mocked out and that the message wasn't just dropped
			Assert.IsTrue(responseCalled);
		}

		// Test Case:
		// - Start with a user that has used energy but enough time has gone by for that energy to fully recharge
		// - Verify that the user is at max energy
		// - Use 1/2 the energy
		// - Verify that the energy was used and that the recharge date was updated
		[Test]
		public void UsingEnergyVerification()
		{
			// 60 energy recharges in 18 hours, so wait 19
			SetupEnergyData(40, 100, DateTime.UtcNow - new TimeSpan(19, 0, 0));

			// Verify user is now at max energy again
			bool getCurrentEnergyDataReturned = false;
			mEnergyManager.GetCurrentEnergyData
			(
				MockServerAccount,
				delegate(float lastEnergy, float maxEnergy, DateTime rechargeDate)
				{
					getCurrentEnergyDataReturned = true;
					float currentEnergy = Energy.CalculateCurrentEnergy(maxEnergy, lastEnergy, rechargeDate);
					Assert.AreEqual(maxEnergy, currentEnergy);
				}
			);
			Assert.IsTrue(getCurrentEnergyDataReturned);

			Message useEnergyMessage = new Message
			(
				MessageType.FashionMinigame,
				new List<object>(new object[]
				{
					"0",		// callbackId
					"17"		// energy to use
				})
			);

			bool useEnergyReturned = false;
			mClientMessageFunc = delegate(Message clientResponse)
			{
				useEnergyReturned = true;

				Energy.UseRequestResult useEnergyResult = (Energy.UseRequestResult)Enum.Parse(typeof(Energy.UseRequestResult), (string)clientResponse.Data[0]);
				Assert.AreEqual(Energy.UseRequestResult.Success, useEnergyResult);

				bool getEnergyCalled = false;
				mEnergyManager.GetCurrentEnergyData
				(
					MockServerAccount,
					delegate(float lastEnergy, float maxEnergy, DateTime rechargeDate)
					{
						getEnergyCalled = true;
						float currentEnergy = Energy.CalculateCurrentEnergy(maxEnergy, lastEnergy, rechargeDate);
						Assert.IsWithin(83.0f, currentEnergy, 0.01f, "Energy should be removed from database when UseEnergy is called");
						Assert.IsTrue
						(
							(DateTime.UtcNow - rechargeDate).TotalHours < 1.0,
							"The recharge date was not updated when UseEnergy was called with full energy (Expected: " + DateTime.UtcNow + ", Actual: " + rechargeDate + ")"
						);
					}
				);
				Assert.IsTrue(getEnergyCalled);
			};

			mEnergyManager.UseEnergy(useEnergyMessage, Guid.Empty);

			// make sure all the necessary parts are mocked out and that the message wasn't just dropped
			Assert.IsTrue(useEnergyReturned);
		}

		[Test]
		public void RefillEnergyVerification()
		{
			SetupEnergyData(40, 100, DateTime.UtcNow - new TimeSpan(1, 0, 0));

			bool getCurrentEnergyDataReturned = false;
			mEnergyManager.RefillEnergy(MockServerAccount);
			mEnergyManager.GetCurrentEnergyData
			(
				MockServerAccount,
				delegate(float lastEnergy, float maxEnergy, DateTime rechargeDate)
				{
					getCurrentEnergyDataReturned = true;
					float currentEnergy = Energy.CalculateCurrentEnergy(maxEnergy, lastEnergy, rechargeDate);
					Assert.AreEqual(maxEnergy, currentEnergy);
				}
			);
			Assert.IsTrue(getCurrentEnergyDataReturned);
		}
    }
}
