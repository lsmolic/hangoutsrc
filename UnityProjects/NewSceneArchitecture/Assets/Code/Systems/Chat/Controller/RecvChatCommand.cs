/*=============================================================
(c) all rights reserved
================================================================*/
using UnityEngine;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace Hangout.Client
{
	class RecvChatCommand:SimpleCommand, ICommand
	{
		override public void Execute(INotification notification)
		{
		    Console.Log("In RecvChatCommand.execute");
		}       
		
	}
	
}
