/**  --------------------------------------------------------  *
 *   TabButton.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/22/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui {
	
	/// Class for the buttons at the top of a TabView.
	public class TabButton : ToggleButton {
		private readonly IGuiFrame mFrame;

		public IGuiFrame Frame {
			get { return mFrame; }
		}
		
		public void RegisterWithTabView(TabView tabView) {
			if( mFrame != null ) {
				mFrame.Parent = tabView;
				
				this.AddOnActivatedAction(delegate() {
					tabView.ActivateTab(this);
				});
				
				this.AddOnDeactivatedAction(delegate() {
					tabView.ClearContextFrame();
				});
			}
		}
		
		public TabButton(string name,
						 IGuiSize size,
						 IGuiStyle activatedStyle, 
						 IGuiStyle deactivatedStyle,
						 Texture image,
						 IGuiFrame frame)
		: base(name, size, activatedStyle, deactivatedStyle, image) {
			mFrame = frame;
		}
		
		public override void Draw(IGuiContainer parent, Vector2 size) {
			base.Draw(parent, size);
		}
	}
}