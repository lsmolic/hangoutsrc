/**  --------------------------------------------------------  *
 *   SvnGui.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/04/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public class SvnGui : EditorGuiController 
	{
		private static readonly string mSvnGuiXml = "file:/" + Application.dataPath + "/Editor/GUI/SVN/SVN GUI.xml";
		private ITopLevel mGuiRoot = null;
		
		private static SvnGui mTheOneGui = null;
		
		[MenuItem("Window/SVN %0")]
		public static void OpenSvnGui()
		{
			mTheOneGui = new SvnGui(new EditorGuiManager());
			SuppressUnusedVariableWarning( mTheOneGui );
		}
		
		private static void SuppressUnusedVariableWarning(object o) {}
		
		public SvnGui(IGuiManager manager) 
			: base(manager, mSvnGuiXml)
		{
			List<IGuiFrame> toolContextFrames = new List<IGuiFrame>(); 
			foreach(IGuiElement element in this.AllElements) 
			{
				if( element is ITopLevel ) 
				{
					mGuiRoot = (ITopLevel)element;
				}
				else if( element is IGuiFrame ) 
				{
					toolContextFrames.Add((IGuiFrame)element);
				}
				else 
				{
					Debug.LogWarning("Ignoring unexpected GuiElement (" + element.Name + ") loaded from " + mSvnGuiXml);
				}
			}
			
			SuppressUnusedVariableWarning( mGuiRoot );
		}
	}
}
