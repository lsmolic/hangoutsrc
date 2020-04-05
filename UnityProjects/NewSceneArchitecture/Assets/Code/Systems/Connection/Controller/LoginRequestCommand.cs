/*=============================================================
(c) all rights reserved
================================================================*/
using System;
using System.Collections.Generic;
using UnityEngine;

using PureMVC.Interfaces;
using PureMVC.Patterns;

using Hangout.Client.Gui;

namespace Hangout.Client
{
	class LoginRequestCommand : SimpleCommand, ICommand
	{
		override public void Execute(INotification notification)
		{
		    List<object> loginData = (List<object>)notification.Body;
		    
		    LoginProxy loginProxy = Facade.RetrieveProxy<LoginProxy>();
		    loginProxy.ProcessLoginInfo(loginData);
		}    
		
	}
	
}
