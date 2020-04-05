/**  --------------------------------------------------------  *
 *   FashionEnergyManager.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 11/30/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using System.Xml;

using Hangout.Shared;
using Hangout.Shared.FashionGame;

using log4net;

namespace Hangout.Server
{
    public class FashionEnergyManager
	{
		public const string PLAYER_ENERGY_KEY = "Energy";
		public const string ENERGY_REGEN_DATE = "EnergyRegenStart";
		public const string PLAYER_MAX_ENERGY = "PlayerMaxEnergy";
		public const float INITIAL_MAX_ENERGY = Energy.INITIAL_MAX_ENERGY;

		private readonly ILog mLogger;
		private readonly Func<ServerAccount, Guid> mGuidToServerAccount;
		private readonly Hangout.Shared.Action<Message, Guid> mSendMessageToClientCallback;

		/// <summary>
		/// Creates a new instance of a FashionEnergyManager
		/// </summary>
		/// <param name="logger">If null, errors on messages will be thrown as exceptions, otherwise they will be logged as errors and the message will be dropped.</param>
		/// <param name="serverStateMachine"></param>
		/// <param name="sendMessageToClientCallback"></param>
		public FashionEnergyManager(ILog logger, Func<ServerAccount, Guid> guidToServerAccount, Hangout.Shared.Action<Message, Guid> sendMessageToClientCallback)
		{
			if (guidToServerAccount == null)
			{
				throw new ArgumentNullException("guidToServerAccount");
			}
			mGuidToServerAccount = guidToServerAccount;
			mLogger = logger;

			if( sendMessageToClientCallback == null )
			{
				throw new ArgumentNullException("sendMessageToClientCallback");
			}
			mSendMessageToClientCallback = sendMessageToClientCallback;
		}

		/// <summary>
		/// Calculates all the parameters required to call Energy.CalculateCurrentEnergy
		/// </summary>
		/// <param name="serverAccount"></param>
		/// <param name="result">returns (float lastEnergy, float maxEnergy, DateTime rechargeDate)</param>
		public void GetCurrentEnergyData(ServerAccount serverAccount, Action<float, float, DateTime> result)
		{
			FashionMinigameServiceAPI.GetGameData
			(
				serverAccount, 
				new string[] { PLAYER_ENERGY_KEY, ENERGY_REGEN_DATE, PLAYER_MAX_ENERGY }, 
				delegate(XmlDocument resultXml)
				{
					string playerEnergyData = null;
					string energyRegenDateData = null;
					string playerMaxEnergyData = null;

					foreach( XmlNode dataNode in resultXml.SelectNodes("//DataKey") )
					{
						switch(dataNode.Attributes["KeyName"].InnerText)
						{
							case PLAYER_ENERGY_KEY:
								playerEnergyData = dataNode.InnerText;
								break;
							case ENERGY_REGEN_DATE:
								energyRegenDateData = dataNode.InnerText;
								break;
							case PLAYER_MAX_ENERGY:
								playerMaxEnergyData = dataNode.InnerText;
								break;
							default:
								throw new Exception("Error in FashionMinigameServiceAPI.GetGameData, result XML has an unexpected key: " + dataNode.Attributes["KeyName"].InnerText + "\nXML:\n" + resultXml.OuterXml);
						}
					}

					if( playerMaxEnergyData == null )
					{
						SetMaxEnergy(serverAccount, INITIAL_MAX_ENERGY);
						playerMaxEnergyData = INITIAL_MAX_ENERGY.ToString();
					}

					if( energyRegenDateData == null )
					{
						SetEnergyRegenDate(serverAccount, DateTime.UtcNow);
						energyRegenDateData = DateTime.UtcNow.ToString();
					}

					if( playerEnergyData == null )
					{
						SetEnergy(serverAccount, INITIAL_MAX_ENERGY);
						playerEnergyData = playerMaxEnergyData;
					}

					float lastEnergy = float.Parse(playerEnergyData);
					float maxEnergy = float.Parse(playerMaxEnergyData);
					DateTime lastUpdate = DateTime.Parse(energyRegenDateData);
					
					result(lastEnergy, maxEnergy, lastUpdate);
				}
			);
		}

		private void SetMaxEnergy(ServerAccount account, float energy)
		{
			FashionMinigameServiceAPI.SetGameData(account, PLAYER_MAX_ENERGY, energy.ToString(), FashionMinigameServer.VerifySuccess);
		}

		private void SetEnergyRegenDate(ServerAccount account, DateTime time)
		{
			FashionMinigameServiceAPI.SetGameData(account, ENERGY_REGEN_DATE, time.ToString(), FashionMinigameServer.VerifySuccess);
		}

		/// <summary>
		/// Directly sets the amount of energy in the database without any bounds or other safety checks.
		/// Only to be used on values after they've been verified
		/// </summary>
		private void SetEnergy(ServerAccount account, float energy)
		{
			FashionMinigameServiceAPI.SetGameData(account, PLAYER_ENERGY_KEY, energy.ToString(), FashionMinigameServer.VerifySuccess);
		}

		/// <summary>
		/// On level up, the user gains a little more max energy and their energy is refilled
		/// </summary>
		public void UserLeveledUp(ServerAccount account)
		{
			GetCurrentEnergyData(account, delegate(float energy, float maxEnergy, DateTime rechargeDate)
			{
				// not worried about this silly chain of messages because the client isn't waiting on it
				float newMaxEnergy = maxEnergy + Energy.MAX_ENERGY_INCREASE_PER_LEVEL;
				FashionMinigameServiceAPI.SetGameData(account, PLAYER_MAX_ENERGY, newMaxEnergy, 
				delegate(XmlDocument maxEnergyResult)
				{
					FashionMinigameServer.VerifySuccess(maxEnergyResult);
					FashionMinigameServiceAPI.SetGameData(account, ENERGY_REGEN_DATE, DateTime.UtcNow, 
					delegate(XmlDocument dateResult)
					{
						FashionMinigameServer.VerifySuccess(dateResult);
						FashionMinigameServiceAPI.SetGameData(account, PLAYER_ENERGY_KEY, newMaxEnergy.ToString(), 
						delegate(XmlDocument currentEnergyResult)
						{
							FashionMinigameServer.VerifySuccess(currentEnergyResult);
						});
					});
				});
			});
		}

		public void RefillEnergy(ServerAccount account)
		{
			GetCurrentEnergyData
			(
				account,
				delegate(float lastEnergy, float maxEnergy, DateTime lastUpdate)
				{
					SetEnergy(account, maxEnergy);
				}
			);
		}

		public void UseEnergy(Message message, Guid sessionId)
		{
			if (!FashionMinigameServer.VerifyMessageData(message, 2, mLogger))
			{
				// Drop the message
				return;
			}

			ServerAccount serverAccount = mGuidToServerAccount(sessionId);
			float energyToUse = CheckType.TryAssignType<float>(message.Data[1]);

			GetCurrentEnergyData
			(
				serverAccount,
				delegate(float lastEnergy, float maxEnergy, DateTime lastUpdate)
				{
					List<object> responseData = new List<object>();
					responseData.Add(message.Data[0]); // callback Id

					float energy = Energy.CalculateCurrentEnergy(maxEnergy, lastEnergy, lastUpdate);
					if(energy >= energyToUse)
					{
						float newEnergy = energy - energyToUse;
						responseData.Add(Energy.UseRequestResult.Success.ToString());
						responseData.Add(newEnergy.ToString());
						responseData.Add(maxEnergy.ToString());

						if (energy == maxEnergy)
						{
							SetEnergyRegenDate(serverAccount, DateTime.UtcNow);
							responseData.Add(DateTime.UtcNow.ToString());
						}
						else
						{
							responseData.Add(lastUpdate.ToString());
						}

						SetEnergy(serverAccount, newEnergy);
					}
					else
					{
						responseData.Add(Energy.UseRequestResult.NotEnoughEnergy.ToString());
					}

					Message responseMessage = new Message(MessageType.FashionMinigame, responseData);
					mSendMessageToClientCallback(responseMessage, sessionId);
				}
			);
		}
    }
}
