/**  --------------------------------------------------------  *
 *   ProtocolUtilityTest.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/09/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

using Hangout.Client;
using Hangout.Shared.UnitTest;

namespace Hangout.Client.UnitTest
{
	[TestFixture]
	public class ProtocolUtilityTest
	{
		[Test]
		public void SplitProtocolVerification()
		{
			string testPath1 = @"resources://test/Try-some.Unexpected Characters/soemthingelse.file";
			string testPath2 = @"file://C:/Windows/System32/";
			string testPath3 = @"http://www.google.com";

			Assert.IsTrue(ProtocolUtility.SplitProtocol(testPath1).First == "resources");
			Assert.IsTrue(ProtocolUtility.SplitProtocol(testPath1).Second == "test/Try-some.Unexpected Characters/soemthingelse.file");

			Assert.IsTrue(ProtocolUtility.SplitProtocol(testPath2).First == "file");
			Assert.IsTrue(ProtocolUtility.SplitProtocol(testPath2).Second == "C:/Windows/System32/");

			Assert.IsTrue(ProtocolUtility.SplitProtocol(testPath3).First == "http");
			Assert.IsTrue(ProtocolUtility.SplitProtocol(testPath3).Second == "www.google.com");
		}

		[Test]
		public void SplitAndResolveProtocolVerification()
		{
			string testPath1 = @"resources://test/Try-some.Unexpected Characters/soemthingelse.file";
			string testPath2 = @"file://C:/Windows/System32/";
			string testPath3 = @"http://www.google.com";
			string testPath4 = @"assets://some/Asset.fbx";

			// Resolve should strip off the resources:// or assets://, but not file:// or http://
			Assert.IsFalse(ProtocolUtility.SplitAndResolve(testPath1).Second.StartsWith("resources"));
			Assert.IsTrue(ProtocolUtility.SplitAndResolve(testPath2).Second.StartsWith("file"));
			Assert.IsTrue(ProtocolUtility.SplitAndResolve(testPath3).Second.StartsWith("http"));
			Assert.IsFalse(ProtocolUtility.SplitAndResolve(testPath4).Second.StartsWith("assets"));
		}

		[Test]
		public void InEditorAssetsProtocolHandlerVerification()
		{
			// Unit tests are run in the editor (or visual studio which reports itself to be editor)
			string editorTestPath = ProtocolUtility.ResolvePath(@"assets://some/Asset.fbx");

			Assert.IsTrue(editorTestPath.StartsWith("file://" + Application.dataPath));
		}
	}

}
