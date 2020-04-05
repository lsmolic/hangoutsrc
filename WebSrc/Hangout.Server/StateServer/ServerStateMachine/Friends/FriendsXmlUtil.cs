using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;

namespace Hangout.Server
{
    public static class FriendsXmlUtil
    {
        public static bool GetFriendsInfoFromXml(XmlDocument xml, out List<FacebookFriendInfo> friendInfos)
        {
			friendInfos = new List<FacebookFriendInfo>();
			
			XmlNode friendsXmlNode = xml.SelectSingleNode("Friends");
			if (friendsXmlNode != null)
            {
				XmlNodeList friendXmlNodes = friendsXmlNode.SelectNodes("Friend");
				foreach (XmlNode friendNode in friendXmlNodes)
				{
					XmlAttribute hangoutAccountIdAttribute = friendNode.Attributes["AccountId"];
					AccountId accountId = null;
					if (hangoutAccountIdAttribute != null && hangoutAccountIdAttribute.Value != string.Empty)
					{
						try
						{
							uint accountIdUint = Convert.ToUInt32(hangoutAccountIdAttribute.Value);
							accountId = new AccountId(accountIdUint);
						}
						catch (System.Exception ex)
						{
							//log here.. someone put a non-uint in for the accountId
							StateServerAssert.Assert(ex);
						}
					}
					string firstName = (friendNode.Attributes["FirstName"].Value);
					string lastName = (friendNode.Attributes["LastName"].Value);
					long fbAccountId = long.Parse(friendNode.Attributes["FBAccountId"].Value);
					string imageUrl = (friendNode.Attributes["PicSquare"].Value);

					friendInfos.Add(new FacebookFriendInfo(accountId, fbAccountId, firstName, lastName, imageUrl));
				}
			}
			else
			{
				return false;	
			}

            return true;
        }
    }
}
