using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Server;
using Hangout.Shared;
using System.Xml;
using Hangout.Shared.UnitTest;

namespace ServerToServicesTest
{
	public class RoomTests : ITest
	{
		private readonly AccountId mTestAccountId = null;

		public RoomTests()
		{
			mTestAccountId = new AccountId(0);
		}

		public void RunTests()
		{
			//get accounts for an id that should not have an account.. verify no rooms are returned
			GetNewUserSessionOwnedRoomsTest(delegate()
			{
				GetNewUserSessionOwnedRoomsWithPrivacyTest(delegate()
				{
					//create a room for the account
					CreateNewRoom(delegate()
					{
						//verify we have exactly one room
						GetUserWithRoomsSessionOwnedRoomsTest(delegate()
						{
							GetUserWithRoomsSessionOwnedRoomsWithPrivacyTest(delegate()
							{
							});
						});
					});
				});
			});
			


			////verify the data on the room
			//RoomManagerServiceAPI.GetRoomService
			////verify the room is the default room
			//RoomManagerServiceAPI.GetDefaultRoomService
			////update the room dna
			//RoomManagerServiceAPI.UpdateRoomDnaService
			////delete the room

			////verify we have no more rooms
			//RoomManagerServiceAPI.GetSessionOwnedRoomsService
			//RoomManagerServiceAPI.GetSessionOwnedRoomsWithPrivacyService
		}

		private List<RoomProperties> AssertNumberOfRoomsFromXml(int numberOfExpectedRooms, XmlDocument xmlResponse)
		{
			MockServerAssetRepository mockServerAssetRepo = new MockServerAssetRepository("<Item></Item>");
			List<RoomProperties> availableRooms = null;
			try
			{
				availableRooms = RoomXmlUtil.GetRoomsPropertiesFromXml(xmlResponse, mockServerAssetRepo);
			}
			catch (System.Exception ex)
			{
				
			}

			if (availableRooms.Count != numberOfExpectedRooms)
			{
				throw new System.Exception("we found rooms on an account thats not supposed to have any rooms in it!");
			}

			return availableRooms;
		}

		private void GetNewUserSessionOwnedRoomsTest(System.Action functionFinishedCallback)
		{
		    Action<XmlDocument> getSessionOwnedRoomsServiceCallback = delegate(XmlDocument xmlResponse)
		    {
				AssertNumberOfRoomsFromXml(0, xmlResponse);
				functionFinishedCallback();
		    };
		    RoomManagerServiceAPI.GetSessionOwnedRoomsService(mTestAccountId, getSessionOwnedRoomsServiceCallback);
		}

		private void GetNewUserSessionOwnedRoomsWithPrivacyTest(System.Action functionFinishedCallback)
		{
			Action<XmlDocument> getSessionOwnedRoomsServiceCallback = delegate(XmlDocument xmlResponse)
		    {
				AssertNumberOfRoomsFromXml(0, xmlResponse);
				functionFinishedCallback();
		    };
			RoomManagerServiceAPI.GetSessionOwnedRoomsWithPrivacyService(mTestAccountId, PrivacyLevel.Default, getSessionOwnedRoomsServiceCallback);
		}

		private void CreateNewRoom(System.Action functionFinishedCallback)
		{
			Action<XmlDocument> createRoomFinishedCallback = delegate(XmlDocument xmlResponse)
			{
				List<RoomProperties> roomsProperties = AssertNumberOfRoomsFromXml(1, xmlResponse);

				if(roomsProperties[0].RoomName != "test room" || 
					roomsProperties[0].RoomType != RoomType.GreenScreenRoom || 
					roomsProperties[0].RoomItemsXml.OuterXml != "<Items></Items>" ||
					roomsProperties[0].RoomId != null ||
					roomsProperties[0].PrivacyLevel != PrivacyLevel.Private ||
					roomsProperties[0].AccountCreatingRoom != mTestAccountId )
				{
					throw new System.Exception("We did not receive the expected results with this returned room.");
				}

				functionFinishedCallback();
			};
			RoomManagerServiceAPI.CreateNewRoomService(mTestAccountId, "test room", PrivacyLevel.Private, createRoomFinishedCallback);
		}

		private void GetUserWithRoomsSessionOwnedRoomsTest(System.Action functionFinishedCallback)
		{
			Action<XmlDocument> getSessionOwnedRoomsServiceCallback = delegate(XmlDocument xmlResponse)
			{
				List<RoomProperties> roomsProperties = AssertNumberOfRoomsFromXml(1, xmlResponse);

				if (roomsProperties[0].RoomName != "test room" ||
					roomsProperties[0].RoomType != RoomType.GreenScreenRoom ||
					roomsProperties[0].RoomItemsXml.OuterXml != "<Items></Items>" ||
					roomsProperties[0].RoomId != null ||
					roomsProperties[0].PrivacyLevel != PrivacyLevel.Private ||
					roomsProperties[0].AccountCreatingRoom != mTestAccountId)
				{
					throw new System.Exception("We did not receive the expected results with this returned room.");
				}

				functionFinishedCallback();
			};
			RoomManagerServiceAPI.GetSessionOwnedRoomsService(mTestAccountId, getSessionOwnedRoomsServiceCallback);
		}

		private void GetUserWithRoomsSessionOwnedRoomsWithPrivacyTest(System.Action functionFinishedCallback)
		{
			Action<XmlDocument> getSessionOwnedRoomsServiceCallbackPrivate = delegate(XmlDocument xmlResponse)
			{
				List<RoomProperties> roomsProperties = AssertNumberOfRoomsFromXml(1, xmlResponse);

				if (roomsProperties[0].RoomName != "test room" ||
					roomsProperties[0].RoomType != RoomType.GreenScreenRoom ||
					roomsProperties[0].RoomItemsXml.OuterXml != "<Items></Items>" ||
					roomsProperties[0].RoomId != null ||
					roomsProperties[0].PrivacyLevel != PrivacyLevel.Private ||
					roomsProperties[0].AccountCreatingRoom != mTestAccountId)
				{
					throw new System.Exception("We did not receive the expected results with this returned room.");
				}

				Action<XmlDocument> getSessionOwnedRoomsServiceCallbackPublic = delegate(XmlDocument xmlResponse2)
				{
					AssertNumberOfRoomsFromXml(0, xmlResponse2);

					Action<XmlDocument> getSessionOwnedRoomsServiceCallbackDefault = delegate(XmlDocument xmlResponse3)
					{
						List<RoomProperties> roomsProperties2 = AssertNumberOfRoomsFromXml(1, xmlResponse3);

						if (roomsProperties2[0].RoomName != "test room" ||
							roomsProperties2[0].RoomType != RoomType.GreenScreenRoom ||
							roomsProperties2[0].RoomItemsXml.OuterXml != "<Items></Items>" ||
							roomsProperties2[0].RoomId != null ||
							roomsProperties2[0].PrivacyLevel != PrivacyLevel.Private ||
							roomsProperties2[0].AccountCreatingRoom != mTestAccountId)
						{
							throw new System.Exception("We did not receive the expected results with this returned room.");
						}

						functionFinishedCallback();
					};
					//default should return us back all rooms regardless of privacy level
					RoomManagerServiceAPI.GetSessionOwnedRoomsWithPrivacyService(mTestAccountId, PrivacyLevel.Default, getSessionOwnedRoomsServiceCallbackDefault);
				};
				
				RoomManagerServiceAPI.GetSessionOwnedRoomsWithPrivacyService(mTestAccountId, PrivacyLevel.Public, getSessionOwnedRoomsServiceCallbackPublic);
			};
			RoomManagerServiceAPI.GetSessionOwnedRoomsWithPrivacyService(mTestAccountId, PrivacyLevel.Private, getSessionOwnedRoomsServiceCallbackPrivate);

		}

	}
}
