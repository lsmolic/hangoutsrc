/*
 * So Phergulous.
 * 10/08/09
 */
 
using System;
using System.Collections.Generic;
using System.Text;

using Hangout.Shared;

namespace Hangout.Client
{
	public class InputReceipt : IReceipt
	{
		private Shared.Action mUnregisterCallback;

		public InputReceipt(Shared.Action unregisterCallback)
		{
			mUnregisterCallback = unregisterCallback;
		}
		
		public void Exit()
		{
			mUnregisterCallback();
		}
	}
}
