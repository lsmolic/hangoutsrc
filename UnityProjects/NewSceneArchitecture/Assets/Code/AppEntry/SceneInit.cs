/**  --------------------------------------------------------  *
 *   SceneInit.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/08/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

using Hangout.Shared;
using Hangout.Client.Gui;
using Hangout.Client;
using System.IO;
using PureMVC.Interfaces;

public class SceneInit : AppEntry
{
	private GameFacade mGameFacade = null;
	private RuntimeGuiManager mGuiManager = null;
	private ILogger mLogger = new Logger();

	private void OnGUI()
	{
		try
		{
			mGuiManager.Draw();
		}
		catch (System.Exception ex)
		{
			// Catch all here is to avoid browser crashes on exit if something becomes unstable.
			mLogger.Log(ex.ToString(), LogLevel.Error);
		}
	}

	protected override void Awake()
	{
		try
		{
			base.Awake();

			// Application settings go here
            Application.runInBackground = true;
            Application.targetFrameRate = 60;
			
			// Set up required components, and just the bare minimum of MVC actors.  We don't have config data yet, so 
			// anything that requires real data off the network should be registered in StartupCommand or later
			mGuiManager = new RuntimeGuiManager(mLogger);
			mLogger.AddReporter(new DebugLogReporter());
			mGameFacade = GameFacade.Instance; // Nulls out the reference on application exit for editor integration

			// Register Required Mediators
			GameFacade.Instance.RegisterMediator(new LoggerMediator(mLogger));
			GameFacade.Instance.RegisterMediator(new InputManagerMediator());
			GameFacade.Instance.RegisterMediator(new SchedulerMediator(this.Scheduler));
			GameFacade.Instance.RegisterMediator((IMediator)mGuiManager);

			RuntimeConsole runtimeConsole = new RuntimeConsole(mGuiManager);
			runtimeConsole.AddCommand("showTasks", ((Scheduler)this.Scheduler).ShowTasks);
			((Scheduler)this.Scheduler).Logger = mLogger;
			GameFacade.Instance.RegisterMediator(runtimeConsole);
			mLogger.AddReporter(runtimeConsole);

			mLogger.Log("SceneInit.Awake", LogLevel.Info);

			ConnectionProxy connectionProxy = new ConnectionProxy();
			GameFacade.Instance.RegisterProxy(connectionProxy);

            // Get configuration data before doing anything else.  This sends STARTUP_COMMAND when done getting the config data
            ConfigManagerClient configManager = new ConfigManagerClient();
            GameFacade.Instance.RegisterProxy(configManager);
            configManager.InitAndStartup();
        }
		catch (System.Exception ex)
		{
			// Catch all here is to avoid browser crashes on exit if something becomes unstable.
			mLogger.Log(ex.ToString(), LogLevel.Error);
		}
		finally
		{
			mLogger.Flush();
		}
	}

	private void OnApplicationQuit()
	{
		try
		{
			mGameFacade.Dispose();
			GameFacade.Instance.RemoveMediator(typeof(ClientMessageProcessor).Name);
		}
		catch (System.Exception ex)
		{
			// Catch all here is to avoid browser crashes on exit if something becomes unstable.
			mLogger.Log(ex.ToString(), LogLevel.Error);
		}
		finally
		{
			mLogger.Flush();
		}
	}

    public void ReceiveJavascriptCallback(string param)
    {
        mLogger.Log("We got a message from javascript, param=" + param);
    }
}
