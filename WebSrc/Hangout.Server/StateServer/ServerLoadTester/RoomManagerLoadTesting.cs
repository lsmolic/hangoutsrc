using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hangout.Server;
using System.Diagnostics;
using Hangout.Shared;
using System.Xml;
using System.Threading;
using System.ComponentModel;

namespace ServerLoadTester
{
	public class RoomManagerLoadTesting
	{
		private const float mRoomsPerSecond = 1.0f;
		private static int mNumberOfRoomsToCreate = 20;
		private static int mNumberOfRoomsCreated = 0;
		private static int mNumberOfRoomsGotten = 0;
		private List<RoomId> mRoomIdsCreated = new List<RoomId>();

		private class TestRoomManager : RoomManager
		{
			ServerAccount mMockServerAccount = null;
			public TestRoomManager(ServerStateMachine serverStateMachine, ServerAccount mockServerAccount)
				: base(serverStateMachine)
			{
				mMockServerAccount = mockServerAccount;
			}

			//the unlucky dude with an accountId of 0 is gonna gets LOTS of rooms!
			protected override void CreateNewRoomInDatabaseService(Guid sessionId, string roomName, PrivacyLevel privacyLevel, System.Action<XmlDocument> createRoomFinishedCallback)
			{
				RoomManagerServiceAPI.CreateNewRoomService(mMockServerAccount.AccountId, roomName, privacyLevel, createRoomFinishedCallback);
			}

			protected override void GetRoomFromRoomId(RoomId roomId, System.Action<IServerDistributedRoom> getRoomCallback)
			{
				TestGetRoom(roomId, getRoomCallback);
			}

			public void TestGetRoom(RoomId roomId, System.Action<IServerDistributedRoom> getRoomCallback)
			{
				this.GetRoomFromRoomIdService(roomId, getRoomCallback);
			}

			//public void DeleteRoom()
			//{
			//    this.JoinRoom()
			//}
		}

		public RoomManagerLoadTesting(ServerStateMachine serverStateMachine, ServerAccount serverAccount)
		{
			Console.WriteLine("Start Rooms Tests...");
			//			TestServerStateMachine testServerStateMachine = new TestServerStateMachine();
			TestRoomManager testRoomManager = new TestRoomManager(serverStateMachine, serverAccount);

			LoadTestCreateNewRooms(testRoomManager, delegate()
			{
				LoadTestGetRoom(testRoomManager, delegate()
				{
					//LoadTestJoinRoom(testAvatarManager, delegate()
					//{
					//});
				});
			});


			//Console.WriteLine("Finished All Rooms Tests!");
		}

		private void LoadTestCreateNewRooms(TestRoomManager testRoomManager, System.Action finishedCreatingAllRooms)
		{
			// create a writer and open the file
			System.IO.TextWriter tw = new System.IO.StreamWriter("C:/TestCreateNewRoomLoadTest.txt");

			Console.WriteLine("Creating " + mNumberOfRoomsToCreate + " rooms...");
			tw.WriteLine("Creating " + mNumberOfRoomsToCreate + " rooms...");

			Stopwatch runEntireTestStopWatch = new Stopwatch();
			runEntireTestStopWatch.Start();

			System.Action createdRoomsDoneCallback = delegate()
			{
				runEntireTestStopWatch.Stop();
				tw.WriteLine("Total time it took to create " + mNumberOfRoomsToCreate + " of rooms: " + runEntireTestStopWatch.Elapsed.ToString());
				Console.WriteLine("Total time it took to create " + mNumberOfRoomsToCreate + " of rooms: " + runEntireTestStopWatch.Elapsed.ToString());

				// close the stream
				tw.Close();
				finishedCreatingAllRooms();
			};

			BackgroundWorker bgWorker = new BackgroundWorker();
			bgWorker.DoWork += new DoWorkEventHandler(delegate(object o, DoWorkEventArgs args)
				{
					for (int i = 0; i < mNumberOfRoomsToCreate; ++i)
					{
						LoadTestCreateNewRoomCallback(testRoomManager, i, tw, createdRoomsDoneCallback);
						System.Threading.Thread.Sleep((int)(mRoomsPerSecond * 1000));
					}
				});

			bgWorker.RunWorkerAsync();

		}

		private void LoadTestCreateNewRoomCallback(TestRoomManager testRoomManager, int roomIndex, System.IO.TextWriter tw, System.Action createdRoomsDoneCallback)
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			testRoomManager.CreateNewRoomInDatabase(Guid.Empty, "test room #" + roomIndex, Hangout.Shared.RoomType.GreenScreenRoom, Hangout.Shared.PrivacyLevel.Private,
				delegate(IServerDistributedRoom room)
				{
					stopWatch.Stop();
					tw.WriteLine("It took " + stopWatch.Elapsed.ToString() + " to create room: " + room.RoomName);
					Console.WriteLine("It took " + stopWatch.Elapsed.ToString() + " to create room: " + room.RoomName);
					mNumberOfRoomsCreated++;
					mRoomIdsCreated.Add(room.RoomId);

					if(mNumberOfRoomsToCreate == mNumberOfRoomsCreated)
					{
						createdRoomsDoneCallback();
					}
				});
		}

		private void LoadTestGetRoom(TestRoomManager testRoomManager, System.Action getRoomDoneCallback)
		{
			// create a writer and open the file
			System.IO.TextWriter tw = new System.IO.StreamWriter("C:/TestGetRoomLoadTest.txt");

			Console.WriteLine("Getting " + mNumberOfRoomsToCreate + " rooms...");
			tw.WriteLine("Getting " + mNumberOfRoomsToCreate + " rooms...");

			Stopwatch runEntireTestStopWatch = new Stopwatch();
			runEntireTestStopWatch.Start();

			System.Action gotRoomsDoneCallback = delegate()
			{
				runEntireTestStopWatch.Stop();
				tw.WriteLine("Total time it took to get " + mNumberOfRoomsToCreate + " of rooms: " + runEntireTestStopWatch.Elapsed.ToString());
				Console.WriteLine("Total time it took to get " + mNumberOfRoomsToCreate + " of rooms: " + runEntireTestStopWatch.Elapsed.ToString());

				// close the stream
				tw.Close();

				getRoomDoneCallback();
			};

			BackgroundWorker bgWorker = new BackgroundWorker();
			bgWorker.DoWork += new DoWorkEventHandler(delegate(object o, DoWorkEventArgs args)
			{
				foreach(RoomId roomId in mRoomIdsCreated)
				{
					LoadTestGetRoomCallback(testRoomManager, roomId, tw, gotRoomsDoneCallback);
					System.Threading.Thread.Sleep((int)(mRoomsPerSecond * 1000));
				}
			});

			bgWorker.RunWorkerAsync();
		}

		private void LoadTestGetRoomCallback(TestRoomManager testRoomManager, RoomId roomId, System.IO.TextWriter tw, System.Action gotRoomsDoneCallback)
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			testRoomManager.TestGetRoom(roomId, 
				delegate(IServerDistributedRoom room)
				{
					stopWatch.Stop();
					tw.WriteLine("It took " + stopWatch.Elapsed.ToString() + " to get room: " + room.RoomName);
					Console.WriteLine("It took " + stopWatch.Elapsed.ToString() + " to get room: " + room.RoomName);
					mNumberOfRoomsGotten++;

					if (mNumberOfRoomsToCreate == mNumberOfRoomsGotten)
					{
						gotRoomsDoneCallback();
					}
				});
		}

		//private void LoadTestJoinRoom(TestRoomManager testAvatarManager, System.Action joinRoomDoneCallback)
		//{
		//    // create a writer and open the file
		//    System.IO.TextWriter tw = new System.IO.StreamWriter("C:/TestJoinRoomLoadTest.txt");

		//    Console.WriteLine("Joining " + mNumberOfRoomsToCreate + " rooms...");
		//    tw.WriteLine("Joining " + mNumberOfRoomsToCreate + " rooms...");

		//    Stopwatch runEntireTestStopWatch = new Stopwatch();
		//    runEntireTestStopWatch.Start();

		//    System.Action joinRoomsDoneCallback = delegate()
		//    {
		//        runEntireTestStopWatch.Stop();
		//        tw.WriteLine("Total time it took to get " + mNumberOfRoomsToCreate + " of rooms: " + runEntireTestStopWatch.Elapsed.ToString());
		//        Console.WriteLine("Total time it took to get " + mNumberOfRoomsToCreate + " of rooms: " + runEntireTestStopWatch.Elapsed.ToString());

		//        // close the stream
		//        tw.Close();

		//        joinRoomDoneCallback();
		//    };

		//    BackgroundWorker bgWorker = new BackgroundWorker();
		//    bgWorker.DoWork += new DoWorkEventHandler(delegate(object o, DoWorkEventArgs args)
		//    {
		//        foreach (RoomId roomId in mRoomIdsCreated)
		//        {
		//            LoadTestJoinRoomCallback(testAvatarManager, roomId, tw, gotRoomsDoneCallback);
		//            System.Threading.Thread.Sleep((int)(mRoomsPerSecond * 1000));
		//        }
		//    });

		//    bgWorker.RunWorkerAsync();
		//}

		//private void LoadTestJoinRoomCallback(TestRoomManager testAvatarManager)
		//{
		//    Stopwatch stopWatch = new Stopwatch();
		//    stopWatch.Start();
		//    testAvatarManager.join(roomId,
		//        delegate(IServerDistributedRoom room)
		//        {
		//            stopWatch.Stop();
		//            tw.WriteLine("It took " + stopWatch.Elapsed.ToString() + " to get room: " + room.RoomName);
		//            Console.WriteLine("It took " + stopWatch.Elapsed.ToString() + " to get room: " + room.RoomName);
		//            mNumberOfRoomsGotten++;

		//            if (mNumberOfRoomsToCreate == mNumberOfRoomsGotten)
		//            {
		//                gotRoomsDoneCallback();
		//            }
		//        });
		//}
	
	}
}
