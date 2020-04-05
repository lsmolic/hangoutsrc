using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using Hangout.Shared.Messages;
using System.Xml;
using Hangout.Server.StateServer;

namespace Hangout.Server
{
    public class RoomManager : AbstractExtension
    {
        private Dictionary<RoomId, IServerDistributedRoom> mRoomIdToRoomDistributedObject = null;

		public RoomManager(ServerStateMachine serverStateMachine) : base(serverStateMachine)
        {
            mRoomIdToRoomDistributedObject = new Dictionary<RoomId, IServerDistributedRoom>();

			mMessageActions.Add(MessageSubType.RequestRooms, SendClientAvailableRooms);
			mMessageActions.Add(MessageSubType.DeleteRoom, DeleteRoomForClient);
			mMessageActions.Add(MessageSubType.SwitchRoom, SwitchRoomForClient);
			mMessageActions.Add(MessageSubType.CreateRoom, CreateRoomForClient);
			mMessageActions.Add(MessageSubType.LeaveRoom, LeaveRoomForClient);
			mMessageActions.Add(MessageSubType.JoinRoom, JoinRoomForClient);
        }

        #region createRoomFunctions
            public void CreateRoomForClient(Message receivedMessage, Guid senderId)
            {
                string roomName = CheckType.TryAssignType<string>(receivedMessage.Data[0]);
                RoomType roomType = CheckType.TryAssignType<RoomType>(receivedMessage.Data[1]);
                PrivacyLevel privacyLevel = CheckType.TryAssignType<PrivacyLevel>(receivedMessage.Data[2]);

				CreateNewRoomInDatabase(senderId, roomName, roomType, privacyLevel,
                    delegate(IServerDistributedRoom newRoom)
                    {
						SendClientAvailableRooms(senderId, MessageSubType.ClientOwnedRooms);
                    }
                );
            }

            /// <summary>
            /// This is called after PaymentItemsCommand response.  If the command was for purchasing a room, add a room
            /// to the users room database.
            /// </summary>
            /// <param name="responseData"></param>
            /// <returns></returns>
            public bool RoomItemPurchaseCallback(XmlNodeList purchasedRoomItems, Guid sessionId)
            {
                bool roomCreated = false;
				//flag this to true if we find item elements in the xmlNodeList
				roomCreated = (purchasedRoomItems.Count > 0);
                foreach (XmlElement item in purchasedRoomItems)
                {
                    Console.WriteLine("Room Addition purchase complete for sessionId: " + sessionId.ToString() + ".  Adding room to database.");
                    string roomName = item.GetAttribute("buttonName");
                    RoomType roomType = RoomType.GreenScreenRoom;
                    PrivacyLevel privacyLevel = PrivacyLevel.Private;
                    CreateNewRoomInDatabase(sessionId, roomName, roomType, privacyLevel,
                        delegate(IServerDistributedRoom newRoom)
                        {
							SendClientAvailableRooms(sessionId, MessageSubType.ClientOwnedRooms);
                        });
                 }

                return roomCreated;
            }


            /// <summary>
            /// This function creates the room on the database...
            /// ...and then uses AddRoomToManager to update the RoomManager class' internal data structures
            /// </summary>
            public virtual void CreateNewRoomInDatabase(Guid sessionId, string roomName, RoomType roomType, PrivacyLevel privacyLevel, System.Action<IServerDistributedRoom> createRoomFinishedCallback)
            {
				Action<XmlDocument> createRoomServiceCallback = delegate(XmlDocument xmlResponse)
                {
                    RoomId roomId = null;
                    XmlNode newRoomNode = xmlResponse.SelectSingleNode("Rooms/Room");
                    AccountId accountId = null;
                    List<ItemId> itemIds = new List<ItemId>();
                    RoomXmlUtil.GetRoomInfoFromRoomXmlNode(newRoomNode, out accountId, out privacyLevel, out roomName, out roomId, out roomType, out itemIds);
                    XmlDocument roomItemsDnaXml = mServerStateMachine.ServerAssetRepository.GetXmlDna(itemIds);

					IServerDistributedRoom newRoom = CreateServerDistributedRoom(accountId, roomName, roomId, roomType, privacyLevel, roomItemsDnaXml);
                    AddRoomToRoomManager(newRoom);
                    createRoomFinishedCallback(newRoom);
                };

				CreateNewRoomInDatabaseService(sessionId, roomName, privacyLevel, createRoomServiceCallback);
            }

			protected virtual void CreateNewRoomInDatabaseService(Guid sessionId, string roomName, PrivacyLevel privacyLevel, System.Action<XmlDocument> createRoomFinishedCallback)
			{
				ServerAccount account = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(sessionId);
				RoomManagerServiceAPI.CreateNewRoomService(account.AccountId, roomName, privacyLevel, createRoomFinishedCallback);
			}
        #endregion

		private IServerDistributedRoom CreateServerDistributedRoom(AccountId accountCreatingRoom, string roomName, RoomId roomId, RoomType roomType, PrivacyLevel privacyLevel, XmlNode roomItemsXml)
		{
			return CreateServerDistributedRoom(new RoomProperties(accountCreatingRoom, roomName, roomId, roomType, privacyLevel, roomItemsXml));
		}

        private IServerDistributedRoom CreateServerDistributedRoom(RoomProperties roomProperties)
        {
            IServerDistributedRoom newServerDistributedRoom = null;
			switch (roomProperties.RoomType)
            {
                case RoomType.GreenScreenRoom:
					newServerDistributedRoom = new ServerDistributedGreenScreenRoom(mServerStateMachine.ServerObjectRepository, mServerStateMachine.ServerAssetRepository, roomProperties.AccountCreatingRoom, roomProperties.RoomName, roomProperties.RoomId, roomProperties.PrivacyLevel, mServerStateMachine.DistributedObjectIdManager.GetNewId(), roomProperties.RoomItemsXml);
                    break;
                case RoomType.MiniGameRoom:
					newServerDistributedRoom = new ServerDistributedMiniGameRoom(mServerStateMachine.ServerObjectRepository, roomProperties.AccountCreatingRoom, roomProperties.RoomName, roomProperties.RoomId, roomProperties.PrivacyLevel, mServerStateMachine.DistributedObjectIdManager.GetNewId(), roomProperties.RoomItemsXml);
                    break;
            }
            return newServerDistributedRoom;
        }

        /// <summary>
        /// This function creates in ram the room distributed object for the RoomManager
        /// roomDnaXmlNode is expected to be in the format of:
        /// <RoomDna RoomName="" RoomType="">
        ///     <Items>1, 2, 3, 4 </Items>
        /// </RoomData>
        /// </summary>
        private void AddRoomToRoomManager(IServerDistributedRoom newServerDistributedRoom)
        {
            //create a new zone Id for the room
            ZoneId zoneId = mServerStateMachine.ZoneManager.GetNewZoneId();

            //register the new room with the server object repository
            mServerStateMachine.ServerObjectRepository.AddObject(newServerDistributedRoom);
            //register the new room with the zone
            mServerStateMachine.ServerEngine.ProcessZoneChange(newServerDistributedRoom, zoneId);
            //add the new room to our dictionary listing indexed by room Id
            if (!mRoomIdToRoomDistributedObject.ContainsKey(newServerDistributedRoom.RoomId))
            {
                mRoomIdToRoomDistributedObject.Add(newServerDistributedRoom.RoomId, newServerDistributedRoom);
            }
            else
            {
				StateServerAssert.Assert(new System.Exception("Trying to create a room that has a non-unique RoomId."));
            }
        }

        #region getRoom(s)Functions

            /// <summary>
            /// the delegate returns null if there was no default room found
            /// will also throw an error if the default room is not enabled in the xml or if the xml is not formatted correctly
            /// this does NOT add the room to the RoomManager cache
            /// </summary>
            /// <param name="userAccount"></param>
            /// <param name="getDefaultRoomFinishedCallback"></param>
            private void GetDefaultRoom(ServerAccount userAccount, System.Action<IServerDistributedRoom> getDefaultRoomFinishedCallback)
            {
                Action<XmlDocument> getDefaultRoomServiceCallback = delegate(XmlDocument xmlResponse)
                {
                    IServerDistributedRoom returnedDefaultRoom = null;
                    XmlNode defaultRoomXmlNode = xmlResponse.SelectSingleNode("Rooms/Room");
                    //if the default Room is null, we want to create a new room and set that as the default room
                    if(defaultRoomXmlNode != null)
                    {
						RoomProperties roomProperties = RoomXmlUtil.GetRoomPropertiesFromXml(defaultRoomXmlNode, mServerStateMachine.ServerAssetRepository);
						IServerDistributedRoom defaultRoom = CreateServerDistributedRoom(roomProperties);
                        if(defaultRoom == null)
                        {
							StateServerAssert.Assert(new System.Exception("Error: the default room is either not enabled or does not contain proper data: " + defaultRoomXmlNode.OuterXml));
                        }
                        else
                        {
                            //if the user's default room is already cached, we just return the room object we have stored
                            // otherwise we add the newly created defaultRoom to the roomManager cache
                            if (!mRoomIdToRoomDistributedObject.TryGetValue(defaultRoom.RoomId, out returnedDefaultRoom))
                            {
                                returnedDefaultRoom = defaultRoom;
                            }
                        }
                    }
                    getDefaultRoomFinishedCallback(returnedDefaultRoom);
                };

                RoomManagerServiceAPI.GetDefaultRoomService(userAccount.AccountId, getDefaultRoomServiceCallback);
            }

            virtual protected void GetRoomFromRoomId(RoomId roomId, System.Action<IServerDistributedRoom> getRoomCallback)
            {
                //if the new room doesn't exist in memory, we need to create it!
                IServerDistributedRoom serverDistributedRoom = null;
                if (!mRoomIdToRoomDistributedObject.TryGetValue(roomId, out serverDistributedRoom))
                {
					GetRoomFromRoomIdService(roomId, getRoomCallback);
                }
                else
				{
					getRoomCallback(serverDistributedRoom);
				}
			}

			protected void GetRoomFromRoomIdService(RoomId roomId, System.Action<IServerDistributedRoom> getRoomCallback)
			{
				RoomManagerServiceAPI.GetRoomService(roomId,
				delegate(XmlDocument xmlResponse)
				{
					RoomProperties roomProperties = RoomXmlUtil.GetRoomPropertiesFromXml(xmlResponse, mServerStateMachine.ServerAssetRepository);
					IServerDistributedRoom serverDistributedRoom = CreateServerDistributedRoom(roomProperties);
					getRoomCallback(serverDistributedRoom);
				});
			}

			private void SetDefaultRoom(ServerAccount userAccount, RoomId roomId)
			{
                WebServiceRequest setDefaultRoomService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Rooms", "SetDefaultRoom");
                setDefaultRoomService.AddParam("accountId", userAccount.AccountId.ToString());
                setDefaultRoomService.AddParam("roomId", roomId.ToString());

                Action<XmlDocument> setDefaultRoomServiceCallback = delegate(XmlDocument xmlResponse)
                {
                    //TODO: error checking?
                };

                setDefaultRoomService.GetWebResponseAsync(setDefaultRoomServiceCallback);
            }

			/// <summary>
			/// this function returns back the default room for a user through the delegate OR 
			/// if the user doesn't have a default room, it creates a room, and sets it as the default room for the user
            /// this does NOT cache any rooms as a side effect!
			/// </summary>
			public void GetOrCreateDefaultRoomForUser(Guid sessionId, ServerAccount userAccount, Action<IServerDistributedRoom> createDefaultRoomFinishedCallback)
			{
				GetDefaultRoom(userAccount,
					delegate(IServerDistributedRoom defaultRoom)
					{
						if (defaultRoom == null)
						{
							string roomName = userAccount.FirstName + " " + userAccount.LastName;
							if(roomName == string.Empty)
							{
								roomName = userAccount.Nickname;
							}

							CreateNewRoomInDatabase(sessionId, roomName, RoomType.GreenScreenRoom, PrivacyLevel.Private, 
								delegate(IServerDistributedRoom newRoom)
								{
									if(newRoom != null)
									{
										SetDefaultRoom(userAccount, newRoom.RoomId);
									}
									createDefaultRoomFinishedCallback(newRoom);
								}
							);
						}
						else
						{
							createDefaultRoomFinishedCallback(defaultRoom);
						}
					}
				);
			}

            public bool GetRoomDistributedObjectId(RoomId roomId, out DistributedObjectId distributedObjectId)
            {
                distributedObjectId = null;
                IServerDistributedRoom serverDistributedRoom = null;
                if (mRoomIdToRoomDistributedObject.TryGetValue(roomId, out serverDistributedRoom))
                {
                    distributedObjectId = serverDistributedRoom.DistributedObjectId;
                    return true;
                }
                return false;
            }

            private void SendClientAvailableRooms(Message receivedMessage, Guid senderId)
            {
				MessageSubType roomRequestType = CheckType.TryAssignType<MessageSubType>(receivedMessage.Data[0]);
                SendClientAvailableRooms(senderId, roomRequestType);
            }

			protected virtual void SendClientAvailableRooms(Guid sessionId, MessageSubType roomRequestType)
            {
                ServerAccount account = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(sessionId);
                Action<XmlDocument> getAvailableRoomsCallback = delegate(XmlDocument xmlResponse)
				{
					List<IServerDistributedRoom> availableRooms = new List<IServerDistributedRoom>();
					List<RoomProperties> roomPropertiesList = RoomXmlUtil.GetRoomsPropertiesFromXml(xmlResponse, mServerStateMachine.ServerAssetRepository);
					foreach(RoomProperties roomProperties in roomPropertiesList)
					{
						availableRooms.Add(CreateServerDistributedRoom(roomProperties));
					}
					Message availableRoomsMessage = GenerateSendClientAvailableRoomsMessage(availableRooms);
					//send message to client
					SendMessageToClient(availableRoomsMessage, sessionId);
				};

                switch(roomRequestType)
                {
					case MessageSubType.ClientOwnedRooms:
                        RoomManagerServiceAPI.GetSessionOwnedRoomsService(account.AccountId, getAvailableRoomsCallback);
                    break;
					case MessageSubType.PublicRooms:
						RoomManagerServiceAPI.GetAllSystemRoomsService(getAvailableRoomsCallback);
                    break;
					case MessageSubType.FriendsRooms:
						
						IList<AccountId> friendAccountIds = 
							Functionals.MapImmediate<FacebookFriendInfo, AccountId>
						(
							delegate(FacebookFriendInfo facebookFriendInfo)
							{
								return facebookFriendInfo.AccountId;
							},
							account.HangoutFacebookFriends
						);

						RoomManagerServiceAPI.GetSessionOwnedRoomsWithPrivacyService(friendAccountIds, PrivacyLevel.Default, getAvailableRoomsCallback);
                    break;
                    default:
                        //Console.WriteLine("be patient! not implemented yet!");
                        List<IServerDistributedRoom> availableRooms = new List<IServerDistributedRoom>();
                        Message availableRoomsMessage = GenerateSendClientAvailableRoomsMessage(availableRooms);
                        //send message to client
                        SendMessageToClient(availableRoomsMessage, sessionId);
                    break;
                }
            }
        #endregion

        #region deleteFunctions
            private void DeleteRoomForClient(Message receivedMessage, Guid senderId)
            {
                RoomId roomId = CheckType.TryAssignType<RoomId>(receivedMessage.Data[0]);

				DisableRoomInDatabase(roomId, senderId,
                    delegate(bool success)
                    {
                        if(success)
                        {
                            DeleteRoomInRoomManager(roomId);
                        }
						SendClientAvailableRooms(senderId, MessageSubType.ClientOwnedRooms);
                    }
                );
            }

            /// <summary>
            /// Removes the room from the database and also removes it from the RoomManager memory
            /// </summary>
            /// <param name="roomId"></param>
            /// <param name="roomDisabledInDatabaseFinishedCallback"></param>
            private void DisableRoomInDatabase(RoomId roomId, Guid sessionId, System.Action<bool> roomDisabledInDatabaseFinishedCallback)
            {
                ServerAccount account = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(sessionId);

                System.Action<XmlDocument> disableRoomServiceCallback = delegate(XmlDocument xmlResponse)
                {
                    //parse true / false out of xml
                    XmlNode successNode = xmlResponse.SelectSingleNode("Success");
                    bool returnValue = Convert.ToBoolean(successNode.InnerXml);
                    roomDisabledInDatabaseFinishedCallback(returnValue);
                };

				RoomManagerServiceAPI.DisableRoomService(account.AccountId, roomId, disableRoomServiceCallback);
            }

            /// <summary>
            /// Removes the specified room from the RoomManager memory
            /// </summary>
            /// <param name="roomId"></param>
            private void DeleteRoomInRoomManager(RoomId roomId)
            {
                IServerDistributedRoom serverDistributedRoomForDelete = null;
                if (mRoomIdToRoomDistributedObject.TryGetValue(roomId, out serverDistributedRoomForDelete))
                {
                    ZoneId zoneId = mServerStateMachine.ServerObjectRepository.GetZone(serverDistributedRoomForDelete);
                List<Guid> sessionIdsToLeaveRoom = new List<Guid>(mServerStateMachine.SessionManager.GetSessionIdsInterestedInZoneId(zoneId));
                foreach (Guid sessionId in sessionIdsToLeaveRoom)
                {
					LeaveRoom(sessionId, serverDistributedRoomForDelete);
                }

                //unregister the room with the zone
                mServerStateMachine.ServerEngine.RemoveObjectFromServer(serverDistributedRoomForDelete);

                //remove roomId from dictionary
                    mRoomIdToRoomDistributedObject.Remove(roomId);
                }
            }
        #endregion

        #region leaveRoomFunctions
			public void LeaveRoomForClient(Message receivedMessage, Guid senderId)
			{
				RoomId roomIdToLeave = CheckType.TryAssignType<RoomId>(receivedMessage.Data[0]);

				LeaveRoom(senderId, roomIdToLeave);
			}

			public void LeaveRoom(Guid sessionId, IServerDistributedRoom serverDistributedRoomToLeave)
			{
				serverDistributedRoomToLeave.DecrementPopulation(sessionId);
				ZoneId zoneIdToLeave = mServerStateMachine.ServerObjectRepository.GetZone(serverDistributedRoomToLeave);
				List<ZoneId> zoneIdsToLeave = new List<ZoneId>();
				zoneIdsToLeave.Add(zoneIdToLeave);
                mServerStateMachine.ServerEngine.ProcessCloseZoneInterest(sessionId, zoneIdsToLeave);

				List<DistributedObjectId> distributedObjectsAssociatedWithSession = mServerStateMachine.ServerObjectRepository.GetDistributedObjectIdsOwnedBySessionId(sessionId);
				//move any distributed objects belonging to the client from the old zone to the new zone 
				foreach (DistributedObjectId distributedObjectId in distributedObjectsAssociatedWithSession)
				{
                    mServerStateMachine.ServerEngine.ProcessZoneChange((ServerDistributedObject)mServerStateMachine.ServerObjectRepository.GetObject(distributedObjectId), ZoneId.LimboZone);
				}
			}

            private void LeaveRoom(Guid sessionId, RoomId oldRoomId)
            {
                //lose interest in the zones that belong to the room the client is leaving
                IServerDistributedRoom serverDistributedRoomToLeave = null;
                if (mRoomIdToRoomDistributedObject.TryGetValue(oldRoomId, out serverDistributedRoomToLeave))
                {
                    LeaveRoom(sessionId, serverDistributedRoomToLeave);
                }
            }
        #endregion

        #region joinRoomFunctions
            private void AddUserSessionToRoom(Guid sessionId, IServerDistributedRoom room)
            {
                //look up ZoneId for DistributedObjectId
                ZoneId zoneId = mServerStateMachine.ServerObjectRepository.GetZone(room);

                //look up all DistributedObjectId's for ZoneId... this is done already when you process an open interest in a zone
                mServerStateMachine.ServerEngine.ProcessOpenZoneInterest(sessionId, zoneId);

                room.IncrementPopulation(sessionId);
            }

            private void SwitchRoomForClient(Message receivedMessage, Guid senderId)
            {
                RoomId newRoomId = CheckType.TryAssignType<RoomId>(receivedMessage.Data[0]);
                MessageSubType roomRequestType = CheckType.TryAssignType<MessageSubType>(receivedMessage.Data[1]);

				ServerAccount account = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(senderId);
                RoomId oldRoomId = account.LastRoomId;

				SwitchRoom(senderId, newRoomId, oldRoomId, delegate()
                    {
                        string newRoom = "";
                        if (newRoomId != null)
                        {
                            newRoom = newRoomId.ToString();
                        }
                        Metrics.Log(LogGlobals.CATEGORY_ROOMS, LogGlobals.ROOM_ENTERED, LogGlobals.ROOM_LABEL, newRoom, account.AccountId.ToString());
						SendClientAvailableRooms(senderId, roomRequestType);
                    }
                );
            }

			private void JoinRoomForClient(Message receivedMessage, Guid senderId)
			{
				RoomId newRoomId = CheckType.TryAssignType<RoomId>(receivedMessage.Data[0]);
				MessageSubType roomRequestType = CheckType.TryAssignType<MessageSubType>(receivedMessage.Data[1]);


				JoinRoom(senderId, newRoomId, delegate()
				{
					AccountId accountId = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(senderId).AccountId;
                    Metrics.Log(LogGlobals.CATEGORY_ROOMS, LogGlobals.ROOM_ENTERED, LogGlobals.ROOM_LABEL, newRoomId.ToString(), accountId.ToString());
					SendClientAvailableRooms(senderId, roomRequestType);
				});
			}

			private void SwitchRoom(Guid sessionId, RoomId newRoomId, RoomId oldRoomId, Hangout.Shared.Action userJoinedRoomCallback)
			{
                if (oldRoomId != null)
                {
                    LeaveRoom(sessionId, oldRoomId);
                }
				JoinRoom(sessionId, newRoomId, userJoinedRoomCallback);
			}

			protected void JoinRoom(Guid sessionId, RoomId newRoomId, Hangout.Shared.Action userJoinedRoomCallback)
			{
                GetRoomFromRoomId(newRoomId,
                    delegate(IServerDistributedRoom serverDistributedRoomToJoin)
                    {
						if (serverDistributedRoomToJoin != null)
						{
							if (!mRoomIdToRoomDistributedObject.ContainsKey(serverDistributedRoomToJoin.RoomId))
							{
								AddRoomToRoomManager(serverDistributedRoomToJoin);
							}
							List<DistributedObjectId> distributedObjectsAssociatedWithSession = mServerStateMachine.ServerObjectRepository.GetDistributedObjectIdsOwnedBySessionId(sessionId);

							//find the zone for the new room the client wants to join
							ZoneId zoneIdToJoin = mServerStateMachine.ServerObjectRepository.GetZone(serverDistributedRoomToJoin);

							SendMessageToClient(BeginLoadingRoomMessage(newRoomId), sessionId);

							//open interest in the zones that the new room belongs to
							AddUserSessionToRoom(sessionId, serverDistributedRoomToJoin);

							//move any distributed objects belonging to the client from the old zone to the new zone 
							foreach (DistributedObjectId distributedObjectId in distributedObjectsAssociatedWithSession)
							{
                                mServerStateMachine.ServerEngine.ProcessZoneChange((ServerDistributedObject)mServerStateMachine.ServerObjectRepository.GetObject(distributedObjectId), zoneIdToJoin);
							}

                            // Save room id as LastRoomId for account
                            ServerAccount account = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(sessionId);
                            account.LastRoomId = newRoomId;
                            if (account != null)
                            {
                                account.UserProperties.SetProperty(UserAccountProperties.LastRoomId, newRoomId);
                                account.SaveCurrentAccountData(delegate(XmlDocument returnedXml) { });
                            }
                            BossServerAPI.UpdateSession(account.AccountId, sessionId.ToString(), "1", zoneIdToJoin.ToString(), mServerStateMachine.StateServerId, delegate(XmlDocument xmlDocument) { });
						}
                        userJoinedRoomCallback();
                    }
                );

            }

        #endregion

        #region loadingRoomMessageFunctions
            public Message BeginLoadingRoomMessage(RoomId roomId)
            {
                List<object> messageData = new List<object>();
                messageData.Add(roomId);
                messageData.Add(mRoomIdToRoomDistributedObject[roomId].DistributedObjectId);

                Message message = new Message();
                message.Callback = (int)MessageSubType.LoadNewRoom;
                message.LoadingMessage(false, false, messageData);
                return message;
            }
        #endregion

        public IServerDistributedRoom FindRoomForUser(Guid sessionId)
        {
            foreach (KeyValuePair<RoomId, IServerDistributedRoom> room in mRoomIdToRoomDistributedObject)
            {
                if(room.Value.ContainsUser(sessionId))
                {
                    return room.Value;
                }
            }
            return null;
        }

        private Message GenerateSendClientAvailableRoomsMessage(List<IServerDistributedRoom> rooms)
        {
            List<object> messageData = new List<object>();
            foreach (IServerDistributedRoom room in rooms)
            {
                //if this room is cached locally, get the most up to date info from the server.. eventually this info should be updated via services
                IServerDistributedRoom cachedRoom = null;
                if (mRoomIdToRoomDistributedObject.TryGetValue(room.RoomId, out cachedRoom))
                {
                    messageData.Add(new KeyValuePair<RoomId, List<object>>(cachedRoom.RoomId, cachedRoom.Data));
                }
                else
                {
                    messageData.Add(new KeyValuePair<RoomId, List<object>>(room.RoomId, room.Data));
                }
            }
            Message sendClientAvailableRoomsMessage = new Message();
            sendClientAvailableRoomsMessage.Callback = (int)MessageSubType.ReceiveRooms;
            sendClientAvailableRoomsMessage.RoomMessage(messageData);

            return sendClientAvailableRoomsMessage;
        }

    }
}
