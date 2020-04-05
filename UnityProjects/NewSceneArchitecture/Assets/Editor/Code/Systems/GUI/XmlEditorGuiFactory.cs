/**  --------------------------------------------------------  *
 *   XmlEditorGuiFactory.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/04/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;

using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public class XmlEditorGuiFactory : XmlGuiFactory
	{
		public XmlEditorGuiFactory(XmlDocument xml, EditorGuiManager manager) 
			: base(xml, manager)
		{
		}

		public XmlEditorGuiFactory(string path, EditorGuiManager manager) 
			: base(path, manager) 
		{
		}

		protected override Window BuildWindow(XmlNode windowNode)
		{
			string name = BuildName(windowNode);
			IGuiSize size = BuildSize(windowNode);

			IGuiFrame mainFrame = null;
			IGuiPosition mainFramePosition = null;
			XmlNode mainFrameNode = windowNode.SelectSingleNode("MainFrame");
			if (mainFrameNode != null)
			{
				mainFramePosition = BuildPosition(mainFrameNode);
				mainFrame = BuildFrame(mainFrameNode);
				if (mainFrame == null)
				{
					throw new GuiConstructionException("Found MainFrame node in Window (" + name + "), but was unable to construct it.");
				}
			}

			IGuiStyle style = GetStyle(windowNode, typeof(Window));

			return new DockableEditorTab
			(
				name,
				size,
				mManager,
				new KeyValuePair<IGuiFrame, IGuiPosition>
				(
					mainFrame,
					mainFramePosition
				),
				style
			);
		}
	}
}
