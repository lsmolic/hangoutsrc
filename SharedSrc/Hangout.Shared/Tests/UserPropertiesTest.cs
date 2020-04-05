using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace Hangout.Shared.UnitTest
{
	[TestFixture]
	public class UserPropertiesTest
	{
		private UserProperties mTestObjectProperties = new UserProperties();
		public UserPropertiesTest()
		{
			mTestObjectProperties.SetProperty(UserAccountProperties.FirstTimeUser, true);
			mTestObjectProperties.SetProperty(UserAccountProperties.HasCompletedShoppingTutorial, true);
		}

		//[Test]
		//public void UserPropertiesXmlSerializationTest()
		//{
		//    XmlDocument serializedUserPropertiesXmlDocument = null;
		//    UserProperties.UserPropertiesToXml(mTestObjectProperties, out serializedUserPropertiesXmlDocument);

		//    UserProperties userPropertiesToDeserialize = null;
		//    UserProperties.UserPropertiesFromXml(serializedUserPropertiesXmlDocument.FirstChild, out userPropertiesToDeserialize);

		//    XmlDocument serializedUserPropertiesXmlDocumentPart2 = null;
		//    UserProperties.UserPropertiesToXml(userPropertiesToDeserialize, out serializedUserPropertiesXmlDocumentPart2);

		//    UserProperties userPropertiesToDeserializePart2 = null;
		//    UserProperties.UserPropertiesFromXml(serializedUserPropertiesXmlDocumentPart2.FirstChild, out userPropertiesToDeserializePart2);

		//    Assert.IsTrue(userPropertiesToDeserialize == userPropertiesToDeserializePart2, "Serialization fail!  UserProperties objects not equal!");
		//}

		//[Test]
		//public void UserPropertiesBinaryFormatterSerializationTest()
		//{
		//    Message mockMessageSerialize = new Message();
		//    mockMessageSerialize.LoginMessage(new List<object>() { mTestObjectProperties });
			
		//    BinaryFormatter binaryFormatter = new BinaryFormatter();
		//    MemoryStream ms = new MemoryStream();
		//    binaryFormatter.Serialize(ms, mockMessageSerialize);

		//    ms.Position = 0;

		//    Message mockMessageDeserialize = (Message)binaryFormatter.Deserialize(ms);

		//    UserProperties deserializedUserProperties = (UserProperties)mockMessageDeserialize.Data[0];
		//}
	}
}
