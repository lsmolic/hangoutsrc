using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Hangout.Shared;

namespace Hangout.Server
{
    public static class AccountsXmlUtil
    {
        /// <summary>
        /// it is expected the Account Xml node be formated as such:
        ///  <Account accountId="" fbAccountid="" piAccountId="" nickname="">
		///		<AccountData>
		///			<UserProperties>
		///				<UserProperty Name="" Value=""/>
		///				<UserProperty Name="" Value=""/>
		///				<UserProperty Name="" Value=""/>
		///				...
		///			</UserProperties>
		///		</AccountData>
		///  </Account>
        /// </summary>
        /// <param name="xmlNode"></param>
        public static ServerAccount GetAccountFromXml(XmlNode xmlNode)
        {
			string accountIdString = string.Empty;
			string fbAccountIdString = string.Empty;
			string piAccountIdString = string.Empty;
			string piSecureKeyString = string.Empty;
			string nickNameString = string.Empty;
			string firstNameString = string.Empty;
			string lastNameString = string.Empty;

			if (!(XmlUtil.TryGetAttributeFromXml(ConstStrings.kAccountId, xmlNode, out accountIdString) &&  //uint
				XmlUtil.TryGetAttributeFromXml(ConstStrings.kFbAccountId, xmlNode, out fbAccountIdString) && //long
				XmlUtil.TryGetAttributeFromXml(ConstStrings.kPiAccountId, xmlNode, out piAccountIdString) && //
				XmlUtil.TryGetAttributeFromXml(ConstStrings.kPiSecureKey, xmlNode, out piSecureKeyString) && //
				XmlUtil.TryGetAttributeFromXml(ConstStrings.kNickName, xmlNode, out nickNameString) &&
				XmlUtil.TryGetAttributeFromXml(ConstStrings.kFirstName, xmlNode, out firstNameString) &&
				XmlUtil.TryGetAttributeFromXml(ConstStrings.kLastName, xmlNode, out lastNameString) )) //
			{
                StateServerAssert.Assert(new Exception("An account parameter is missing when creating a new account."));
				return null;
			}

            AccountId accountId = null;
            long fbAccountId = 0;
            try
            {
                uint accountIdUInt = Convert.ToUInt32(accountIdString);
                accountId = new AccountId(accountIdUInt);
                fbAccountId = Convert.ToInt64(fbAccountIdString);
            }
            catch (System.Exception ex)
            {
				StateServerAssert.Assert(new Exception("Error converting string to accountId type: " + ex.Data, ex));
				return null;
            }

			XmlNode accountDataNode = xmlNode.SelectSingleNode(ConstStrings.kAccountData);
			UserProperties userProperties = new UserProperties();

            if (accountDataNode != null)
            {
                XmlNode userPropertiesXmlNode = accountDataNode.SelectSingleNode(ConstStrings.kUserProperties);
                //if we can't find a UserProperties node, we can just set some default values
                if (userPropertiesXmlNode != null)
                {
                    if (!UserProperties.UserPropertiesFromXml(userPropertiesXmlNode, out userProperties))
                    {
                        StateServerAssert.Assert(new Exception("Error parsing user properties from xml: " + xmlNode.OuterXml));
                        return null;
                    }
                }
            }

            SetDefaultUserProperties(ref userProperties);

            return new ServerAccount(accountId, fbAccountId, piAccountIdString, piSecureKeyString, nickNameString, firstNameString, lastNameString, userProperties);
        }

        private static void SetDefaultUserProperties(ref UserProperties userProperties)
        {

            if (!userProperties.TryGetProperty(UserAccountProperties.FirstTimeUser))
            {
                userProperties.SetProperty(UserAccountProperties.FirstTimeUser, true);
            }

            if (!userProperties.TryGetProperty(UserAccountProperties.HasCompletedShoppingTutorial))
            {
                userProperties.SetProperty(UserAccountProperties.HasCompletedShoppingTutorial, false);
            }

            if (!userProperties.TryGetProperty(UserAccountProperties.HasPlayedFashionMiniGame))
            {
                userProperties.SetProperty(UserAccountProperties.HasPlayedFashionMiniGame, false);
            }

            if (!userProperties.TryGetProperty(UserAccountProperties.HasCompletedOpenMapTutorial))
            {
                userProperties.SetProperty(UserAccountProperties.HasCompletedOpenMapTutorial, false);
            }

            if (!userProperties.TryGetProperty(UserAccountProperties.HasCompletedDecorateTutorial))
            {
                userProperties.SetProperty(UserAccountProperties.HasCompletedDecorateTutorial, false);
            }

            if (!userProperties.TryGetProperty(UserAccountProperties.HasCompletedGetCashTutorial))
            {
                userProperties.SetProperty(UserAccountProperties.HasCompletedGetCashTutorial, false);
            }

            if (!userProperties.TryGetProperty(UserAccountProperties.HasCompleteMoveTutorial))
            {
                userProperties.SetProperty(UserAccountProperties.HasCompleteMoveTutorial, false);
            }





            if (!userProperties.TryGetProperty(UserAccountProperties.LastRoomId))
            {
				userProperties.SetProperty(UserAccountProperties.LastRoomId, GetRandomDefaultPublicRoomId());
            }
        
        }

		private static RoomId GetRandomDefaultPublicRoomId()
		{
			List<RoomId> defaultPublicRoomIds = new List<RoomId>();
			defaultPublicRoomIds.Add(new RoomId(2)); // Central park night
			defaultPublicRoomIds.Add(new RoomId(3)); // Miami night
			defaultPublicRoomIds.Add(new RoomId(6)); // Beach with chairs
			defaultPublicRoomIds.Add(new RoomId(7)); // Bahamas
			defaultPublicRoomIds.Add(new RoomId(8)); // Hawaii
			defaultPublicRoomIds.Add(new RoomId(21)); // Spanish Plaza
			int randomIndex = new System.Random().Next(defaultPublicRoomIds.Count);
			return defaultPublicRoomIds[randomIndex];
		}
        
        /// <summary>
		/// this function produces the following xml blob
		/// 	<AccountData>
		///			<UserProperties>
		///				<UserProperty Name="" Value=""/>
		///				<UserProperty Name="" Value=""/>
		///				<UserProperty Name="" Value=""/>
		///				...
		///			</UserProperties>
		///		</AccountData>
		/// </summary>
		public static XmlDocument CreateAccountDataXml(ServerAccount account)
		{
			UserProperties userProperties = account.UserProperties;
			XmlDocument userPropertiesXml = null;
			UserProperties.UserPropertiesToXml(userProperties, out userPropertiesXml);
			XmlNode userPropertiesXmlNode = userPropertiesXml.SelectSingleNode(ConstStrings.kUserProperties);
	
			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.Append("<" + ConstStrings.kAccountData + ">");
				if (userPropertiesXmlNode != null)
				{
					stringBuilder.Append(userPropertiesXmlNode.OuterXml);
				}
			stringBuilder.Append("</" + ConstStrings.kAccountData + ">");

			XmlDocument returnedXmlDocument = new XmlDocument();
			returnedXmlDocument.LoadXml(stringBuilder.ToString());
			return returnedXmlDocument;
		}

		public static string GetCommaSeperatedListOfAccountIdsFromList(IEnumerable<AccountId> accountIds)
		{
			string[] accountIdStringArray = Converters.Array<AccountId, string>(accountIds, delegate(AccountId accountId)
			{
				return accountId.ToString();
			});

			string accountIdsCommaSeperatedString = String.Join(",", accountIdStringArray);
			return accountIdsCommaSeperatedString;
		}
    }
}
