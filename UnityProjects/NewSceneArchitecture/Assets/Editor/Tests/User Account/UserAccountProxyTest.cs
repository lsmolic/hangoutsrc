using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using Hangout.Shared.UnitTest;

namespace Hangout.Client.UnitTest
{
	[TestFixture]
	public class UserAccountProxyTest
	{
		UserAccountProxy mTestUserAccountProxy = null;

		public UserAccountProxyTest()
		{
			UserProperties userProperties = new UserProperties();

			userProperties.SetProperty(UserAccountProperties.FirstTimeUser, true);
			userProperties.SetProperty(UserAccountProperties.HasCompletedShoppingTutorial, false);

			mTestUserAccountProxy = new UserAccountProxy(userProperties);
		}

		[Test]
		public void TryGetValueSuccessTest()
		{
			bool isFirstTimeUser = false;
			Assert.IsTrue(mTestUserAccountProxy.TryGetAccountProperty<bool>(UserAccountProperties.FirstTimeUser, ref isFirstTimeUser));
			Assert.IsTrue(isFirstTimeUser, "First Time User should be true");

			bool hasBeenShopping = true;
			Assert.IsTrue(mTestUserAccountProxy.TryGetAccountProperty<bool>(UserAccountProperties.HasCompletedShoppingTutorial, ref hasBeenShopping));
			Assert.IsTrue(!hasBeenShopping, "Has Been Shopping should be false");
		}

		[Test]
		public void TryGetValueFailTest()
		{
			bool hasPlayedMiniGame = false;
			Assert.IsTrue(!mTestUserAccountProxy.TryGetAccountProperty<bool>(UserAccountProperties.HasPlayedFashionMiniGame, ref hasPlayedMiniGame));
		}

		[Test]
		public void TryGetValueInvalidCastTest()
		{
			bool caughtException = false;
			try
			{
				int isFirstTimeUser = -1;
				mTestUserAccountProxy.TryGetAccountProperty<int>(UserAccountProperties.FirstTimeUser, ref isFirstTimeUser);
			}
			catch (System.Exception)
			{
				caughtException = true;
			}
			Assert.IsTrue(caughtException, "Invalid cast from bool to int for the FirstTimeUser property");
		}

	}
}
