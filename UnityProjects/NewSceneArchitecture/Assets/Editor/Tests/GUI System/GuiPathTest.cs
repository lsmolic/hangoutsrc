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
	public class GuiPathTest
	{
		[Test]
		public void SingleDepthSingleElementSelectTest()
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

			ITopLevel testWindow = new Window
			(
				"TestWindow",
				new FixedSize(new Vector2(132.0f, 232.0f)),
				manager,
				testFrame,
				null
			);

			// All of these paths should return the testFrame object
			GuiPath testPath1 = new GuiPath("/TestFrame");
			GuiPath testPath2 = new GuiPath("TestFrame");
			GuiPath testPath3 = new GuiPath("/*");
			GuiPath testPath4 = new GuiPath("*");

			IGuiElement test1 = testPath1.SelectSingleElement(testWindow);
			IGuiElement test2 = testPath2.SelectSingleElement(testWindow);
			IGuiElement test3 = testPath3.SelectSingleElement(testWindow);
			IGuiElement test4 = testPath4.SelectSingleElement(testWindow);

			Assert.AreEqual<IGuiElement>(testFrame, test1);
			Assert.AreEqual<IGuiElement>(testFrame, test2);
			Assert.AreEqual<IGuiElement>(testFrame, test3);
			Assert.AreEqual<IGuiElement>(testFrame, test4);
		}

		[Test]
		public void SingleDepthSingleElementSelectNegativeTest()
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

			ITopLevel testWindow = new Window
			(
				"TestWindow",
				new FixedSize(new Vector2(132.0f, 232.0f)),
				manager,
				testFrame,
				null
			);

			// All of these paths should return the testFrame object
			GuiPath testPath1 = new GuiPath("/foo");
			GuiPath testPath2 = new GuiPath("bar");

			IGuiElement test1 = testPath1.SelectSingleElement(testWindow);
			IGuiElement test2 = testPath2.SelectSingleElement(testWindow);

			Assert.AreNotEqual<IGuiElement>(testFrame, test1);
			Assert.AreNotEqual<IGuiElement>(testFrame, test2);
		}

		[Test]
		public void SingleDepthMultipleElementSelectTest()
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

			ITopLevel testWindow = new Window
			(
				"TestWindow",
				new FixedSize(new Vector2(132.0f, 232.0f)),
				manager,
				testFrame,
				testFrame,
				null
			);

			// All of these paths should return the testFrame object
			GuiPath testPath1 = new GuiPath("/TestFrame");
			GuiPath testPath2 = new GuiPath("TestFrame");
			GuiPath testPath3 = new GuiPath("/*");
			GuiPath testPath4 = new GuiPath("*");

			IGuiElement[] test1 = testPath1.SelectElements(testWindow);
			IGuiElement[] test2 = testPath2.SelectElements(testWindow);
			IGuiElement[] test3 = testPath3.SelectElements(testWindow);
			IGuiElement[] test4 = testPath4.SelectElements(testWindow);

			IGuiElement[] testFrames = new IGuiElement[2] { testFrame, testFrame };

			Assert.ArraysAreEqual(testFrames, test1);
			Assert.ArraysAreEqual(testFrames, test2);
			Assert.ArraysAreEqual(testFrames, test3);
			Assert.ArraysAreEqual(testFrames, test4);
		}

		[Test]
		public void MultipleDepthSingleElementSelectTest()
		{
			RuntimeGuiManager manager = new RuntimeGuiManager(false, new Logger());
			IDictionary<IWidget, IGuiPosition> widgets = new Dictionary<IWidget, IGuiPosition>();
			widgets.Add
			(
				new Button
				(
					"Button01",
					new FixedSize(new Vector2(10.0f, 10.0f)),
					null,
					null,
					"OK"
				),
				new FixedPosition(Vector2.zero)
			);
			widgets.Add
			(
				new Button
				(
					"Button02",
					new FixedSize(new Vector2(10.0f, 10.0f)),
					null,
					null,
					"OK"
				),
				new FixedPosition(Vector2.zero)
			);
			widgets.Add
			(
				new Button
				(
					"Button03",
					new FixedSize(new Vector2(10.0f, 10.0f)),
					null,
					null,
					"OK"
				),
				new FixedPosition(Vector2.zero)
			);

			// A window named 'TestWindow' with an empty main frame named 'TestFrame'
			IGuiFrame testFrame = new GuiFrame
			(
				"TestFrame",
				new FixedSize(new Vector2(32.0f, 32.0f)),
				widgets,
				null
			);

			ITopLevel testWindow = new Window
			(
				"TestWindow",
				new FixedSize(new Vector2(132.0f, 232.0f)),
				manager,
				testFrame,
				null,
				null
			);

			// All of these paths should return a button
			GuiPath testPath1 = new GuiPath("/TestFrame/Button01");
			GuiPath testPath2 = new GuiPath("TestFrame/Button01");
			GuiPath testPath3 = new GuiPath("/TestFrame/*");
			GuiPath testPath4 = new GuiPath("TestFrame/*");

			IGuiElement test1 = testPath1.SelectSingleElement(testWindow);
			IGuiElement test2 = testPath2.SelectSingleElement(testWindow);
			IGuiElement test3 = testPath3.SelectSingleElement(testWindow);
			IGuiElement test4 = testPath4.SelectSingleElement(testWindow);

			Assert.IsTrue(test1 is Button);
			Assert.IsTrue(test2 is Button);
			Assert.IsTrue(test3 is Button);
			Assert.IsTrue(test4 is Button);
		}


		[Test]
		public void MultipleDepthMultipleElementSelectTest()
		{
			RuntimeGuiManager manager = new RuntimeGuiManager(false, new Logger());
			IDictionary<IWidget, IGuiPosition> widgets = new Dictionary<IWidget, IGuiPosition>();
			widgets.Add
			(
				new Button
				(
					"Button01",
					new FixedSize(new Vector2(10.0f, 10.0f)),
					null,
					null,
					"OK"
				),
				new FixedPosition(Vector2.zero)
			);
			widgets.Add
			(
				new Button
				(
					"Button02",
					new FixedSize(new Vector2(10.0f, 10.0f)),
					null,
					null,
					"OK"
				),
				new FixedPosition(Vector2.zero)
			);
			widgets.Add
			(
				new Button
				(
					"Button03",
					new FixedSize(new Vector2(10.0f, 10.0f)),
					null,
					null,
					"OK"
				),
				new FixedPosition(Vector2.zero)
			);

			// A window named 'TestWindow' with an empty main frame named 'TestFrame'
			IGuiFrame testFrame = new GuiFrame
			(
				"TestFrame",
				new FixedSize(new Vector2(32.0f, 32.0f)),
				widgets,
				null
			);

			ITopLevel testWindow = new Window
			(
				"TestWindow",
				new FixedSize(new Vector2(132.0f, 232.0f)),
				manager,
				testFrame,
				null,
				null
			);

			// All of these paths should return all of the Buttons
			GuiPath testPath1 = new GuiPath("/TestFrame/*");
			GuiPath testPath2 = new GuiPath("TestFrame/*");

			IGuiElement[] test1 = testPath1.SelectElements(testWindow);
			IGuiElement[] test2 = testPath2.SelectElements(testWindow);

			Assert.AreEqual(3, test1.Length);
			Assert.AreEqual(3, test2.Length);

			foreach (IGuiElement testElement in test1)
			{
				Assert.IsTrue(testElement is Button);
			}
			foreach (IGuiElement testElement in test2)
			{
				Assert.IsTrue(testElement is Button);
			}
		}


		[Test]
		public void UnknownDepthElementSelectTest()
		{
			RuntimeGuiManager manager = new RuntimeGuiManager(false, new Logger());
			IDictionary<IWidget, IGuiPosition> widgets = new Dictionary<IWidget, IGuiPosition>();
			widgets.Add
			(
				new Button
				(
					"Button01",
					new FixedSize(new Vector2(10.0f, 10.0f)),
					null,
					null,
					"OK"
				),
				new FixedPosition(Vector2.zero)
			);
			widgets.Add
			(
				new Button
				(
					"Button02",
					new FixedSize(new Vector2(10.0f, 10.0f)),
					null,
					null,
					"OK"
				),
				new FixedPosition(Vector2.zero)
			);
			widgets.Add
			(
				new Button
				(
					"Button03",
					new FixedSize(new Vector2(10.0f, 10.0f)),
					null,
					null,
					"OK"
				),
				new FixedPosition(Vector2.zero)
			);

			// A window named 'TestWindow' with an empty main frame named 'TestFrame'
			IGuiFrame testFrame = new GuiFrame
			(
				"TestFrame",
				new FixedSize(new Vector2(32.0f, 32.0f)),
				widgets,
				null
			);

			ITopLevel testWindow = new Window
			(
				"TestWindow",
				new FixedSize(new Vector2(132.0f, 232.0f)),
				manager,
				testFrame,
				null,
				null
			);

			GuiPath testPath = new GuiPath("**/Button01");
			IGuiElement test = testPath.SelectSingleElement(testWindow);

			Assert.AreEqual("Button01", test.Name);
		}

		[Test]
		public void ReverseHeiarchyClimbMultipleQueryMultipleDepth()
		{
			RuntimeGuiManager manager = new RuntimeGuiManager(false, new Logger());

			IGuiFrame testFrame1 = new GuiFrame("TestFrame1", new FillParent());

			IGuiFrame testFrame2 = new GuiFrame("TestFrame2", new FillParent());
			testFrame1.AddChildWidget(testFrame2, new FixedPosition(0.0f, 0.0f));
			
			IGuiFrame testFrame3 = new GuiFrame("TestFrame3", new FillParent());
			testFrame2.AddChildWidget(testFrame3, new FixedPosition(0.0f, 0.0f));
			
			IGuiFrame testFrame4 = new GuiFrame("TestFrame4", new FillParent());
			testFrame3.AddChildWidget(testFrame4, new FixedPosition(0.0f, 0.0f));
			
			IGuiFrame testFrame5 = new GuiFrame("TestFrame5", new FillParent());
			testFrame4.AddChildWidget(testFrame5, new FixedPosition(0.0f, 0.0f));

			new Window("TestWindow", new FixedSize(new Vector2(128.0f, 128.0f)), manager, testFrame1);

			GuiPath testPath = new GuiPath("../../../*");

			Assert.AreEqual("TestFrame3", testPath.SelectSingleElement(testFrame5).Name);
			Assert.AreEqual("TestFrame2", testPath.SelectSingleElement(testFrame4).Name);
			Assert.AreEqual("TestFrame1", testPath.SelectSingleElement(testFrame3).Name);

			GuiPath testPath2 = new GuiPath("../../../../../TestFrame1/TestFrame2/TestFrame3/TestFrame4/TestFrame5");
			Assert.AreEqual(testFrame5, testPath2.SelectSingleElement<IGuiFrame>(testFrame5));
		}

	}
}
