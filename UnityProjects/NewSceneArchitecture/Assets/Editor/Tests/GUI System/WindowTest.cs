/**  --------------------------------------------------------  *
 *   GuiPathTest.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/21/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

using Hangout.Client.Gui;
using Hangout.Shared;

namespace Hangout.Shared.UnitTest
{
	[TestFixture]
	public class WindowTest
	{
		[Test]
		public void WindowsCanBeNamed()
		{
			RuntimeGuiManager manager = new RuntimeGuiManager(false, new Logger());

			// A window named 'TestWindow' with an empty main frame named 'TestFrame'
			ITopLevel testWindow = new Window
			(
				"TestWindow",
				new FixedSize(new Vector2(150.0f, 200.0f)),
				manager,
				null,
				null,
				null
			);

			Assert.AreEqual("TestWindow", testWindow.Name);
		}

		[Test]
		public void ChildFramesKeepName()
		{
			RuntimeGuiManager manager = new RuntimeGuiManager(false, new Logger());

			// A window named 'TestWindow' with an empty main frame named 'TestFrame'
			IGuiFrame testFrame = new GuiFrame
			(
				"TestFrame",
				new FixedSize(new Vector2(32.0f, 32.0f)),
				new Dictionary<IWidget, IGuiPosition>(),
				null
			);

			Window testWindow = new Window
			(
				"TestWindow",
				new FixedSize(new Vector2(150.0f, 200.0f)),
				manager,
				testFrame,
				null,
				null
			);

			Assert.AreEqual("TestFrame", testWindow.MainFrame.Key.Name);
		}
	}
}
