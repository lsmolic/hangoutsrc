using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Server;

namespace Hangout.Shared.UnitTest
{
	/// <summary>
	/// Inherit from this class for any test fixtures that test objects that use the FashionMinigameServiceAPI
	/// </summary>
    [TestFixture]
    public class MockFashionSuite : IDisposable
    {
		private readonly List<IReceipt> mCleanThisUp = new List<IReceipt>();

		public MockFashionSuite()
		{
			mCleanThisUp.Add(FashionMinigameServiceAPI.MockGetGameData(MockGetServerData));
			mCleanThisUp.Add(FashionMinigameServiceAPI.MockSetGameData(MockSetServerData));
		}

		private readonly Dictionary<uint, Dictionary<string, string>> mMockUserGameDataDB = new Dictionary<uint, Dictionary<string, string>>();
		protected virtual void MockGetServerData(ServerAccount account, string[] dataKeys, Action<XmlDocument> result)
		{
			XmlDocument resultDoc = new XmlDocument();
			XmlElement root = resultDoc.CreateElement("DataKeys");
			if (mMockUserGameDataDB.ContainsKey(account.AccountId.Value))
			{
				Dictionary<string, string> userData = mMockUserGameDataDB[account.AccountId.Value];
				foreach (string dataKey in dataKeys)
				{
					string dataValue;
					if (userData.TryGetValue(dataKey, out dataValue))
					{
						XmlElement dataNode = resultDoc.CreateElement("DataKey");

						XmlAttribute dataKeyAttribute = resultDoc.CreateAttribute("KeyName");
						dataKeyAttribute.InnerText = dataKey;
						dataNode.Attributes.Append(dataKeyAttribute);

						dataNode.InnerText = dataValue;
						root.AppendChild(dataNode);
					}
				}
			}

			resultDoc.AppendChild(root);
			result(resultDoc);
		}

		protected virtual void MockSetServerData(ServerAccount account, string dataKey, string dataValue, Action<XmlDocument> result)
		{
			if (!mMockUserGameDataDB.ContainsKey(account.AccountId.Value))
			{
				mMockUserGameDataDB[account.AccountId.Value] = new Dictionary<string, string>();
			}
			mMockUserGameDataDB[account.AccountId.Value][dataKey] = dataValue;
			XmlDocument resultXml = new XmlDocument();
			resultXml.LoadXml("<Success>true</Success>");
			result(resultXml);
		}

		public virtual void Dispose()
		{
			mCleanThisUp.ForEach(delegate(IReceipt r) { r.Exit(); });
		}

		// Don't really know what's up with the payment info strings, so I just threw some GUIDs in there, hopefully that will be fine for these tests since those fields are not used
		private readonly ServerAccount mMockServerAccount = new ServerAccount(new AccountId(1000003u), 17500116L, "{60F01A2C-AEDE-4a61-BD4F-4ADABE80E00F}", "{46D39107-C947-41da-A11E-B0B4DFF26690}", "MockUser", "mein", "schmeer", new UserProperties());
		protected ServerAccount MockServerAccount
		{
			get { return mMockServerAccount; }
		}


		[Test]
		public void FashionMinigameServiceAPIMockVerification()
		{
			bool callbackExecuted = false;
			FashionMinigameServiceAPI.SetGameData(mMockServerAccount, "Fairytales", "Goldielocks,Robin Hood,Cinderella", delegate(XmlDocument xml) { callbackExecuted = true; });
			FashionMinigameServiceAPI.SetGameData(mMockServerAccount, "Experience", "123456789", delegate(XmlDocument xml) { });
			Assert.IsTrue(callbackExecuted, "Something's wrong with the MockFashionSuite, the SetGameData callback isn't happening immediatedly");
			
			callbackExecuted = false;
			FashionMinigameServiceAPI.GetGameData(mMockServerAccount, new string[] { "Fairytales", "Experience", "KeyNotFoundEdgeCase" }, delegate(XmlDocument xml)
			{
				callbackExecuted = true;

				Assert.AreEqual("Robin Hood", xml.SelectSingleNode("//DataKey[@KeyName='Fairytales']").InnerText.Split(',')[1]);
				Assert.AreEqual(123456789, int.Parse(xml.SelectSingleNode("//DataKey[@KeyName='Experience']").InnerText));
			});
			Assert.IsTrue(callbackExecuted, "Something's wrong with the MockFashionSuite, the GetGameData callback isn't happening immediatedly");
		}
    }
}
