/**  --------------------------------------------------------  *
 *   ChatWindow.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 6/12/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;

using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client.Gui
{	
	public class ChatWindow : IDisposable
	{
		private Textbox mChatEntryBox = null;
        
        private IGuiFrame mChatFrame = null;

		private IDictionary<string, Hangout.Shared.Action> mSlashCommands = new Dictionary<string, Hangout.Shared.Action>();

        private JSDispatcher mJSDispatcher;

		private void SetupSlashCommands()
		{
            mJSDispatcher = new JSDispatcher();

            mSlashCommands.Add("/jsc", delegate()
            {
                mJSDispatcher.ToggleJSConsole();
            });
            
            // Only do this in dev
			ConnectionProxy connectionProxy = GameFacade.Instance.RetrieveProxy<ConnectionProxy>();
			if (connectionProxy.StageName != "DEV") return;
				
			mSlashCommands.Add("/newRoom", delegate()
			{
				GameFacade.Instance.SendNotification(GameFacade.ENTER_ROOM_CREATOR);
			});

			mSlashCommands.Add("/fashionGame", delegate()
			{
				GameFacade.Instance.SendNotification(GameFacade.ENTER_FASHION_MINIGAME);
			});
			mSlashCommands.Add("/roomAPI", delegate()
			{
                GameFacade.Instance.SendNotification(GameFacade.SHOW_SERVER_ROOM_API);
			});

            mSlashCommands.Add("/roomPicker", delegate()
            {
                GameFacade.Instance.SendNotification(GameFacade.SHOW_ROOM_PICKER_GUI, MessageSubType.ClientOwnedRooms);
            });
            
			mSlashCommands.Add("/openInventory", delegate()
			{
				GameFacade.Instance.RetrieveProxy<InventoryProxy>().OpenPlayerInventory();
			});
			mSlashCommands.Add("/openShop", delegate()
			{
				GameFacade.Instance.SendNotification(GameFacade.SHOP_BUTTON_CLICKED);
			});

            mSlashCommands.Add("/showFriends", delegate()
            {
                GameFacade.Instance.SendNotification(GameFacade.SHOW_FRIENDS);
            });
		}

		public void Dispose()
		{
			mInputReceiptReturn.Exit(); 
			mInputReceiptEnter.Exit();
        }

		public bool Showing
		{
			get { return mChatFrame.Showing; }
            set { mChatFrame.Showing = value; }
		}

		private IReceipt mInputReceiptReturn;
		private IReceipt mInputReceiptEnter;

		public ChatWindow(IInputManager inputManager, IGuiFrame chatFrame) 
		{			
			if( inputManager == null )
			{
				throw new ArgumentNullException("inputManager");
			}

			SetupSlashCommands();

            mChatFrame = chatFrame;

			mChatEntryBox = mChatFrame.SelectSingleElement<Textbox>("ChatEntryTextbox");

            //mChatLogFrame = (mChatFrame.SelectSingleElement<TabButton>("BottomLeftTabView/ButtonsFrame/ChatLogTab")).Frame;
			//mLocalChatPrototype = mChatLogFrame.SelectSingleElement<Textbox>("**/MyMessages");
			
			//mChatLogFrame.RemoveChildWidget(mLocalChatPrototype);
			
			Hangout.Shared.Action chatSend = delegate()
			{
				// Get chat from text entry box, and clear it
				String chatText = mChatEntryBox.Text;
				mChatEntryBox.Text = "";

				Hangout.Shared.Action slashCommand;
				if (mSlashCommands.TryGetValue(chatText, out slashCommand))
				{
					slashCommand();
				}
				else
				{
					// Filter out empty string
					if (chatText != "")
					{
						// Dispatch chat event.  This will get picked up by SendChatCommand
						object[] args = { chatText };
						GameFacade.Instance.SendNotification(GameFacade.SEND_CHAT, args);
					}
                }
			};
			
			mInputReceiptReturn = inputManager.RegisterForButtonDown(KeyCode.Return, chatSend);
			// If numlock is down, some laptops send this event instead
			mInputReceiptEnter = inputManager.RegisterForButtonDown(KeyCode.KeypadEnter, chatSend);
		}

		public void AddChatText(string chatText)
		{/*
			Textbox newChatLog = new Textbox(mLocalChatPrototype);
			newChatLog.Text = chatText;
			mChatLogFrame.AddChildWidget(newChatLog, new HorizontalAutoLayout());
		
			if( mChatLogFrame is ScrollFrame )
			{
				((ScrollFrame)mChatLogFrame).VerticalScrollbar.ThumbPosition = 1.0f;
			}*/
		}
	}

}
