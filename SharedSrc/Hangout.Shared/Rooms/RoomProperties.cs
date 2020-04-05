using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Hangout.Shared
{
	public class RoomProperties
	{
		private AccountId mAccountCreatingRoom = null;
		public Hangout.Shared.AccountId AccountCreatingRoom
		{
			get { return mAccountCreatingRoom; }
		}
		private string mRoomName = string.Empty;
		public string RoomName
		{
			get { return mRoomName; }
		}
		private RoomId mRoomId = null;
		public Hangout.Shared.RoomId RoomId
		{
			get { return mRoomId; }
		}
		private RoomType mRoomType = RoomType.Default;
		public Hangout.Shared.RoomType RoomType
		{
			get { return mRoomType; }
		}
		private PrivacyLevel mPrivacyLevel = PrivacyLevel.Default;
		public Hangout.Shared.PrivacyLevel PrivacyLevel
		{
			get { return mPrivacyLevel; }
		}
		private XmlNode mRoomItemsXml = null;
		public System.Xml.XmlNode RoomItemsXml
		{
			get { return mRoomItemsXml; }
		}
		public RoomProperties(AccountId accountCreatingRoom, string roomName, RoomId roomId, RoomType roomType, PrivacyLevel privacyLevel, XmlNode roomItemsXml)
		{
			mAccountCreatingRoom = accountCreatingRoom;
			mRoomName = roomName;
			mRoomId = roomId;
			mRoomType = roomType;
			mPrivacyLevel = privacyLevel;
			mRoomItemsXml = roomItemsXml;
		}
	}
}
