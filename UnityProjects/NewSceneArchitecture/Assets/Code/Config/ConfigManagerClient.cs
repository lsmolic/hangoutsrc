/*
 * ConfigManagerClient - Client interface to the ConfigManager.
 * 
 * Responsible for:
 * - Locating and reading config and group files
 * - Parsing config and group JSON data to produce a rule set (see ConfigManagerBase)
 * - Retrieving global or per-user config values from the rule set
 * - Casting config values to the type specified by the caller
 * 
 * See also:
 * - ConfigManagerBase
 * - ConfigManagerStateServer
 * - ConfigManagerASP
 * 
 */
using Hangout.Shared;
using System.Collections.Generic;
using System.Web;
using UnityEngine;

namespace Hangout.Client
{
    public class ConfigManagerClient : Hangout.Shared.ConfigManagerBase, PureMVC.Interfaces.IProxy
    {
        public ConfigManagerClient()
        { }

        public void InitAndStartup()
        {
            if (Application.isEditor)  // Running on a dev desktop in the editor
            {
                if (!InitFromLocalDisk())
                {
                    throw new System.Exception("Couldn't find a config.json in your search paths.  You need this file to run in development.  Ask Ian for help!");
                }
                GameFacade.Instance.SendNotification(GameFacade.STARTUP_COMMAND);
            }
            else if (Application.absoluteURL.Contains("file://"))  // Running on a dev desktop in the web build
            {
                GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(InitForLocalWebBuild());
            }
            else  // Running in production
            {
                GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.AddReporter(new JSConsole());
                GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("ConfigManagerClient.  Sending js call", LogLevel.Info);
                // Ask javascript for our config info
                JSDispatcher jsd = new JSDispatcher();
                jsd.RequestConfigObject(delegate(string configJSON)
                {
                    configJSON = configJSON.Replace("&quot;", "\"");

                    GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("ConfigManagerClient.  Got response from javascript: " + configJSON, LogLevel.Info);

                    try
                    {
                        // Client should never need knowledge of groups
                        InitFromJSON(configJSON, "{}");
                        GameFacade.Instance.SendNotification(GameFacade.STARTUP_COMMAND);
                    }
                    catch (System.Exception ex)
                    {
                        GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("Unhandled exception: " + ex.ToString(), LogLevel.Error);
                    }
                    finally
                    {
                        GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Flush();
                    }
                });
            }
        }

        private IEnumerator<IYieldInstruction> InitForLocalWebBuild()
        {
            string configJSON = "{}";
            WWW dub = null;

            foreach (string path in GetSearchPaths())
            {
                dub = new WWW(HttpUtility.LocalPathToFileUrlPrefix(path) + "config.json");
                yield return new YieldForWWW(dub);
                if (string.IsNullOrEmpty(dub.error))
                {
                    GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("Successfully read config.json at " + dub.url, LogLevel.Info);
                    configJSON = dub.data;
                    break;
                }
                else
                {
                    GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("Did not find config.json at " + dub.url, LogLevel.Info);
                }
            }

            GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("Running in dev web build, here is my config JSON: " + configJSON, LogLevel.Info);
            InitFromJSON(configJSON, "{}");
            GameFacade.Instance.SendNotification(GameFacade.STARTUP_COMMAND);
        }

        // ---- Begin config property getters ----

        public object GetObject(string cfgKey, object defaultValue)
        {
            return GetObject(cfgKey, null, null, defaultValue);
        }

        public T GetObject<T>(string cfgKey, T defaultValue)
        {
            return GetObject<T>(cfgKey, null, null, defaultValue);
        }

        public List<object> GetList(string cfgKey, List<object> defaultValue)
        {
            return GetList(cfgKey, null, null, defaultValue);
        }

        public List<T> GetList<T>(string cfgKey, List<T> defaultValue)
        {
            return GetList<T>(cfgKey, null, null, defaultValue);
        }

        public string GetString(string cfgKey, string defaultValue)
        {
            return GetString(cfgKey, null, null, defaultValue);
        }

        public bool GetBool(string cfgKey, bool defaultValue)
        {
            return GetBool(cfgKey, null, null, defaultValue);
        }

        public int GetInt(string cfgKey, int defaultValue)
        {
            return GetInt(cfgKey, null, null, defaultValue);            
        }

        public long GetLong(string cfgKey, long defaultValue)
        {
            return GetLong(cfgKey, null, null, defaultValue);
        }

        public float GetFloat(string cfgKey, float defaultValue)
        {
            return GetFloat(cfgKey, null, null, defaultValue);
        }

        public double GetDouble(string cfgKey, double defaultValue)
        {
            return GetDouble(cfgKey, null, null, defaultValue);
        }


        // ---- Begin IProxy methods ----

        public string ProxyName
        {
            get { return this.GetType().Name; ; }
        }

        public object Data
        {
            get { return mRuleSet; }
            set { mRuleSet = (Hangout.Shared.ConfigRuleSet)value; }
        }

        public void OnRegister()
        { }

        public void OnRemove()
        { }
    }
}
