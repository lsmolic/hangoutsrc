/**  --------------------------------------------------------  *
 *   PlayerInventoryState.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 08/25/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Client.Gui
{
	public class NoticeDiaglogBox : IDisposable
	{

		private readonly Window mWindow;
		public NoticeDiaglogBox(string noticeMessage, string buttonText)
		{
			GuiFrame noticeFrame = new GuiFrame("NoticeDialogFrame", new FillParent());
			Label noticeLabel = new Label("NoticeDialogLabel", new FixedSize(180, 50), null, noticeMessage);
			Button okButton = new Button("OkButton", new FixedSize(69, 30), null, buttonText);
			okButton.AddOnPressedAction(delegate(){
				okButton.GetTopLevel().Close();
			});
			noticeFrame.AddChildWidget(noticeLabel, new FixedPosition(5, 5));
			noticeFrame.AddChildWidget(okButton, new FixedPosition(62, 60));
			mWindow = new Window("NoticeDiaglogBox", new FixedSize(200, 100), GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>(), noticeFrame);
		}

		public void Dispose()
		{
			mWindow.Close();
		}
	}
}
