/**  --------------------------------------------------------  *
 *   AutoLayoutTest.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/03/2009
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
	public class AutoLayoutTest
	{

		[Test]
		public void HorizontalLayoutStartsAtOrigin()
		{
			Button testWidget1 = new Button("testButton1",
											 new FixedSize(new Vector2(24.0f, 48.0f)),
											 null,
											 "One");
			IDictionary<IWidget, IGuiPosition> widgets = new Dictionary<IWidget, IGuiPosition>();
			widgets[testWidget1] = new HorizontalAutoLayout();
			GuiFrame testFrame = new GuiFrame("testFrame",
											   new FixedSize(new Vector2(100.0f, 60.0f)),
											   widgets);

			testFrame.AutoLayoutUpdate(); // This is the part of the draw process that sets the autolayout positions

			foreach (IGuiElement element in testFrame.Children)
			{
				Assert.AreEqual<IGuiElement>(testWidget1, element, "The Button with HorizontalAutoLayout wasn't properly added to the GuiFrame.");
				Vector2 widgetPosition = widgets[testWidget1].GetPosition(testWidget1);
				Assert.AreEqual(0.0f, widgetPosition.x);
				Assert.AreEqual(0.0f, widgetPosition.y);
			}
		}

		[Test]
		public void HorizontalLayoutPlacesObjectsHorizontally()
		{
			Button testWidget1 = new Button("testButton1",
											 new FixedSize(new Vector2(48.0f, 24.0f)),
											 null,
											 "One");

			Button testWidget2 = new Button("testButton2",
											 new FixedSize(new Vector2(28.0f, 24.0f)),
											 null,
											 "Two");

			List<KeyValuePair<IWidget, IGuiPosition>> widgets = new List<KeyValuePair<IWidget, IGuiPosition>>();

			HorizontalAutoLayout layout = new HorizontalAutoLayout();
			widgets.Add(new KeyValuePair<IWidget, IGuiPosition>(testWidget1, layout));
			widgets.Add(new KeyValuePair<IWidget, IGuiPosition>(testWidget2, layout));

			GuiFrame testFrame = new GuiFrame("testFrame",
											   new FixedSize(new Vector2(100.0f, 60.0f)),
											   widgets);

			testFrame.AutoLayoutUpdate();

			foreach (IGuiElement element in testFrame.Children)
			{
				if (element == testWidget2)
				{
					Vector2 widgetPosition = layout.GetPosition(testWidget2);
					Assert.AreEqual(48.0f, widgetPosition.x);
					Assert.AreEqual(0.0f, widgetPosition.y);
				}
				else if (element == testWidget1)
				{
					Vector2 widgetPosition = layout.GetPosition(testWidget1);
					Assert.AreEqual(0.0f, widgetPosition.x);
					Assert.AreEqual(0.0f, widgetPosition.y);
				}
				else
				{
					Assert.Fail("There shouldn't be anything in this frame but testWidget1 and 2");
				}
			}
		}



		[Test]
		public void HorizontalLayoutLinewraps()
		{
			Button testWidget1 = new Button("testButton1",
											 new FixedSize(new Vector2(48.0f, 24.0f)),
											 null,
											 "One");

			Button testWidget2 = new Button("testButton2",
											 new FixedSize(new Vector2(28.0f, 24.0f)),
											 null,
											 "Two");

			Button testWidget3 = new Button("testButton3",
											 new FixedSize(new Vector2(64.0f, 28.0f)),
											 null,
											 "Three");

			List<KeyValuePair<IWidget, IGuiPosition>> widgets = new List<KeyValuePair<IWidget, IGuiPosition>>();

			HorizontalAutoLayout layout = new HorizontalAutoLayout();
			widgets.Add(new KeyValuePair<IWidget, IGuiPosition>(testWidget1, layout));
			widgets.Add(new KeyValuePair<IWidget, IGuiPosition>(testWidget2, layout));
			widgets.Add(new KeyValuePair<IWidget, IGuiPosition>(testWidget3, layout));

			GuiFrame testFrame = new GuiFrame("testFrame",
											   new FixedSize(new Vector2(100.0f, 60.0f)),
											   widgets);

			testFrame.AutoLayoutUpdate();

			foreach (IGuiElement element in testFrame.Children)
			{
				if (element == testWidget2)
				{
					Vector2 widgetPosition = layout.GetPosition(testWidget2);
					Assert.AreEqual(48.0f, widgetPosition.x);
					Assert.AreEqual(0.0f, widgetPosition.y);
				}
				else if (element == testWidget1)
				{
					Vector2 widgetPosition = layout.GetPosition(testWidget1);
					Assert.AreEqual(0.0f, widgetPosition.x);
					Assert.AreEqual(0.0f, widgetPosition.y);
				}
				else if (element == testWidget3)
				{
					Vector2 widgetPosition = layout.GetPosition(testWidget3);
					Assert.AreEqual(0.0f, widgetPosition.x);
					Assert.AreEqual(24.0f, widgetPosition.y);
				}
				else
				{
					Assert.Fail("There shouldn't be anything in this frame but testWidget1 and 2");
				}
			}
		}
	}
}
