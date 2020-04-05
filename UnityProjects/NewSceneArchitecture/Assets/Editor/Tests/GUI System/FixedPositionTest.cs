/**  --------------------------------------------------------  *
 *   FixedPositionTest.cs  
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
using Hangout.Shared;

namespace Hangout.Shared.UnitTest
{
	[TestFixture]
	public class FixedPositionTest
	{
		[Test]
		public void FixedPositionConstructorVerification()
		{
			Vector2 relativePos = new Vector2(-3.0f, 10.0f);
			FixedPosition testPosition = new FixedPosition
			(
				relativePos,
				GuiAnchor.CenterCenter,
				GuiAnchor.BottomRight
			);

			Assert.AreEqual(testPosition.LocalAnchor, GuiAnchor.CenterCenter);
			Assert.AreEqual(testPosition.ParentAnchor, GuiAnchor.BottomRight);
			Assert.AreEqual(testPosition.RelativePosition, relativePos);
		}

		[Test]
		public void FixedPositionsOffsetOverAnchorsCorrectlyCenterCenter()
		{
			Button testButton = new Button
			(
				"Test Button",
				new FixedSize(32.0f, 16.0f),
				null,
				"TEST"
			);

			FixedPosition buttonPosition = new FixedPosition
			(
				new Vector2(0.0f, 0.0f),
				GuiAnchor.BottomRight,
				GuiAnchor.CenterCenter
			);

			IDictionary<IWidget, IGuiPosition> widgets = new Dictionary<IWidget, IGuiPosition>();
			widgets.Add(testButton, buttonPosition);

			new GuiFrame
			(
				"Test Frame",
				new FixedSize(256.0f, 384.0f),
				widgets,
				null
			);

			Vector2 calculatedPosition = buttonPosition.GetPosition(testButton);
			Assert.IsWithin(96.0f, calculatedPosition.x, 0.001f);
			Assert.IsWithin(176.0f, calculatedPosition.y, 0.001f);
		}


		[Test]
		public void FixedPositionsOffsetOverAnchorsCorrectlyLeftBottom()
		{
			Textbox testTextBox = new Textbox
			(
				"Test Box",
				new FixedSize(184.0f, 48.0f),
				null,
				"TEST",
				true,
				true
			);

			FixedPosition textBoxPosition = new FixedPosition
			(
				new Vector2(8.0f, 8.0f),
				GuiAnchor.BottomLeft,
				GuiAnchor.BottomLeft
			);

			IDictionary<IWidget, IGuiPosition> widgets = new Dictionary<IWidget, IGuiPosition>();
			widgets.Add(testTextBox, textBoxPosition);

			GuiFrame mainFrame = new GuiFrame
			(
				"Test Frame",
				new FillParent(),
				widgets,
				null
			);

			new Window
			(
				"Test Window",
				new FixedSize(229.0f, 172.0f),
				new RuntimeGuiManager(false, new Logger()),
				mainFrame
			);

			Vector2 calculatedPosition = textBoxPosition.GetPosition(testTextBox);
			Assert.IsWithin(8.0f, calculatedPosition.x, 0.001f);
			Assert.IsWithin(132.0f, calculatedPosition.y, 0.001f);
		}
	}
}
