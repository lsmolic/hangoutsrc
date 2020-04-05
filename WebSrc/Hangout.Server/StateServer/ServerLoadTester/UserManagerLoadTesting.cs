using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hangout.Server;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace ServerLoadTester
{
	public class UserManagerLoadTesting
	{
		public class TestUsersManager : UsersManager
		{
			public XmlDocument  mTestXmlResponseForGetUserService = null;
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

		public UserManagerLoadTesting()
		{
			TestCreateNewAccount();
		}

		/// <summary>
		/// note: this does not call services
		/// </summary>
		private void TestCreateNewAccount()
		{
			XmlDocument getUserServiceXmlResponse = new XmlDocument();
			string newUserResponseFromGetUserService = "<Accounts></Accounts>";
			getUserServiceXmlResponse.LoadXml(newUserResponseFromGetUserService);

			XmlDocument createUserServiceXmlResponse = new XmlDocument();
			string newUserResponseFromCreateUserService = "<Accounts> <Account AccountId=\"1\" FBAccountId=\"12345\" PIAccountId=\"123\" PISecureKey=\"gagaga\" NickName=\"mein schmeer\" FirstName=\"samir\" LastName=\"awesome\"/> </Accounts>";
			createUserServiceXmlResponse.LoadXml(newUserResponseFromCreateUserService);

			ServerStateMachine serverStateMachine = new TestServerStateMachine();
			TestUsersManager userManager = new TestUsersManager(serverStateMachine, getUserServiceXmlResponse, createUserServiceXmlResponse);

			// create a writer and open the file
			System.IO.TextWriter tw = new System.IO.StreamWriter("C:/TestCreateNewAccountLoadTest10000Users.txt");

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			int numberOfAccountsToCreate = 100;
			for (int i = 0; i < numberOfAccountsToCreate; ++i)
			{
				CreateNewAccount(userManager, i, tw);
			}

			stopWatch.Stop();
			tw.WriteLine("Total time it took to create " + numberOfAccountsToCreate + " of users: " + stopWatch.Elapsed.ToString());

			// close the stream
			tw.Close();
		}

		private void CreateNewAccount(UsersManager usersManager, int userIndex, System.IO.TextWriter tw)
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			usersManager.CreateAccountForUser(userIndex.ToString(), "", "userNumber" + userIndex.ToString(), "mock", "user", "127.0.01", "loadTesting", "mockReferralId",
				delegate(ServerAccount serverAccount)
				{
					stopWatch.Stop();
					tw.WriteLine("It took " + stopWatch.Elapsed.ToString() + " to create user # " + userIndex);
				});

		}
	}
}
