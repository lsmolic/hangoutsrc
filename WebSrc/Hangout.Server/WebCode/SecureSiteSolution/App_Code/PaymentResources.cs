using System;
using System.Resources;
using Hangout.Server;

/// <summary>
/// Summary description for Resources
/// </summary>
public static class PaymentResources
{
    public static string GetStringFromResourceFile(string key)
    {
        return (PaymentItemResourceHandler.GetStringFromResourceFile(key));
    }
}