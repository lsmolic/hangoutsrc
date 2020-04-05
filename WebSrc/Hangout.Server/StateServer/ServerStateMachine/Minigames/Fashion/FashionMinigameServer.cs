/**  --------------------------------------------------------  *
 *   FashionMinigameServer.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/14/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using System.Xml;
using Hangout.Shared;
using Hangout.Shared.FashionGame;
using Hangout.Shared.Messages;
using log4net;
using System.Collections.Specialized;

namespace Hangout.Server
{
    public class FashionMinigameServer : AbstractExtension
    {
		private static ILog mLogger = LogManager.GetLogger("FashionMinigame");
		private readonly FashionEnergyManager mEnergyManager;
		public FashionEnergyManager EnergyManager
		{
			get { return mEnergyManager; }
		}

		private readonly Random mRand = new Random();

		public FashionMinigameServer(ServerStateMachine serverStateMachine) : base(serverStateMachine)
        {
			mEnergyManager = new FashionEnergyManager(mLogger, serverStateMachine.SessionManager.GetServerAccountFromSessionId, SendMessageToClient);

			mMessageActions.Add(MessageSubType.RequestLoadingInfo, BuildFashionGameLoadingInfo);
			mMessageActions.Add(MessageSubType.RequestNpcAssets, GetAvatarsForJob);
			mMessageActions.Add(MessageSubType.GetPlayerData, GetPlayerGameData);
			mMessageActions.Add(MessageSubType.SetPlayerData, SetPlayerGameData);
			mMessageActions.Add(MessageSubType.LevelComplete, SetLevelComplete);
			mMessageActions.Add(MessageSubType.GetFriendsToHire, GetFriendsToHire);
			mMessageActions.Add(MessageSubType.GetAllHiredAvatars, GetAllHiredAvatars);
			mMessageActions.Add(MessageSubType.HireFriend, HireFriend);
			mMessageActions.Add(MessageSubType.UseEnergy, mEnergyManager.UseEnergy);
        }

		public static bool VerifyMessageData(Message message, int expectedDataCount, ILog errorLog)
		{
			bool result = true;
			if (message.Data.Count != expectedDataCount)
			{
				string errorMessage = "Invalid Message Data (" + message + "), expected Data to be Count == " + expectedDataCount + ", actual was " + message.Data.Count;
				if (errorLog != null)
				{
					errorLog.Error(errorMessage);
				}
				else
				{
					throw new Exception(errorMessage);
				}
			}
			return result;
		}

		private void GetPlayerGameData(Message message, Guid sessionId)
		{
            mLogger.Debug("GetPlayerGameData: " + message.ToString());
            if (message.Data.Count < 2)
			{
				mLogger.Error("Dropping Message (" + message + "), expected Data to be Count == 2, actual was " + message.Data.Count);
				return;
			}

            string callbackId = CheckType.TryAssignType<string>(message.Data[0]);
            string dataKey = CheckType.TryAssignType<string>(message.Data[1]);
            ServerAccount serverAccount = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(sessionId);

            mLogger.Debug("GetGameData: SENT");
			FashionMinigameServiceAPI.GetGameData(serverAccount, dataKey, delegate(XmlDocument resultXml)
			{
                mLogger.Debug("GetGameData: RECEIVED");
				Message responseMessage = new Message();
				List<object> responseData = new List<object>();
				responseData.Add(callbackId);
				responseData.Add(resultXml.OuterXml);
				responseMessage.FashionGameGetData(responseData);
				SendMessageToClient(responseMessage, sessionId);
			});
		}

		/// <summary>
		/// Saves a friend as hired for the provided job and then returns that friend's avatar's DNA to the client
		/// </summary>
		/// <param name="message"></param>
		private void HireFriend(Message message, Guid sessionId)
		{
            mLogger.Debug("HireFriend: " + message.ToString());
			if( message.Data.Count != 3 )
			{
				mLogger.Error("Dropping Message (" + message + "), expected Data to be Count == 4, actual was " + message.Data.Count);
				return;
			}

			long facebookId;
			if( !long.TryParse(message.Data[1].ToString(), out facebookId) )
			{
				mLogger.Error("Dropping Message (" + message + "), expected Data[1] to be able to parse to long. Value: " + message.Data[1] );
				return;
			}

			Jobs job = (Jobs)Enum.Parse(typeof(Jobs), message.Data[2].ToString());
			string jobName = job.ToString();
			ServerAccount serverAccount = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(sessionId);
			FashionMinigameServiceAPI.GetGameData(serverAccount, jobName, delegate(XmlDocument resultXml)
			{
				XmlNode hiredFriendsListNode = resultXml.SelectSingleNode
				(
					"//DataKey[@KeyName='" + jobName + "']"
				);

				string dataValue;
				if( hiredFriendsListNode == null )
				{
					dataValue = facebookId.ToString();
				}
				else
				{
					dataValue = String.Format("{0},{1}", hiredFriendsListNode.InnerText, facebookId.ToString());
				}
				FashionMinigameServiceAPI.SetGameData(serverAccount, jobName, dataValue, delegate(XmlDocument responseXml)
				{
					VerifySuccess(responseXml);

					Action<List<object>> returnHiredFriend = delegate(List<object> responseData)
					{
						Message responseMessage = new Message();
						responseData.Insert(0, message.Data[0]); // callback Id
						responseData.Insert(1, jobName);
						responseMessage.FashionGameHireFriend(responseData);
						SendMessageToClient(responseMessage, sessionId);
					};

					ProcessHiredFriends(serverAccount, returnHiredFriend, new List<object>(), new long[] { facebookId });
				});
			});
		}

		private static void HandleFriendCase
		(
			ServerAccount serverAccount, 
			long facebookId, 
			Action<FacebookFriendInfo> onIsHangoutUser, 
			Action<FacebookFriendInfo> onIsNotHangoutUser, 
			Hangout.Shared.Action onIsUnknownFacebookId
		)
		{
			if(onIsHangoutUser == null)
			{
				throw new ArgumentNullException("onIsHangoutUser");
			}
			
			if(onIsNotHangoutUser == null)
			{
				throw new ArgumentNullException("onIsNotHangoutUser");
			}
			
			if(onIsUnknownFacebookId == null)
			{
				throw new ArgumentNullException("onIsUnknownFacebookId");
			}

			FacebookFriendInfo facebookFriendInfo;
			if (serverAccount.FacebookFriendsLookupTable.TryGetValue(facebookId, out facebookFriendInfo))
			{
				if( facebookFriendInfo.AccountId != null )
				{
					onIsHangoutUser(facebookFriendInfo);
				}
				else
				{
					onIsNotHangoutUser(facebookFriendInfo);
				}
			}
			else if(serverAccount.FacebookAccountId == facebookId)
			{
				onIsHangoutUser(null);
			}
			else
			{
				onIsUnknownFacebookId();
			}
		}

		public static void VerifySuccess(XmlDocument responseXml)
		{
			if (responseXml.SelectSingleNode("Success") == null)
			{
				StateServerAssert.Assert(new Exception("Fashion Minigame Server Error:\n" + responseXml.OuterXml));
			}
		}

		private void GetFriendsToHire(Message message, Guid sessionId)
		{
            mLogger.Debug("GetFriendsToHire: " + message.ToString());
			if( message.Data.Count < 2 )
			{
				mLogger.Error("Dropping Message (" + message + "), expected Data to be Count == 2, actual was " + message.Data.Count);
				return;
			}

			Jobs job = (Jobs)Enum.Parse(typeof(Jobs), message.Data[1].ToString());
			ServerAccount serverAccount = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(sessionId);
            serverAccount.FacebookFriendsReadyCallbackId = (ulong)message.Data[0];


			XmlDocument resultXml = new XmlDocument();
			XmlElement rootElement = resultXml.CreateElement("FacebookFriends");
            serverAccount.FacebookFriendsRequested = true;

            if (!serverAccount.FacebookFriendsPending)
            {
                FacebookFriendsReady(serverAccount, sessionId);
            }
            else
            {
                mLogger.Debug("Waiting for facebook friends to return for account: " + serverAccount.AccountId.ToString());
            }
		}

        public void FacebookFriendsReady(ServerAccount serverAccount, Guid sessionId)
        {
			try
			{
				if (serverAccount.FacebookFriendsRequested)
				{
					mLogger.Debug("FacebookFriendsReady: " + serverAccount.AccountId.ToString());
					serverAccount.FacebookFriendsRequested = false;
					IDictionary<long, FacebookFriendInfo> possibleHires = new Dictionary<long, FacebookFriendInfo>(serverAccount.FacebookFriendsLookupTable);

					FashionMinigameServiceAPI.GetAllHiredFriends(serverAccount, delegate(ICollection<long> hiredFriendIds)
					{
						// Filter out any users that have already been hired
						foreach (long facebookId in hiredFriendIds)
						{
							possibleHires.Remove(facebookId);
						}

						Message responseMessage = new Message();
						List<object> responseData = new List<object>();
						responseData.Add(serverAccount.FacebookFriendsReadyCallbackId); // callback Id
						if (possibleHires.Count == 0)
						{
							responseData.Add(false);
						}
						else
						{
							responseData.Add(possibleHires);
						}
						responseMessage.FashionGameFriendsToHire(responseData);
						SendMessageToClient(responseMessage, sessionId);
					});
				}
			}
			catch (System.Exception ex)
			{
				StateServerAssert.Assert(new Exception("Something went wrong with sending the facebook friends to the client.", ex));
			}

        }

        private void SetPlayerGameData(Message message, Guid sessionId)
		{
            mLogger.Debug("SetPlayerGameData: " + message.ToString());
			if (message.Data.Count < 2)
			{
				mLogger.Error("Dropping Message (" + message + "), expected Data to be Count == 3, actual was " + message.Data.Count);
				return;
			}

			string dataKey = CheckType.TryAssignType<string>(message.Data[0]);
			string dataValue = CheckType.TryAssignType<string>(message.Data[1]);

            ServerAccount serverAccount = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(sessionId);

			FashionMinigameServiceAPI.SetGameData(serverAccount, dataKey, dataValue, VerifySuccess);
		}

        private void SetLevelComplete(Message message, Guid sessionId)
		{
            mLogger.Debug("SetLevelComplete: " + message.ToString());

            if (message.Data.Count < 4)

			{
				mLogger.Error("Dropping Message (" + message + "), expected Data to be Count == 3, actual was " + message.Data.Count);
				return;
			}

            int totalXP = CheckType.TryAssignType<int>(message.Data[1]);
            int XPEarnedOnThisLevel = CheckType.TryAssignType<int>(message.Data[2]);
			bool leveledUp = CheckType.TryAssignType<bool>(message.Data[3]);

            ServerAccount serverAccount = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(sessionId);

            // Save experience to the database
			FashionMinigameServiceAPI.SetGameData(serverAccount, GameDataKeys.PLAYER_EXPERIENCE_KEY, totalXP.ToString(), VerifySuccess);

            // Reward coins for experience
			int coinsEarned = Rewards.GetCoinsFromExperience(XPEarnedOnThisLevel);
            NameValueCollection  args = new NameValueCollection();
            args.Add("userId", mServerStateMachine.PaymentItemsManager.GetPaymentItemsUserId(sessionId));
            args.Add("amount", coinsEarned.ToString());
            args.Add("ipAddress", mServerStateMachine.ServerMessageProcessor.ServerReflector.GetClientIPAddress(sessionId));

            PaymentItemsProcessClientMessage clientMessage = new PaymentItemsProcessClientMessage();
            PaymentCommand cmd = clientMessage.AddVirtualCoinForUser(args);

            mServerStateMachine.PaymentItemsManager.ProcessPaymentCommand(cmd, delegate(string response) 
            {
                XmlDocument xmlResponse = new XmlDocument();
                xmlResponse.LoadXml(response);
                mLogger.Debug("SetLevelComplete success: " + response);
                XmlElement vcoinNode = (XmlElement)xmlResponse.SelectSingleNode("//accounts/account[@currencyName='VCOIN']");

                // Convert to double, cast to int and then back to string to strip off decimal points
                string totalCoins = String.Empty;
                if(vcoinNode != null)
                {
                    totalCoins = ((int)Double.Parse(vcoinNode.GetAttribute("balance"))).ToString();
                }

				Hangout.Shared.Action resultMessage = delegate()
				{
					// Return a message to the user with the results of SetLevelComplete
					List<object> data = new List<object>();
					data.Add(message.Data[0]); // callback id
					data.Add(coinsEarned.ToString());
					data.Add(totalCoins);
					Message responseMessage = new Message(MessageType.FashionMinigame, data);
					SendMessageToClient(responseMessage, sessionId);
				};
			});

			// Award friends a fraction of earned coins for working in this show
			int friendEscrowCoins = Rewards.GetFriendCoins(XPEarnedOnThisLevel, serverAccount.EntourageSize);
			if (friendEscrowCoins > 0) 
			{
				EscrowManager.GiveFashionMinigameHiredFriendsCoins(serverAccount, friendEscrowCoins);
			}

			if (leveledUp)
			{
				mEnergyManager.UserLeveledUp(serverAccount);
			}
		}

		private void GetAvatarsForJob(Message message, Guid sessionId)
		{
            mLogger.Debug("GetAvatarsForJob: " + message.ToString());
            if (message.Data.Count < 2)
			{
				mLogger.Error("Dropping Message (" + message + "), expected Data to be Count == 3, actual was " + message.Data.Count);
				return;
			}

			Jobs avatarJob = (Jobs)Enum.Parse(typeof(Jobs), message.Data[1].ToString());

            ServerAccount localUserServerAccount = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(sessionId);
			Action<List<object>> returnFunc = delegate(List<object> resultData)
			{
				Message responseMessage = new Message();
				resultData.Insert(0, message.Data[0]); // callbackId
				resultData.Insert(1, avatarJob.ToString());
				responseMessage.FashionGameModelAssetMessage(resultData);
				SendMessageToClient(responseMessage, sessionId);
			};

			if( avatarJob == Jobs.Model )
			{
				// Model is the same as other jobs, but it also has a listing for the local user's avatar
				GetFashionModelAvatars(localUserServerAccount, sessionId, returnFunc);
			}
			else
			{
				FashionMinigameServiceAPI.GetHiredFriendsForJob(localUserServerAccount, avatarJob, delegate(ICollection<long> hiredFriendIds)
				{
					ProcessHiredFriends(localUserServerAccount, returnFunc, new List<object>(), hiredFriendIds);
				});
			}
		}

		private void GetAllHiredAvatars(Message message, Guid sessionId)
		{
            ServerAccount localUserServerAccount = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(sessionId);

			AvatarManagerServiceAPI.GetAvatarForUser(localUserServerAccount, delegate(XmlDocument localAvatarXml)
			{
				FashionMinigameServiceAPI.GetAllHiredFriends(localUserServerAccount, delegate(XmlDocument hiredFriendsXml)
				{
					Dictionary<long, Jobs> friendFbIds = Functionals.Reduce<Dictionary<long, Jobs>>
					(
						delegate(Dictionary<long, Jobs> accumulator, object friendIdNode)
						{
							XmlNode friendNode = (XmlNode)friendIdNode;
							foreach (string idText in friendNode.InnerText.Split(','))
							{
								accumulator.Add(long.Parse(idText), (Jobs)Enum.Parse(typeof(Jobs), friendNode.SelectSingleNode("@KeyName").InnerText));
							}
							return accumulator;
						},
						hiredFriendsXml.SelectNodes("//DataKey")
					);

					List<object> resultData = new List<object>();

					XmlNode localAvatarNode = localAvatarXml.SelectSingleNode("//Avatar");
					
					XmlAttribute fbidAttrib = localAvatarXml.CreateAttribute("FBID");
					fbidAttrib.InnerText = localUserServerAccount.FacebookAccountId.ToString();
					localAvatarNode.Attributes.Append(fbidAttrib);

					XmlAttribute firstnameAttrib = localAvatarXml.CreateAttribute("FirstName");
					firstnameAttrib.InnerText = localUserServerAccount.Nickname;
					localAvatarNode.Attributes.Append(firstnameAttrib);

					XmlAttribute lastnameAttrib = localAvatarXml.CreateAttribute("LastName");
					lastnameAttrib.InnerText = "";
					localAvatarNode.Attributes.Append(lastnameAttrib);

					resultData.Add(localAvatarXml.OuterXml);

					ProcessHiredFriends
					(
						localUserServerAccount,
						delegate(List<object> messageData)
						{
							friendFbIds.Add(localUserServerAccount.FacebookAccountId, Jobs.Model);

							Message responseMessage = new Message();
							List<object> responseData = new List<object>();
							responseData.Insert(0, message.Data[0]); // callbackId

							// Build asset lists for all the avatars and add minigame data
							foreach (XmlDocument avatarXml in MessageUtil.GetXmlDocumentsFromMessageData(messageData))
							{
								XmlNode avatarNode = avatarXml.SelectSingleNode("//Avatar");
								ReplaceItemIdsWithDna((XmlElement)avatarNode);

								// The job name will be the DataKey the FBID was stored under
								XmlAttribute avatarJobAttrib = avatarXml.CreateAttribute("job");
								long fbid = long.Parse(avatarNode.SelectSingleNode("@FBID").InnerText);
								Jobs friendJob;
								if( friendFbIds.TryGetValue(fbid, out friendJob) )
								{
									avatarJobAttrib.InnerText = friendJob.ToString();
								}
								else if(fbid == 0) // Unknown user
								{

								}
								else
								{
									string errorMessage = "Unable to find job info for FBID: " + fbid;
									mLogger.Error(errorMessage);
									avatarJobAttrib.InnerText = errorMessage;
								}

								avatarNode.Attributes.Append(avatarJobAttrib);
								responseData.Add(avatarNode.OuterXml);
							}

							responseMessage.FashionGameModelAssetMessage(responseData);
							SendMessageToClient(responseMessage, sessionId);
						},
						resultData,
						friendFbIds.Keys
					);
				});
			});
		}

		private void ReplaceItemIdsWithDna(XmlElement avatarNode)
		{
			if( avatarNode == null )
			{
				throw new ArgumentNullException("avatarNode");
			}

			if( HasResolvedAssets(avatarNode) )
			{
				return;
			}

			XmlDocument avatarXmlDoc = avatarNode.OwnerDocument;

			List<ItemId> avatarItemIds;
			if (AvatarXmlUtil.GetItemIdsFromAvatarXmlNode(avatarNode, out avatarItemIds))
			{
                XmlDocument assetsXml = mServerStateMachine.ServerAssetRepository.GetXmlDna(avatarItemIds);
				XmlNode dnaNode = avatarNode.SelectSingleNode("AvatarDna");

				if( dnaNode == null )
				{
					throw new Exception("avatarNode is not in the expected format.\n" + avatarNode.OuterXml);
				}
				
				dnaNode.RemoveAll();
				dnaNode.AppendChild(avatarXmlDoc.ImportNode(assetsXml.SelectSingleNode("//Items"), true));
			}
			else
			{
				XmlNode errorNode = avatarXmlDoc.CreateElement("Error");
				errorNode.InnerText = "avatarNode does not have the expected format";
				avatarNode.AppendChild(errorNode);
			}
		}

		private void GetFashionModelAvatars(ServerAccount serverAccount, Guid sessionId, Action<List<object>> returnFunc)
		{
			// Get the local avatar assets
			AvatarManagerServiceAPI.GetAvatarForUser(serverAccount, delegate(XmlDocument localAvatarXml)
			{
				List<object> responseData = new List<object>();

				XmlElement avatarNode = (XmlElement)localAvatarXml.SelectSingleNode("//Avatar");
				XmlAttribute localAvatarAttrib = localAvatarXml.CreateAttribute("LocalAvatar");
				localAvatarAttrib.InnerText = "true";
				avatarNode.Attributes.Append(localAvatarAttrib);
				
				ReplaceItemIdsWithDna(avatarNode);
				AddFacebookData
				(
					avatarNode,
					new FacebookFriendInfo
					(
						serverAccount.AccountId, 
						serverAccount.FacebookAccountId, 
						serverAccount.Nickname, 
						"", 
						""
					)
				);

				responseData.Add(avatarNode.OuterXml);

				FashionMinigameServiceAPI.GetHiredFriendsForJob(serverAccount, Jobs.Model, delegate(ICollection<long> hiredFriendIds)
				{
					ProcessHiredFriends(serverAccount, returnFunc, responseData, hiredFriendIds);
				});
			});
		}

		private void ProcessHiredFriends(ServerAccount serverAccount, 
			                             Action<List<object>> returnFunc, 
			                             List<object> responseData, 
			                             ICollection<long> hiredFriendIds)
		{
			if (hiredFriendIds.Count > 0)
			{
				Dictionary<AccountId, FacebookFriendInfo> friendAccounts = new Dictionary<AccountId, FacebookFriendInfo>();
				List<FacebookFriendInfo> friendsWithoutAccounts = new List<FacebookFriendInfo>();

				foreach (long facebookId in hiredFriendIds)
				{
					if( facebookId == serverAccount.FacebookAccountId )
					{
						friendAccounts.Add(serverAccount.AccountId, null);
						continue;
					}

					HandleFriendCase
					(
						serverAccount,
						facebookId,
						delegate(FacebookFriendInfo ffi) { friendAccounts.Add(ffi.AccountId, ffi); },
						delegate(FacebookFriendInfo ffi) { friendsWithoutAccounts.Add(ffi); },
						delegate() { friendsWithoutAccounts.Add(new FacebookFriendInfo(new AccountId(0u), facebookId, "Unfriended", "", "")); }
					);
				}

				AvatarManagerServiceAPI.GetAvatarForUsers(friendAccounts.Keys, delegate(XmlDocument friendAccountsAvatars)
				{
					AvatarManagerServiceAPI.GetSystemAvatars(delegate(XmlDocument npcAvatarsXml)
					{
						// Get the Avatars for the friends with Hangout Avatars
						foreach (XmlNode friendAvatarNode in friendAccountsAvatars.SelectNodes("Avatars/Avatar"))
						{
							// This is a little weird... the dictionary doesn't actually contain the new object, but that object
							// will index properly into the dictionary for what we want. If we didn't do this weirdness we'd have
							// to make a new data structure or search linearly
							AccountId accountId = new AccountId(uint.Parse(friendAvatarNode.SelectSingleNode("@AccountId").InnerText));

							FacebookFriendInfo facebookFriend;
							if( !friendAccounts.TryGetValue(accountId, out facebookFriend) )
							{
								StateServerAssert.Assert
								(
									new Exception
									(
										"Facebook friend ID provided by the client (" +
										accountId + 
										") was not found in Account ID (" + 
										serverAccount.AccountId + 
										")'s friend list while trying to process hired friends"
									)
								);
								return;
							}

							XmlElement friendAvatarElement = (XmlElement)friendAvatarNode;
							ReplaceItemIdsWithDna(friendAvatarElement);
							AddFacebookData(friendAvatarNode, facebookFriend);
							responseData.Add(friendAvatarNode.OuterXml);
						}

						// Get the avatars for the friends without Hangout Avatars
						XmlNodeList npcAvatarNodes = npcAvatarsXml.SelectNodes("/Avatars/Avatar");
						foreach (FacebookFriendInfo facebookFriend in friendsWithoutAccounts)
						{
							XmlNode npcAvatarNode = npcAvatarNodes[mRand.Next(0, npcAvatarNodes.Count)];
							
							// Local avatar is already expanded to assets
							XmlElement npcElement = (XmlElement)npcAvatarNode;
							ReplaceItemIdsWithDna(npcElement);
							AddFacebookData(npcAvatarNode, facebookFriend);
							responseData.Add(npcAvatarNode.OuterXml);
						}

						returnFunc(responseData);
					});
				});
			}
			else
			{
				returnFunc(responseData);
			}
		}

		private bool HasResolvedAssets(XmlNode avatarNode)
		{
			return avatarNode.SelectSingleNode(".//Asset") != null;
		}

		/// <summary>
		/// Formats facebook data into an avatar XML node
		/// </summary>
		private void AddFacebookData(XmlNode avatarNode, FacebookFriendInfo facebookFriend)
		{
			if( facebookFriend == null )
			{
				mLogger.Warn("Attempted to access facebook data for an unknown facebook friend. Defaulting to the 'unknown avatar' info.");
				facebookFriend = new FacebookFriendInfo(new AccountId(0u), 0L, "Unfriended", "", "");
			}
			if( avatarNode == null )
			{
				throw new ArgumentNullException("avatarNode");
			}

			XmlAttribute firstNameAttrib = avatarNode.OwnerDocument.CreateAttribute("FirstName");
			firstNameAttrib.InnerText = facebookFriend.FirstName;
			avatarNode.Attributes.Append(firstNameAttrib);

			XmlAttribute lastNameAttrib = avatarNode.OwnerDocument.CreateAttribute("LastName");
			lastNameAttrib.InnerText = facebookFriend.LastName;
			avatarNode.Attributes.Append(lastNameAttrib);

			XmlAttribute fbidNameAttrib = avatarNode.OwnerDocument.CreateAttribute("FBID");
			fbidNameAttrib.InnerText = facebookFriend.FbAccountId.ToString();
			avatarNode.Attributes.Append(fbidNameAttrib);

			XmlAttribute imageNameAttrib = avatarNode.OwnerDocument.CreateAttribute("ImageUrl");
			imageNameAttrib.InnerText = facebookFriend.ImageUrl;
			avatarNode.Attributes.Append(imageNameAttrib);
		}

		private XmlNode FacebookFriendToXml(XmlNode parentNode, FacebookFriendInfo facebookFriend)
		{
			XmlNode result = parentNode.OwnerDocument.CreateElement("FacebookFriend");

			XmlAttribute firstNameAttrib = result.OwnerDocument.CreateAttribute("FirstName");
			firstNameAttrib.InnerText = facebookFriend.FirstName;
			result.Attributes.Append(firstNameAttrib);

			XmlAttribute lastNameAttrib = result.OwnerDocument.CreateAttribute("LastName");
			lastNameAttrib.InnerText = facebookFriend.LastName;
			result.Attributes.Append(lastNameAttrib);

			XmlAttribute fbidAttrib = parentNode.OwnerDocument.CreateAttribute("FBID");
			fbidAttrib.InnerText = facebookFriend.FbAccountId.ToString();
			result.Attributes.Append(fbidAttrib);

			XmlAttribute imageNameAttrib = result.OwnerDocument.CreateAttribute("ImageUrl");
			imageNameAttrib.InnerText = facebookFriend.ImageUrl;
			result.Attributes.Append(imageNameAttrib);

			parentNode.AppendChild(result);
			return result;
		}

		private void BuildFashionGameLoadingInfo(Message message, Guid sessionId)
		{
            mLogger.Debug("BuildFashionGameLoadingInfo: " + message.ToString());

			if (message.Data.Count < 1)
			{
				mLogger.Error("Dropping Message (" + message + "), expected Data to be Count == 2, actual was " + message.Data.Count);
				return;
			}

			// TODO: Hard coded values
            XmlDocument modelDefaultsXml = mServerStateMachine.ServerAssetRepository.GetXmlDna(new ItemId[] { new ItemId(135u), new ItemId(136u), new ItemId(137u) });

			// TODO: Replace these items with the real station worker defaults
            XmlDocument stationWorkerDefaultsXml = mServerStateMachine.ServerAssetRepository.GetXmlDna(new ItemId[] { new ItemId(240u), new ItemId(244u) });
			
            ServerAccount localUserServerAccount = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(sessionId);

			mEnergyManager.GetCurrentEnergyData(localUserServerAccount, delegate(float lastEnergy, float maxEnergy, DateTime lastUpdate)
			{
				FashionMinigameServiceAPI.GetGameData(localUserServerAccount, GameDataKeys.PLAYER_EXPERIENCE_KEY, delegate(XmlDocument resultXml)
				{
					XmlNode minigameUserDataNode = resultXml.SelectSingleNode("MiniGameUserData");
					if (minigameUserDataNode == null)
					{
						throw new Exception("The server is returning playerData in an unexpected format: " + resultXml.OuterXml);
					}

					XmlNode xpNode = minigameUserDataNode.SelectSingleNode("//DataKey[@KeyName='" + GameDataKeys.PLAYER_EXPERIENCE_KEY + "']");
					string xpString;
					if( xpNode != null )
					{
						xpString = xpNode.InnerText;
					}
					else // No experience node, this is a new user
					{
						xpString = "0";
					}

					Message responseMessage = new Message();
					List<object> responseData = new List<object>();
					responseData.Add(message.Data[0]); // callback Id
					responseData.Add(modelDefaultsXml.OuterXml);
					responseData.Add(stationWorkerDefaultsXml.OuterXml);
					responseData.Add(lastEnergy);
					responseData.Add(maxEnergy);
					responseData.Add(lastUpdate.ToString());
					responseData.Add(uint.Parse(xpString));
					responseData.Add((uint)localUserServerAccount.EntourageSize);
					responseMessage.FashionGameLoadingInfoMessage(responseData);
					SendMessageToClient(responseMessage, sessionId);
				});
			});
		}
    }
}

