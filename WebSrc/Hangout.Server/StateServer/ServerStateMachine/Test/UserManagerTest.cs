using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Server;
using System.Xml;

namespace Hangout.Shared.UnitTest
{
	public class TestUsersManager : UsersManager
	{
		public XmlDocument mTestXmlResponseForGetUserService = null;
		public XmlDocument mTestXmlDocumentResponseForCreateUserService = null;

		public TestUsersManager(ServerStateMachine serverStateMachine, XmlDocument testXmlResponseForGetUserService, XmlDocument testXmlDocumentResponseForCreateUserService)
			: base(serverStateMachine)
		{
			mTestXmlResponseForGetUserService = testXmlResponseForGetUserService;
			mTestXmlDocumentResponseForCreateUserService = testXmlDocumentResponseForCreateUserService;
		}

		protected override void CallCreateServerAccountService(string fbAccountId, string fbSessionKey, string nickName, string firstName, string lastName, string userIpAddress, string campaignId, string referrerId, Action<XmlDocument> createAccountForUserCallback)
		{
			createAccountForUserCallback(mTestXmlDocumentResponseForCreateUserService);
		}

		protected override void CallGetServerAccountForFacebookIdService(string fbAccountId, Action<XmlDocument> getAccountForFacebookIdCallback)
		{
			getAccountForFacebookIdCallback(mTestXmlResponseForGetUserService);
		}

        protected override void SaveCurrentAccountData(ServerAccount serverAccount)
        {
        }

		public override void CreatePaymentItemAccountForUser(ServerAccount serverAccount, string userIpAddress, Action<ServerAccount> accountForUserCallback)
		{
			accountForUserCallback(serverAccount);
		}
	}

	[TestFixture]
	public class UserManagerTest
	{
		[Test]
		public void GetAccountForUserTest()
		{
			XmlDocument getUserServiceXmlResponse = new XmlDocument();
			string newUserResponseFromGetUserService = "<Accounts></Accounts>";
			getUserServiceXmlResponse.LoadXml(newUserResponseFromGetUserService);

			XmlDocument createUserServiceXmlResponse = new XmlDocument();
			string newUserResponseFromCreateUserService = "<Accounts> <Account AccountId=\"1\" FBAccountId=\"12345\" PIAccountId=\"123\" PISecureKey=\"gagaga\" NickName=\"mein schmeer\" FirstName=\"samir\" LastName=\"awesome\"/> </Accounts>";
			createUserServiceXmlResponse.LoadXml(newUserResponseFromCreateUserService);
			
			ServerAccount createUserServiceServerAccount = new ServerAccount(new AccountId(1), (long)12345, string.Empty, string.Empty, "mein schmeer", "samir", "awesome", null);
			ServerStateMachine serverStateMachine = new TestServerStateMachine();
			TestUsersManager userManager = new TestUsersManager(serverStateMachine, getUserServiceXmlResponse, createUserServiceXmlResponse);

			userManager.GetAccountForUser("12345", "qwerty", "mein schmeer", "samir", "awesome", "123.456.7.890", "MockCampaign", "MockFacebookReferredId", delegate(ServerAccount serverAccount)
			{
				Assert.IsTrue(serverAccount.Nickname == "mein schmeer", "NickName does not match");
				Assert.IsTrue(serverAccount.FacebookAccountId.ToString() == "12345", "FacebookAccountId does not match");
			});
		}
	}
}
