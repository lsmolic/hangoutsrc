/**  --------------------------------------------------------  *
 *   InputManagerMediator.cs  
 *
 *   Author: Matt, Hangout Industries
 *   Date: 07/07/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;

using System;
using System.Collections.Generic;
using PureMVC.Patterns;
using Hangout.Shared;

namespace Hangout.Client
{
	public class InputManagerMediator : Mediator, IInputManager
	{
		private readonly IDictionary<KeyCode, List<Hangout.Shared.Action>> mKeyCodeToCallbackForButtonDown = new Dictionary<KeyCode, List<Hangout.Shared.Action>>();
		private readonly IDictionary<KeyCode, List<Hangout.Shared.Action>> mKeyCodeToCallbackForButtonUp = new Dictionary<KeyCode, List<Hangout.Shared.Action>>();
		private readonly List<MousePositionCallback> mListOfCallbacksForMousePosition = new List<MousePositionCallback>();
		
		public Vector3 MousePosition
		{
			get { return Input.mousePosition; }
		}

		public override IList<string> ListNotificationInterests()
		{
			return new string[] 
            {
                GameFacade.APPLICATION_INIT,
            };
		}

		public override void HandleNotification(PureMVC.Interfaces.INotification notification)
		{
			base.HandleNotification(notification);
			switch (notification.Name)
			{
				case GameFacade.APPLICATION_INIT:
					StartInput();
					break;
			}
		}

		public void StartInput()
		{
			//this includes listening for mouse up and down buttons as well as from the keyboard
			IScheduler scheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			scheduler.StartCoroutine(ListenForButtonDown());
			scheduler.StartCoroutine(ListenForButtonUp());
			scheduler.StartCoroutine(ListenForMousePosition());
		}

		public IReceipt RegisterForButtonDown(KeyCode keyCode, Hangout.Shared.Action callback)
		{
			GenericRegisterButtonFunction(keyCode, callback, mKeyCodeToCallbackForButtonDown);
			return new InputReceipt(delegate()
			{
				UnregisterForButtonDown(keyCode, callback);
			});
		}

		public IReceipt RegisterForButtonUp(KeyCode keyCode, Hangout.Shared.Action callback)
		{
			GenericRegisterButtonFunction(keyCode, callback, mKeyCodeToCallbackForButtonUp);
			return new InputReceipt(delegate()
			{
				UnregisterForButtonUp(keyCode, callback);
			});
		}

		private void GenericRegisterButtonFunction(KeyCode keyCode, Hangout.Shared.Action callback, IDictionary<KeyCode, List<Hangout.Shared.Action>> directoryToRegisterWith)
		{
			if (!directoryToRegisterWith.ContainsKey(keyCode))
			{
				List<Hangout.Shared.Action> newListOfActions = new List<Hangout.Shared.Action>();
				newListOfActions.Add(callback);
				directoryToRegisterWith.Add(keyCode, newListOfActions);
			}
			else
			{
				directoryToRegisterWith[keyCode].Add(callback);
			}
		}
		
		private void UnregisterForButtonDown(KeyCode keyCode, Hangout.Shared.Action callback)
		{
			GenericUnregisterButtonFuction(keyCode, callback, mKeyCodeToCallbackForButtonDown);
		}

		private void UnregisterForButtonUp(KeyCode keyCode, Hangout.Shared.Action callback)
		{
			GenericUnregisterButtonFuction(keyCode, callback, mKeyCodeToCallbackForButtonUp);
		}
		
		public void GenericUnregisterButtonFuction(KeyCode keyCode, Hangout.Shared.Action callback, IDictionary<KeyCode, List<Hangout.Shared.Action>> directoryToRegisterWith)
		{
			if (directoryToRegisterWith[keyCode].Contains(callback))
			{
				directoryToRegisterWith[keyCode].Remove(callback);
			}
		}

		public IReceipt RegisterForMousePosition(MousePositionCallback callback)
		{
			mListOfCallbacksForMousePosition.Add(callback);
			return new InputReceipt(delegate()
			{
				UnregisterForMousePosition(callback);
			});
		}

		public void UnregisterForMousePosition(MousePositionCallback callback)
		{
			if (mListOfCallbacksForMousePosition.Contains(callback))
			{
				mListOfCallbacksForMousePosition.Remove(callback);
			}
		}

		private IEnumerator<IYieldInstruction> ListenForButtonDown()
		{
			return GenericListeningForButtonFunction(mKeyCodeToCallbackForButtonDown, Input.GetKeyDown);
		}

		private IEnumerator<IYieldInstruction> ListenForButtonUp()
		{
			return GenericListeningForButtonFunction(mKeyCodeToCallbackForButtonUp, Input.GetKeyUp);
		}

		private IEnumerator<IYieldInstruction> GenericListeningForButtonFunction(IDictionary<KeyCode, List<Hangout.Shared.Action>> dictionaryOfKeyCodesToCallbacks, Hangout.Shared.Func<bool, KeyCode> checkFunc)
		{
			while (true)
			{
				foreach (KeyCode keyCode in dictionaryOfKeyCodesToCallbacks.Keys)
				{
					if (checkFunc(keyCode))
					{
						List<Hangout.Shared.Action> listOfCallbacks = dictionaryOfKeyCodesToCallbacks[keyCode];
						foreach (Hangout.Shared.Action callback in listOfCallbacks)
						{
							callback();
						}
					}
				}
				yield return new YieldUntilNextFrame();
			}
		}

		private IEnumerator<IYieldInstruction> ListenForMousePosition()
		{
			while (true)
			{
				foreach (MousePositionCallback mousePositionCallback in mListOfCallbacksForMousePosition)
				{
					mousePositionCallback(Input.mousePosition);
				}
				yield return new YieldUntilNextFrame();
			}
		}
	}
}
