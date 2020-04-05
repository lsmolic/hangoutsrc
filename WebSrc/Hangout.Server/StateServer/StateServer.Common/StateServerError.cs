using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Server.StateServer
{
	public static class StateServerError
	{
		public static Message ErrorToUser(ErrorIndex errorIndex, MessageSubType errorActionType)
		{
			List<object> errorMessageData = new List<object>();
			errorMessageData.Add(errorIndex);
			
			Message errorMessage = new Message();
			errorMessage.Callback = (int)errorActionType;
			errorMessage.ErrorMessage(errorMessageData);
			return errorMessage;
		}
	}
}
