/**  --------------------------------------------------------  *
 *   GuiAnchorTest.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/15/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

using Hangout.Client.Gui;

namespace Hangout.Shared.UnitTest
{
	[TestFixture]
	public class GuiAnchorTest
	{
		[Test]
		public void GuiAnchorConstructorVerification()
		{
			GuiAnchor testAnchor = new GuiAnchor(GuiAnchor.X.Center, GuiAnchor.Y.Bottom);

			Assert.AreEqual((int)GuiAnchor.X.Center, (int)testAnchor.Horizontal);
			Assert.AreEqual((int)GuiAnchor.Y.Bottom, (int)testAnchor.Vertical);
		}
	}
}
