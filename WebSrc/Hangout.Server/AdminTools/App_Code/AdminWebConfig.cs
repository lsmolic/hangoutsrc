using System;
using System.Data;
using System.Configuration;


/// <summary>
/// Summary description for WebConfig
/// </summary>
public static class AdminWebConfig
{
	public static readonly string WebServicesBaseUrl = ConfigurationSettings.AppSettings["WebServicesBaseUrl"];
}
