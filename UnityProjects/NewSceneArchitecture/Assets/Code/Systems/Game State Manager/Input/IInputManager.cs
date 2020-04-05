/**  --------------------------------------------------------  *
 *   IInputManager.cs  
 *
 *   Author: Matt, Hangout Industries
 *   Date: 07/07/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client
{
	public delegate void MousePositionCallback(Vector3 mousePositionInPixelCoordinates);

	public interface IInputManager 
	{
		IReceipt RegisterForButtonDown(KeyCode keyCode, Hangout.Shared.Action callback);
		IReceipt RegisterForButtonUp(KeyCode keyCode, Hangout.Shared.Action callback);
		IReceipt RegisterForMousePosition(MousePositionCallback callback);
		Vector3 MousePosition { get;}
	}
}