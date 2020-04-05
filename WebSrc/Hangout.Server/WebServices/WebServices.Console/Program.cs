using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Server.WebServices
{
    class Program
    {
        static void Main(string[] args)
        {
            PaymentItemConnect paymentItemConnect = null;
            ServiceConnect serviceConnect = null;

            serviceConnect = new ServiceConnect();
            serviceConnect.Listen();
            serviceConnect.RegisterWellKnownServiceType();

            paymentItemConnect = new PaymentItemConnect();
            paymentItemConnect.RegisterWellKnownServiceType();

            ConsoleKey key = ConsoleKey.Spacebar;

            while (key != ConsoleKey.Enter)
            {
                key = Console.ReadKey(false).Key;
            }
            Console.WriteLine("Exiting Cleanly ");
        }
    }
}
