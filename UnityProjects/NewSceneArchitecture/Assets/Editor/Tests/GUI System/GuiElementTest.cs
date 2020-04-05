/**  --------------------------------------------------------  *
 *   GuiElementTest.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/11/2009
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
	public class GuiElementTest
	{
		[Test]
		public void GetTopLevelVerification()
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
				null
			);

			Assert.AreEqual(testWindow, testFrame.GetTopLevel());

			foreach (KeyValuePair<IWidget, IGuiPosition> widget in widgets)
			{
				Assert.AreEqual(testWindow, widget.Key.GetTopLevel());
			}
		}

		[Test]
		public void GetScreenPositionVerification()
		{
			RuntimeGuiManager manager = new RuntimeGuiManager(false, new Logger());
			IDictionary<IWidget, IGuiPosition> widgets = new Dictionary<IWidget, IGuiPosition>();
			Button testWidget = new Button
			(
				"Button01",
				new FixedSize(new Vector2(10.0f, 10.0f)),
				null,
				null,
				"OK"
			);

			widgets.Add( testWidget, new FixedPosition(new Vector2(10.0f, 10.0f)) );

			IGuiFrame testFrame = new GuiFrame
			(
				"TestFrame",
				new FillParent(),
				widgets,
				null
			);

			ITopLevel testWindow = new Window
			(
				"TestWindow",
				new FixedSize(new Vector2(400.0f, 400.0f)),
				manager,
				new KeyValuePair<IGuiFrame, IGuiPosition>
				(
					testFrame,
					new FixedPosition(10.0f, 10.0f)
				),
				null
			);

			manager.SetTopLevelPosition(testWindow, new FixedPosition(10.0f, 10.0f));

			UnityAssert.AreClose(new Vector2(20.0f, 20.0f), testFrame.GetScreenPosition(), 0.0001f);
			UnityAssert.AreClose(new Vector2(30.0f, 30.0f), testWidget.GetScreenPosition(), 0.0001f);
		}


		[Test]
		public void GetWidgetPositionVerification()
		{
			RuntimeGuiManager manager = new RuntimeGuiManager(false, new Logger());
			IDictionary<IWidget, IGuiPosition> widgets = new Dictionary<IWidget, IGuiPosition>();
			Button testWidget = new Button
			(
				"Button01",
				new FixedSize(new Vector2(10.0f, 10.0f)),
				null,
				null,
				"OK"
			);

			widgets.Add(testWidget, new FixedPosition(new Vector2(10.0f, 10.0f)));

			IGuiFrame testFrame = new GuiFrame
			(
				"TestFrame",
				new FillParent(),
				widgets,
				null
			);

			ITopLevel testWindow = new Window
			(
				"TestWindow",
				new FixedSize(new Vector2(400.0f, 400.0f)),
				manager,
				new KeyValuePair<IGuiFrame, IGuiPosition>
				(
					testFrame,
					new FixedPosition(10.0f, 10.0f)
				),
				null
			);

			manager.SetTopLevelPosition(testWindow, new FixedPosition(10.0f, 10.0f));
			
			UnityAssert.AreClose(Vector2.zero, testFrame.GetWidgetPosition(new Vector2(20.0f, Screen.height - 20.0f)), 0.0001f);
			UnityAssert.AreClose(Vector2.zero, testWidget.GetWidgetPosition(new Vector2(30.0f, Screen.height - 30.0f)), 0.0001f);

			UnityAssert.AreClose(new Vector2(0.0f, 5.0f), testFrame.GetWidgetPosition(new Vector2(20.0f, Screen.height - 25.0f)), 0.0001f);
			UnityAssert.AreClose(new Vector2(5.0f, 0.0f), testWidget.GetWidgetPosition(new Vector2(35.0f, Screen.height - 30.0f)), 0.0001f);
		}

	}
}
