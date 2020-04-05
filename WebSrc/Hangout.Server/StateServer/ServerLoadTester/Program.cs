using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hangout.Server;
using Hangout.Shared;

namespace ServerLoadTester
{
	class Program
	{
		static void Main(string[] args)
		{
			ServerStateMachine serverStateMachine = new ServerStateMachine();
			serverStateMachine.Init(delegate()
			{
				ServerAccount account = new ServerAccount(new AccountId(0), 0, "", "", "mockuser", "mock", "user", new UserProperties());

				UserManagerLoadTesting userManagerLoadTesting = new UserManagerLoadTesting();
				RoomManagerLoadTesting roomManagerLoadTesting = new RoomManagerLoadTesting(serverStateMachine, account);
				AvatarManagerLoadTesting avatarManagerLoadTesting = new AvatarManagerLoadTesting(serverStateMachine, account);
			});
		}


	}
}
