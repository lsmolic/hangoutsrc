/**  --------------------------------------------------------  *
 *   RuntimeGuiManagerTest.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/20/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Text;
using System.Xml;

using Hangout.Client.Gui;
using Hangout.Shared;

namespace Hangout.Shared.UnitTest
{
	[TestFixture]
	public class RuntimeGuiManagerTest
	{
		private XmlDocument mDefaultStylesDoc = new XmlDocument();
		private RuntimeGuiManager mManager = new RuntimeGuiManager(false, new Logger());

		public RuntimeGuiManagerTest()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<GUI>");

			sb.AppendLine("<Style name=\"DefaultEditorBaseStyle\" defaultFor=\"GuiElement TopLevel\">");
			sb.AppendLine("<StyleState name=\"NormalState\"><TextColor color=\"0x000000\" />");
			sb.AppendLine("</StyleState>");
			sb.AppendLine("<StyleState name=\"HoverState\">");
			sb.AppendLine("<TextColor color=\"0x552222\" />");
			sb.AppendLine("</StyleState>");
			sb.AppendLine("<StyleState name=\"DisabledState\">");
			sb.AppendLine("<TextColor color=\"0x303030\" />");
			sb.AppendLine("</StyleState>");
			sb.AppendLine("<Margin name=\"InternalMargins\" value=\"2 2 2 2\" />");
			sb.AppendLine("<Margin name=\"ExternalMargins\" value=\"2 2 2 2\" />");
			sb.AppendLine("<Margin name=\"NinePartScale\" value=\"0 0 0 0\" />");
			sb.AppendLine("<Anchor name=\"DefaultTextAnchor\" value=\"Left Bottom\" />");
			sb.AppendLine("<WordWrap value=\"false\" />");
			sb.AppendLine("<ClipText value=\"false\" />");
			sb.AppendLine("</Style>");

			sb.AppendLine("<Style name=\"DefaultEditorButtonStyle\" parent=\"DefaultEditorBaseStyle\" defaultFor=\"Button\">");
			sb.AppendLine("<StyleState name=\"NormalState\"><TextColor color=\"0x000000\" />");
			sb.AppendLine("</StyleState>");
			sb.AppendLine("<StyleState name=\"HoverState\">");
			sb.AppendLine("<TextColor color=\"0x552222\" />");
			sb.AppendLine("</StyleState>");
			sb.AppendLine("<StyleState name=\"DisabledState\">");
			sb.AppendLine("<TextColor color=\"0x303030\" />");
			sb.AppendLine("</StyleState>");
			sb.AppendLine("<Margin name=\"NinePartScale\" value=\"8 8 8 8\" />");
			sb.AppendLine("<ClipText value=\"true\" />");
			sb.AppendLine("</Style>");

			sb.AppendLine("<Style name=\"DefaultEditorTextboxStyle\" parent=\"DefaultEditorBaseStyle\" defaultFor=\"Textbox\">");
			sb.AppendLine("<StyleState name=\"NormalState\"><TextColor color=\"0x000000\" />");
			sb.AppendLine("</StyleState>");
			sb.AppendLine("<StyleState name=\"HoverState\">");
			sb.AppendLine("<TextColor color=\"0x552222\" />");
			sb.AppendLine("</StyleState>");
			sb.AppendLine("<StyleState name=\"DisabledState\">");
			sb.AppendLine("<TextColor color=\"0x303030\" />");
			sb.AppendLine("</StyleState>");
			sb.AppendLine("<Margin name=\"NinePartScale\" value=\"8 8 8 8\" />");
			sb.AppendLine("<ClipText value=\"true\" />");
			sb.AppendLine("</Style>");

			sb.AppendLine("<Style name=\"ButtonStyle\">");
			sb.AppendLine("<StyleState name=\"NormalState\"><TextColor color=\"0x000000\" />");
			sb.AppendLine("</StyleState>");
			sb.AppendLine("<StyleState name=\"HoverState\">");
			sb.AppendLine("<TextColor color=\"0x552222\" />");
			sb.AppendLine("</StyleState>");
			sb.AppendLine("<StyleState name=\"DisabledState\">");
			sb.AppendLine("<TextColor color=\"0x303030\" />");
			sb.AppendLine("</StyleState>");
			sb.AppendLine("<Margin name=\"InternalMargins\" value=\"2 2 2 2\" />");
			sb.AppendLine("<Margin name=\"ExternalMargins\" value=\"2 2 2 2\" />");
			sb.AppendLine("<Margin name=\"NinePartScale\" value=\"0 0 0 0\" />");
			sb.AppendLine("<ClipText value=\"true\" />");
			sb.AppendLine("</Style>");

			sb.AppendLine("</GUI>");

			mDefaultStylesDoc.LoadXml(sb.ToString());
			mManager.LoadDefaultStylesFromXml(mDefaultStylesDoc);
		}

		[Test]
		public void TopLevelsRegisterWithManager()
		{
			ITopLevel newTopLevel = new Window
			(
				"TestWindow",
				new FixedSize(new Vector2(150.0f, 200.0f)),
				mManager
			);
			Assert.IsTrue(newTopLevel != null);
			Assert.AreEqual(mManager.ActiveTopLevelCount, 1);
		}

		[Test]
		public void DefaultsCascadeBasedOnSubtypes()
		{
			IGuiStyle guiElementStyle = mManager.GetDefaultStyle(typeof(GuiElement));
			IGuiStyle labelStyle = mManager.GetDefaultStyle(typeof(Label));

			IGuiStyle buttonStyle = mManager.GetDefaultStyle(typeof(Button));
			IGuiStyle toggleButtonStyle = mManager.GetDefaultStyle(typeof(ToggleButton));

			Assert.IsNotNull(guiElementStyle);
			Assert.IsNotNull(labelStyle);
			Assert.IsNotNull(buttonStyle);
			Assert.IsNotNull(toggleButtonStyle);

			Assert.AreEqual(guiElementStyle.Name, labelStyle.Name);
			Assert.AreEqual(buttonStyle.Name, toggleButtonStyle.Name);
			Assert.AreNotEqual(guiElementStyle.Name, buttonStyle.Name);
		}
	}
}
