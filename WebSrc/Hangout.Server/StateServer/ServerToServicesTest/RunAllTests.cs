using System;
using System.Collections.Generic;
using System.Text;

namespace ServerToServicesTest
{
	public class RunAllTests : ITest
	{

		private RoomTests mRoomTests = null;

		public RunAllTests()
		{
			mRoomTests = new RoomTests();
		}

		public void RunTests()
		{
			mRoomTests.RunTests();

		}


	}
}
