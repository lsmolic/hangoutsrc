using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Hangout.Shared
{
	public static class AvatarXmlUtil
	{

		public static XmlDocument CreateAvatarDnaXml(string avatarName, List<ItemId> itemIds)
		{
			//setup a skeleton room xml node so we can fill out some information about the room
			StringBuilder avatarDnaXmlStringBuilder = new StringBuilder();
			avatarDnaXmlStringBuilder.Append("<AvatarDna AvatarName=\"" + avatarName + "\"><Items>");
			foreach (ItemId itemId in itemIds)
			{
				avatarDnaXmlStringBuilder.Append(itemId.ToString() + ",");
			}
			avatarDnaXmlStringBuilder.Append("</Items></AvatarDna>");

			XmlDocument avatarDnaXml = new XmlDocument();
			avatarDnaXml.LoadXml(avatarDnaXmlStringBuilder.ToString());
			return avatarDnaXml;
		}

		public static bool GetAvatarInfoFromAvatarXmlNode(XmlNode avatarXmlNode, out AvatarId avatarId, out List<ItemId> returnList)
		{
			avatarId = null;
			returnList = new List<ItemId>();

			try
			{
				XmlAttribute avatarIdAttribute = avatarXmlNode.Attributes["AvatarId"];
				if (avatarIdAttribute != null)
				{
					avatarId = new AvatarId((uint)Convert.ChangeType(avatarIdAttribute.Value, typeof(uint)));
				}

				return GetItemIdsFromAvatarXmlNode(avatarXmlNode, out returnList);
			}
			catch (System.Exception ex)
			{
				throw new Exception("Parsing exception in GetAvatarInfoFromAvatarXmlNode: " + ex);
			}
		}

		/// <summary>
		/// returns the list of item ids for the first avatar marked as "enabled" by the service response
		/// the expected format for an avatarXmlNode is:
		/// <Avatar AvatarId="1000000">
		///     <AvatarDna>
		///         <Items> </Items>
		///     </AvatarDna>
		/// </Avatar>
		/// </summary>
		public static bool GetItemIdsFromAvatarXmlNode(XmlNode avatarXmlNode, out List<ItemId> returnList)
		{
			returnList = new List<ItemId>();
			//parse out the list of items in an XmlNode
			XmlAttribute isAvatarEnabledXmlAttribute = avatarXmlNode.Attributes["IsEnabled"];
			if (isAvatarEnabledXmlAttribute != null && isAvatarEnabledXmlAttribute.Value == "1")
			{
				GetItemIdsFromAvatarDnaNode(avatarXmlNode, out returnList);
			}
			return true;
		}

		public static void GetItemIdsFromAvatarDnaNode(XmlNode avatarXmlNode, out List<ItemId> returnList)
		{
			returnList = new List<ItemId>();
			XmlNode avatarDnaNode = avatarXmlNode.SelectSingleNode("AvatarDna");
			if (avatarDnaNode == null)
			{
				throw new System.Exception("Malformed avatar xml: AvatarDna node missing " + avatarXmlNode.OuterXml);
			}
			//find the avatar data
			XmlNode isAvatarItemsXmlNode = avatarDnaNode.SelectSingleNode("Items");
			if (isAvatarItemsXmlNode != null)
			{

				//parse out the item ids separated by comma's in the xml node and convert them to ItemId objects
				string avatarItems = isAvatarItemsXmlNode.InnerText;
				string[] avatarItemStrings = avatarItems.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string avatarItemString in avatarItemStrings)
				{
					uint rawUIntValue = 0;

					if( !uint.TryParse(avatarItemString, out rawUIntValue) )
					{
						throw new Exception("Unable to parse Items list:\n" + isAvatarItemsXmlNode.OuterXml);
					}
					returnList.Add(new ItemId(rawUIntValue));
				}
			}
		}
	}
}
