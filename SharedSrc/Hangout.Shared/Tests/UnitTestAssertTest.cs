using System.Collections;

// Note: All [Test]s need to be public methods or they will be ignored by Unitest
namespace Hangout.Shared.UnitTest
{
	[TestFixture]
	public class UnitestAssertTest
	{
		[Test]
		public void AssertsPassWhenTheyShould()
		{
			Assert.IsTrue(true);
			Assert.IsFalse(false);
			Assert.AreEqual(1, 1);
			Assert.AreNotEqual("foo", "bar");
			Assert.IsNotWithin(1.0f, 1.2f, 0.15f);
			Assert.IsWithin(0.0f, 1.0f, 1.1f);
			Assert.IsEmptyString("");
			Assert.IsEmptyString(null);
			Assert.IsNotEmptyString("foo");
			Assert.ContainsString("foobar", "oba");
		}

		[Test]
		public void AssertsFailWhenTheyShould()
		{
			// IsTrue
			bool e = false;
			try
			{
				Assert.IsTrue(false, "");
			}
			catch (AssertionFailedException)
			{
				e = true;
			}
			Assert.IsFalse(!e, "");

			// IsFalse
			e = false;
			try
			{
				Assert.IsFalse(true, "");
			}
			catch (AssertionFailedException)
			{
				e = true;
			}
			Assert.IsTrue(e, "");

			// AreEqual
			e = false;
			try
			{
				Assert.AreEqual(1, 2, "");
			}
			catch (AssertionFailedException)
			{
				e = true;
			}
			Assert.IsTrue(e, "");

			// AreNotEqual
			e = false;
			try
			{
				Assert.AreNotEqual("foo", "foo", "");
			}
			catch (AssertionFailedException)
			{
				e = true;
			}
			Assert.IsTrue(e, "");

			// IsNotWithin - float
			e = false;
			try
			{
				Assert.IsNotWithin(1.0f, 1.1f, 0.15f, "");
			}
			catch (AssertionFailedException)
			{
				e = true;
			}
			Assert.IsTrue(e, "");

			// IsWithin - float
			e = false;
			try
			{
				Assert.IsWithin(0.0f, 100.0f, 1.1f, "");
			}
			catch (AssertionFailedException)
			{
				e = true;
			}
			Assert.IsTrue(e, "");

			// IsEmptyString
			e = false;
			try
			{
				Assert.IsEmptyString("empty string", "");
			}
			catch (AssertionFailedException)
			{
				e = true;
			}
			Assert.IsTrue(e, "");

			// IsNotEmptyString
			e = false;
			try
			{
				Assert.IsNotEmptyString("", "");
			}
			catch (AssertionFailedException)
			{
				e = true;
			}
			Assert.IsTrue(e, "");

			// ContainsString
			e = false;
			try
			{
				Assert.ContainsString("George W. Bush", "Smarts", "");
			}
			catch (AssertionFailedException)
			{
				e = true;
			}
			Assert.IsTrue(e, "");
		}
	}
}
