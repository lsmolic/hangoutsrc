using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using Hangout.Shared;
using UnityEngine;

namespace Hangout.Client
{
    public class RequestConnectionCommand : SimpleCommand
    {
        override public void Execute(INotification notification)
        {
			GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("RequestConnectionCommand.Execute", LogLevel.Info);
			ConfigManagerClient configManager = GameFacade.Instance.RetrieveProxy<ConfigManagerClient>();
            ConnectionProxy connectionProxy = GameFacade.Instance.RetrieveProxy<ConnectionProxy>();
            connectionProxy.SetConfiguration(configManager);

            if (Application.isEditor || configManager.GetBool("override_boss_server", false))
            {
				GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("RequestConnectionCommand, using in editor settings, connection ip " + connectionProxy.IpAddress + " port " + connectionProxy.Port, LogLevel.Info);
				GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>().Connect(connectionProxy.IpAddress, connectionProxy.Port);
                Console.Log("connection ip " + connectionProxy.IpAddress + " port " + connectionProxy.Port);
            }
            else
            {
                string resolvedPath = connectionProxy.WebServicesBaseUrl + "/Boss/GetStateServer";
				GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("RequestConnectionCommand boss server path = " + resolvedPath, LogLevel.Info);
				GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>().CallToService(resolvedPath, delegate(XmlDocument serverDocument)
                {
					//Expected format:
                    //<ConnectTo>
                    //   <StateServer Ip="64.106.173.25" Port="8000" Population="0" />
                    //</ConnectTo>
                    XmlElement serverNode = (XmlElement)serverDocument.SelectSingleNode("/ConnectTo/StateServer");
                    string ipAddress = serverNode.GetAttribute("Ip");
                    int port = int.Parse(serverNode.GetAttribute("Port"));

					GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("Connecting to state server at " + ipAddress + ":" + port, LogLevel.Info);
                    Console.WriteLine("Connecting to state server at " + ipAddress + ":" + port);

                    GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>().Connect(ipAddress, port);
                });
            }
		}
    }
}
