using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using PureMVC.Patterns;

namespace Hangout.Client
{
	public class UserAccountProxy : Proxy
	{
		private UserProperties mUserProperties = null;

		public UserAccountProxy(UserProperties userProperties)
		{
			mUserProperties = userProperties;
		}

        public override string ToString()
        {
            return mUserProperties.ToString();
        }

		public bool SetAccountProperty<T>(UserAccountProperties userAccountProperty, T value)
		{
			try
			{
				mUserProperties.SetProperty(userAccountProperty, value);
                SaveUserProperties();
				return true;
			}
			catch (System.Exception)
			{
				return false;
			}
		}

		public bool TryGetAccountProperty<T>(UserAccountProperties userAccountProperty, ref T returnObject)
		{
			object findObjectInDictionary = null;
			if (mUserProperties.TryGetProperty(userAccountProperty, out findObjectInDictionary))
			{
				try
				{
					returnObject = (T)findObjectInDictionary;
					return true;
				}
				catch (System.Exception ex)
				{
					throw ex;
				}
			}
			return false;
		}

		public void SaveUserProperties()
		{
			ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
			if(clientMessageProcessor != null)
			{
				List<object> saveUserPropertiesMessageData = new List<object>();
				saveUserPropertiesMessageData.Add(mUserProperties);

				Message saveUserPropertiesMessage = new Message();
				saveUserPropertiesMessage.Callback = (int)MessageSubType.SaveUserProperties;
				saveUserPropertiesMessage.AccountMessage(saveUserPropertiesMessageData);

				clientMessageProcessor.SendMessageToReflector(saveUserPropertiesMessage);
			}
			else
			{
				throw new Exception("Unable to get ClientMessageProcessor mediator.");
			}
		}
	}
}
