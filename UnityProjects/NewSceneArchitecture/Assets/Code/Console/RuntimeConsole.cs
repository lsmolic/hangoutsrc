/**  --------------------------------------------------------  *
 *   RuntimeConsole.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/06/2009
 *	 
 *   --------------------------------------------------------  *
 */
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using Hangout.Client.Gui;
using Hangout.Shared;
using PureMVC.Interfaces;

namespace Hangout.Client
{
	public class RuntimeConsole : GuiController, ILogReporter, IMediator
	{
		private const string mGuiPath = "resources://GUI/Tools/DebugConsole.gui";
		private readonly ITopLevel mConsoleWindow;
		private readonly IGuiFrame mLogFrame;
		private readonly IGuiManager mManager;

		private readonly IDictionary<LogLevel, IGuiStyle> mLogLevelStyles = new Dictionary<LogLevel, IGuiStyle>();
		private readonly float mMainFrameWidth; // For word wrap
		private float mNextLogPosition = 0.0f;

		public RuntimeConsole(IGuiManager guiManager)
			: base(guiManager, mGuiPath)
		{
			if (guiManager == null)
			{
				throw new ArgumentNullException("guiManager");
			}

			mManager = guiManager;
			mConsoleWindow = this.MainGui;
			mConsoleWindow.Showing = false;

			IInputManager inputManager = GameFacade.Instance.RetrieveMediator<InputManagerMediator>();
			if(inputManager == null)
			{
				throw new Exception("Cannot construct a RuntimeConsole without having an InputManagerMediator registered in the GameFacade");
			}

			inputManager.RegisterForButtonUp(KeyCode.BackQuote, delegate()
			{
				mConsoleWindow.Showing = !mConsoleWindow.Showing;
			});
			
			mLogFrame = mConsoleWindow.SelectSingleElement<IGuiFrame>("MainFrame/DebugLogFrame");
			mMainFrameWidth = mLogFrame.Size.x;
			Button closeButton = mConsoleWindow.SelectSingleElement<Button>("HeaderFrame/CloseButton");
			if( closeButton == null )
			{
				throw new Exception("Cannot find the button expected to be at 'HeaderFrame/CloseButton'");
			}

			closeButton.AddOnPressedAction(delegate()
			{
				mConsoleWindow.Showing = false;
			});

			IGuiStyle defaultLabelStyle = new GuiStyle(mManager.GetDefaultStyle(typeof(Label)), "ConsoleLabel");
			defaultLabelStyle.WordWrap = true;
			//defaultLabelStyle.DefaultFont = (Font)Resources.Load("Fonts/CourierBold");

			IGuiStyle errorStyle = new GuiStyle(defaultLabelStyle, "ConsoleErrorLabel");

			defaultLabelStyle.Normal.TextColor = Color.white;
			errorStyle.Normal.TextColor = Color.Lerp(Color.black, Color.red, 0.35f);

			mLogLevelStyles.Add(LogLevel.Error, errorStyle);
			mLogLevelStyles.Add(LogLevel.Info, defaultLabelStyle);

			mCommandEntryBox = mConsoleWindow.SelectSingleElement<Textbox>("**/CommandEntryBox");

			inputManager.RegisterForButtonDown(KeyCode.Return, delegate()
			{
				if(this.MainGui.Showing)
				{
					List<string> lineParsed = new List<string>(mCommandEntryBox.Text.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries));
					Action<ICollection<string>> command;
					if( lineParsed.Count > 0 && mCommands.TryGetValue(lineParsed[0], out command))
					{
						lineParsed.RemoveAt(0);
						command(lineParsed);
					}
				}
			});
		}

		private readonly Dictionary<string, Action<ICollection<string>>> mCommands = new Dictionary<string, Action<ICollection<string>>>();
		public void AddCommand(string commandName, Action<ICollection<string>> command)
		{
			if(String.IsNullOrEmpty(commandName))
			{
				throw new ArgumentNullException("commandName");
			}
			if( command == null )
			{
				throw new ArgumentNullException("command");
			}
			if( mCommands.ContainsKey(commandName) )
			{
				throw new ArgumentException("There is already a command registered with the name " + commandName + ": " + mCommands[commandName]);
			}
			mCommands.Add(commandName, command);
		}

		private Textbox mCommandEntryBox;

		public void Report(ILogMessage message)
		{
			IGuiStyle logStyle = mLogLevelStyles[message.Level];
			Textbox newLog = new Textbox
			(
				"LoggedMessage",
				new ExpandText(mMainFrameWidth), 
				logStyle, 
				message.Message,
				false,
				false
			);

			mLogFrame.AddChildWidget(newLog, new FixedPosition(0.0f, mNextLogPosition));
			mNextLogPosition += newLog.Size.y + logStyle.ExternalMargins.GetSizeDifference().y;

			/*if( message.Level == LogLevel.Error )
			{
				mConsoleWindow.Showing = true;
                ((Window)mConsoleWindow).InFront = true;
			}*/
		}

		#region IMediator Members

		public string MediatorName
		{
			get { return this.GetType().Name; }
		}

		public IList<string> ListNotificationInterests()
		{
			return new List<string>();
		}

		public void HandleNotification(INotification notification)
		{
		}

		public void OnRegister()
		{
		}

		public void OnRemove()
		{
		}

		#endregion
	}
}
