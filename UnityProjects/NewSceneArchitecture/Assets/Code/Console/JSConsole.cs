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
	public class JSConsole : ILogReporter
	{
        JSDispatcher mJSDispatcher = new JSDispatcher();

		public JSConsole()
		{
            // Disable toggling for now.  Might want it again later.
            //IInputManager inputManager = GameFacade.Instance.RetrieveMediator<InputManagerMediator>();
            //if(inputManager == null)
            //{
            //    throw new Exception("Cannot construct a JSConsole without having an InputManagerMediator registered in the GameFacade");
            //}
            //inputManager.RegisterForButtonUp(KeyCode.BackQuote, delegate()
            //{
            //    mJSDispatcher.ToggleConsole(null);
            //});
		}

		public void Report(ILogMessage message)
		{
            switch (message.Level)
            {
                case LogLevel.Error:
                    mJSDispatcher.LogToConsole("error", message.Message, null);
                    break;
                case LogLevel.Info:
                    mJSDispatcher.LogToConsole("info", message.Message, null);
                    break;
                default:
                    mJSDispatcher.LogToConsole("debug", message.Message, null);
                    break;
            }
		}
	}
}
