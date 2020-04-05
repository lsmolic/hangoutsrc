using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hangout.Server;
using System.ComponentModel;
using System.Diagnostics;
using Hangout.Shared;

namespace ServerLoadTester
{
	public class AvatarManagerLoadTesting
	{
		private static int mNumberOfAvatarsToCreate = 20;
		private static int mNumberOfAvatarsCreated = 0;
		private static int mNumberOfAvatarGotten = 0;
		private const float mAvatarsPerSecond = 1.0f;
		private List<int> mAvatarsCreated = new List<int>();

		private class TestAvatarManager : AvatarManager
		{
			public TestAvatarManager(ServerStateMachine serverStateMachine)
				: base(serverStateMachine)
			{ }


		}


		public AvatarManagerLoadTesting(ServerStateMachine serverStateMachine, ServerAccount serverAccount)
		{
			Console.WriteLine("Start Rooms Tests...");

			TestAvatarManager testAvatarManager = new TestAvatarManager(serverStateMachine);

			ZoneId zoneId = new ZoneId(0);
			AvatarId defaultAvatarId = new AvatarId(0);

			LoadTestCreateNewAvatar(testAvatarManager, zoneId, serverAccount, defaultAvatarId, delegate()
			{
				LoadTestGetAvatar(testAvatarManager, zoneId, serverAccount, defaultAvatarId, delegate()
				{
					//LoadTestJoinRoom(testAvatarManager, delegate()
					//{
					//});
				});
			});


			//Console.WriteLine("Finished All Rooms Tests!");
		}

		private void LoadTestCreateNewAvatar(TestAvatarManager testAvatarManager, ZoneId zoneId, ServerAccount serverAccount, AvatarId defaultAvatarId, System.Action finishedCreatingAllAvatars)
		{
			// create a writer and open the file
			System.IO.TextWriter tw = new System.IO.StreamWriter("C:/TestCreateNewAvatarLoadTest.txt");

			Console.WriteLine("Creating " + mNumberOfAvatarsToCreate + " avatars...");
			tw.WriteLine("Creating " + mNumberOfAvatarsToCreate + " avatars...");

			Stopwatch runEntireTestStopWatch = new Stopwatch();
			runEntireTestStopWatch.Start();

			System.Action createdAvatarsDoneCallback = delegate()
			{
				runEntireTestStopWatch.Stop();
				tw.WriteLine("Total time it took to create " + mNumberOfAvatarsToCreate + " of avatars: " + runEntireTestStopWatch.Elapsed.ToString());
				Console.WriteLine("Total time it took to create " + mNumberOfAvatarsToCreate + " of avatars: " + runEntireTestStopWatch.Elapsed.ToString());

				// close the stream
				tw.Close();
				finishedCreatingAllAvatars();
			};

			BackgroundWorker bgWorker = new BackgroundWorker();
			bgWorker.DoWork += new DoWorkEventHandler(delegate(object o, DoWorkEventArgs args)
			{
				for (int i = 0; i < mNumberOfAvatarsToCreate; ++i)
				{
					LoadTestCreateNewAvatarCallback(testAvatarManager, zoneId, serverAccount, defaultAvatarId, i, tw, createdAvatarsDoneCallback);
					System.Threading.Thread.Sleep((int)(mAvatarsPerSecond * 1000));
				}
			});

			bgWorker.RunWorkerAsync();
		}

		private void LoadTestCreateNewAvatarCallback(TestAvatarManager testAvatarManager, ZoneId zoneId, ServerAccount serverAccount, AvatarId defaultAvatarId, int avatarIndex, System.IO.TextWriter tw, System.Action createdAvatarsDoneCallback)
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			testAvatarManager.CreateNewAvatarForAccount(Guid.Empty, zoneId, serverAccount, defaultAvatarId, 
				delegate(bool room)
				{
					stopWatch.Stop();
					tw.WriteLine("It took " + stopWatch.Elapsed.ToString() + " to create avatar#: " + avatarIndex);
					Console.WriteLine("It took " + stopWatch.Elapsed.ToString() + " to create avatar#: " + avatarIndex);
					mNumberOfAvatarsCreated++;
					mAvatarsCreated.Add(avatarIndex);

					if (mNumberOfAvatarsToCreate == mNumberOfAvatarsCreated)
					{
						createdAvatarsDoneCallback();
					}
				});
		}

		private void LoadTestGetAvatar(TestAvatarManager testAvatarManager, ZoneId zoneId, ServerAccount serverAccount, AvatarId defaultAvatarId, System.Action getAvatarsDoneCallback)
		{
			// create a writer and open the file
			System.IO.TextWriter tw = new System.IO.StreamWriter("C:/TestGetAvatarLoadTest.txt");

			Console.WriteLine("Getting " + mNumberOfAvatarsToCreate + " avatars...");
			tw.WriteLine("Getting " + mNumberOfAvatarsToCreate + " avatars...");

			Stopwatch runEntireTestStopWatch = new Stopwatch();
			runEntireTestStopWatch.Start();

			System.Action gotAllAvatarsDoneCallback = delegate()
			{
				runEntireTestStopWatch.Stop();
				tw.WriteLine("Total time it took to get " + mNumberOfAvatarsToCreate + " of avatars: " + runEntireTestStopWatch.Elapsed.ToString());
				Console.WriteLine("Total time it took to get " + mNumberOfAvatarsToCreate + " of avatars: " + runEntireTestStopWatch.Elapsed.ToString());

				// close the stream
				tw.Close();

				getAvatarsDoneCallback();
			};

			BackgroundWorker bgWorker = new BackgroundWorker();
			bgWorker.DoWork += new DoWorkEventHandler(delegate(object o, DoWorkEventArgs args)
			{
				int index = 0;
				foreach(int avatarId in mAvatarsCreated)
				{
					LoadTestGetAvatarCallback(testAvatarManager, zoneId, serverAccount, defaultAvatarId, index, tw, gotAllAvatarsDoneCallback);
					System.Threading.Thread.Sleep((int)(mAvatarsPerSecond * 1000));
					++index;
				}
			});

			bgWorker.RunWorkerAsync();
		}

		private void LoadTestGetAvatarCallback(TestAvatarManager testAvatarManager, ZoneId zoneId, ServerAccount serverAccount, AvatarId defaultAvatarId, int index, System.IO.TextWriter tw, System.Action gotAvatarsDoneCallback)
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			testAvatarManager.GetAvatar(Guid.Empty, zoneId, serverAccount, defaultAvatarId, 
				delegate(bool gotAvatar)
				{
					stopWatch.Stop();
					tw.WriteLine("It took " + stopWatch.Elapsed.ToString() + " to get avatar#: " + index);
					Console.WriteLine("It took " + stopWatch.Elapsed.ToString() + " to get avatar#: " + index);
					mNumberOfAvatarGotten++;

					if (mNumberOfAvatarsToCreate == mNumberOfAvatarGotten)
					{
						gotAvatarsDoneCallback();
					}
				});
		}

	}
}
