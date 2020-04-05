/**  --------------------------------------------------------  *
 *   DockableEditorTab.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/04/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui {
	public class DockableEditorTab : Window {
		public DockableEditorTab( string name,
					   			  IGuiSize size,
					   			  IGuiManager manager,
					   			  KeyValuePair<IGuiFrame, IGuiPosition> mainFrame, 
					   			  IGuiStyle style ) 
		: base(name, size, manager, mainFrame, new KeyValuePair<IGuiFrame, IGuiPosition>(null, null), style) { }
		
		
		public override void Draw(IGuiContainer container, Vector2 position) {
			this.DrawContents(0);
		}
	}
}

