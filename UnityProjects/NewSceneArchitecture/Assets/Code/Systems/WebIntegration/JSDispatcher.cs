/*

JSDispatcher.cs

A protected C# interface to our Javascript methods for the web build.

Wraps Application.ExternalCall.  In order to avoid problems with method name 
synchronization and other pesky hard-coding issues, C# code should interface with 
Javascript through JSDispatcher instead of calling Application.ExternalCall.  There 
are still hard-coding issues but we can at least contain them here.

New external Javascript methods should add a corresponding public method on JSDispatcher.  
See JSDispatcher.RequestConfigValue for an example call.

Example usage: See ConnectionProxy, TopBar.

*/

using System;
using System.Collections.Generic;
using Hangout.Shared;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Web;

namespace Hangout.Client
{

	public class JSDispatcher : IDisposable
	{
		private static uint mCallbackCounter = 0;
		private static readonly Dictionary<string, bool> mAllowedCalls = new Dictionary<string, bool>();
		private Queue<Hangout.Shared.Action> mPendingCalls = new Queue<Hangout.Shared.Action>();
		private JSReceipt mOutstandingInitCall = null;

		public JSDispatcher()
		{
			// If running in the editor, this class does nothing
			if (Application.isEditor)
			{
				return;
			}

			// If we don't know which calls we're allowed to make, ask the unityDispatcher
			// in Javascript Land.  Asynchronous query, we'll be called back at the 
			// delegate we're defining below.  Yay for closure.
			if (mAllowedCalls.Count == 0)
			{
				mOutstandingInitCall = (JSReceipt)MakeForcedJSCall("getExposedFunctions",
																	new List<object>(),
																	delegate(string funcList)
																	{
																		try
																		{
																			mAllowedCalls.Clear();
																			string[] funcs = funcList.Split(new Char[] { ',' });

																			foreach (string s in funcs)
																			{
																				if (s != "")
																				{
																					mAllowedCalls.Add(s, true);
																				}
																			}

																			while (mPendingCalls.Count > 0)
																			{
																				Hangout.Shared.Action func = mPendingCalls.Dequeue();
																				func();
																			}
																		}
																		catch (System.Exception ex)
																		{
																			Console.WriteLine("Exception when JSDispatcher received exposed functions: " + ex.Message);
																		}
																	});
			}
		}

		public void Dispose()
		{
			mPendingCalls = null;
			if (mOutstandingInitCall != null)
			{
				mOutstandingInitCall.Exit();
				mOutstandingInitCall = null;
			}
		}

		~JSDispatcher()
		{
			Dispose();
		}

		// Make an immediate call to Javascript that bypasses our list of allowed functions
		// and will never be deferred until later.  Use this only if the call needs to 
		// occur before mAllowedCalls is populated.
		private IReceipt MakeForcedJSCall(string func, List<object> argList, Action<string> callbackFunc)
		{
			bool forcedKey = false;
			if (!mAllowedCalls.ContainsKey(func))
			{
				forcedKey = true;
				mAllowedCalls.Add(func, true);
			}

			IReceipt rec = MakeJSCall(func, argList, callbackFunc);

			if (forcedKey)
			{
				mAllowedCalls.Remove(func);
			}

			return rec;
		}

		
		private IEnumerator<IYieldInstruction> DoCallbackInSeconds(float seconds, Action<string> callback)
		{
			yield return new YieldForSeconds(seconds);
			callback("");
		}

		// Make a call to a Javascript function that has been exposed as an interface to Unity.
		// If mAllowedCalls has not yet been populated, the call will be deferred until it is.
		// Throws ArgumentException if we attempt to call a function that Javascript hasn't 
		// told us is in our interface.
		private IReceipt MakeJSCall(string func, List<object> argList, Action<string> callbackFunc)
		{
			// If in the editor, we to a basic emulation of what's gonna happen in browser.  Provide a dummy receipt.
			if (Application.isEditor)
			{
				if( callbackFunc != null )
				{
					IScheduler scheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
					scheduler.StartCoroutine(DoCallbackInSeconds(1.0f, callbackFunc));
				}
				return new JSReceipt(null);
			}

			if (mAllowedCalls.Count == 0)
			{
				JSReceipt earlyJSR = new JSReceipt(null);
				mPendingCalls.Enqueue(delegate()
				{
					// Fire off the call, but only if the early receipt didn't get killed
					if (!earlyJSR.hasExited())
					{
						JSReceipt realJSR = (JSReceipt)MakeJSCall(func, argList, callbackFunc);
						earlyJSR.UpdateGO(realJSR.GetGO());
					}
				});
				return earlyJSR;
			}

			if (!mAllowedCalls.ContainsKey(func))
			{
				throw new ArgumentException("Non-allowed function call to Javascript: " +
											func +
											".  Check JSDispatcher.mAllowedCalls!");
			}

			if (argList == null)
			{
				argList = new List<object>();
			}

			// Null callbackFunc is okay, we just won't do anything on callback.

			string cbId = "JSCallback" + mCallbackCounter++;
			GameObject go = new GameObject(cbId);
			JSCallback cb = (JSCallback)go.AddComponent(typeof(JSCallback));

			cb.Register(callbackFunc);

			argList.Insert(0, cbId);
			argList.Insert(1, "Callback");
			Application.ExternalCall("Hangout.unityDispatcher.unityToJSCalls." + func, argList.ToArray());

			return new JSReceipt(go);
		}


		// ---- Begin public interface ----

		public IReceipt RequestConfigValue(string arg, Action<string> cbFunc)
		{
			List<object> argList = new List<object>();
			argList.Add(arg);

			return MakeJSCall("requestConfigValue", argList, cbFunc);
		}

		public IReceipt RequestConfigObject(Action<string> cbFunc)
		{
			return MakeJSCall("requestConfigObject", null, cbFunc);
		}

		public IReceipt RequestFeedPost(string targetId,
                                        string feedType,
										string userMessagePrompt,
										string defaultUserMessage,
										string actionLinkName,
										string actionLinkURL,
										string attachmentName,
										string attachmentURL,
										string attachmentCaption,
										string attachmentDescription,
										string attachmentImageSrcURL,
										string attachmentImageLinkURL,
										Action<string> cbFunc)
		{
			List<object> argList = new List<object>();
			argList.Add(targetId);
            argList.Add(feedType);
			argList.Add(userMessagePrompt);
			argList.Add(defaultUserMessage);
			argList.Add(actionLinkName);
			argList.Add(actionLinkURL);
			argList.Add(attachmentName);
			argList.Add(attachmentURL);
			argList.Add(attachmentCaption);
			argList.Add(attachmentDescription);
			argList.Add(attachmentImageSrcURL);
			argList.Add(attachmentImageLinkURL);
			return MakeJSCall("requestFeedPost", argList, cbFunc);
		}

		public IReceipt RequestCashStore(string urlParams, Action<string> cbFunc)
		{
			List<object> argList = new List<object>();
			argList.Add(urlParams);
			return MakeJSCall("requestCashStore", argList, cbFunc);
		}

		public IReceipt RequestFriendInviter(string inviteType, Action<string> cbFunc)
		{
			List<object> argList = new List<object>();
			argList.Add(inviteType);
			return MakeJSCall("requestFriendInviter", argList, cbFunc);
		}

		public IReceipt OpenNewBrowserTab(string url, Action<string> cbFunc)
		{
			List<object> argList = new List<object>();
			argList.Add(url);

			return MakeJSCall("openNewBrowserTab", argList, cbFunc);
		}

		public IReceipt LogMetricsEvent(string eventName, string extraPropsJSON, Action<string> cbFunc)
		{
			if (extraPropsJSON == "")
			{
				extraPropsJSON = "{}";
			}

			List<object> argList = new List<object>();
			argList.Add(eventName);
			argList.Add(extraPropsJSON);

			return MakeJSCall("logMetricsEvent", argList, cbFunc);
		}

		public IReceipt LogMetricsFunnelStep(string funnelId, uint stepNum, string stepName, string extraPropsJSON, Action<string> cbFunc)
		{
			if (extraPropsJSON == "")
			{
				extraPropsJSON = "{}";
			}

			List<object> argList = new List<object>();
			argList.Add(funnelId);
			argList.Add(stepNum.ToString());
			argList.Add(stepName);
			argList.Add(extraPropsJSON);

			return MakeJSCall("logMetricsFunnelStep", argList, cbFunc);
		}

		public IReceipt LogToConsole(string level, string message, Action<string> cbFunc)
		{
			List<object> argList = new List<object>();
			//Regex newlines = new Regex("[\r\n]");
			//Regex quotes = new Regex("[\"\'\x60]");

			if (message[message.Length - 1] == "\n"[0])
			{
				message = message.Substring(0, message.Length - 1);
			}

			message = message.Replace("\'", "&#39;");
			message = message.Replace("\"", "&quot;");
			message = message.Replace("\r", "");
			message = message.Replace("\n", "<br>");

			argList.Add(level);
			argList.Add(message);
			return MakeJSCall("logToConsole", argList, cbFunc);
		}

        public IReceipt ToggleJSConsole()
        {
            return MakeJSCall("toggleJSConsole", new List<object>(), null);
        }
	}
}
