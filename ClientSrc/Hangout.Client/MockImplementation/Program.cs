using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Hangout.Shared;
using System.Timers;
using System.ComponentModel;
using System.Threading;
using System.Configuration;

namespace Hangout.Client
{
    public class Program
    {
        private enum MainLoopStates
        {
            InitializationState,
            UnitTestRelectorStarting,
            UnitTestRelectorStarted,
            MockClientRelectorStarting,
            MockClientRelectorStarted
        }

        private static bool mFinished = false;
        private static MockClientMessageProcessor mockClientMessageProcessor;
        private static GameFacade mInstance;
        private static int mMainLoopState = (int) MainLoopStates.InitializationState;
        private static BackgroundWorker mBackgroundWorkerReceiver;

        public static int Main(string[] args)
        {
            int result = -1;

            string IPAddress = getServerIP(args);
            if (IPAddress == null)
            {
                Console.Error.WriteLine("ERROR: unable to resolve IP address");
                return -1;
            }

            int port = getPort(args);
            if (port == -1)
            {
                Console.Error.WriteLine("ERROR: unable to determine port");
                return -1;
            }

            string testResultDir = getOutputDir(args);
            
            mInstance = new GameFacade();
            mockClientMessageProcessor = new MockClientMessageProcessor(IPAddress, port);
            if (FindArg(args, "/t"))
            {
                bool multiple = false;
                if (FindArg(args, "/m"))
                {
                    multiple = true;
                }

                result = UnitTestLoop(testResultDir, multiple);
            }
            else
            {
                runMockClient();
            }

            return result;
        }

        private static string getOutputDir(object[] args)
        {
            string outDir = FindArgValue(args, "/o");

            if(outDir == null)
                outDir = ConfigurationManager.AppSettings["TestResultDir"];

            if (!Directory.Exists(outDir))
            {
                Console.Error.WriteLine("WARNING: output directory did not exist...setting to current working directory");
                outDir = Directory.GetCurrentDirectory();
            }

            return outDir;
        }

        private static int getPort(object[] args)
        {
            int port;
            
            string portStr = FindArgValue(args, "/p");

            if (portStr == null)
                int.TryParse(ConfigurationManager.AppSettings["StateServerPort"], out port);
            else
                int.TryParse(portStr, out port);

            return port;
        }

        private static string getServerIP(object[] args)
        {
            //Check to see if we are overriding the baked in config file value by
            // providing a command line parameter for the host name
            string IPAddress = FindArgValue(args, "/h");

            if (IPAddress == null)
                IPAddress = ConfigurationManager.AppSettings["StateServerAddress"];

            // If someone passed in a host name instead of an IP address
            // be nice and try to resolve it for them

            try
            {
                IPAddress[] ipAddr = Dns.GetHostAddresses(IPAddress);
                
                if (ipAddr.Length > 0)
                    return ipAddr[0].ToString();
            }
            catch (System.Net.Sockets.SocketException se)
            {
                // The system had problems resolving the address passed
                Console.WriteLine(se.Message.ToString());
                
            }

            return null;
        }

        private static bool FindArg(object[] args, string argValue)
        {
            bool findArg = false;

            foreach (object arg in args)
            {
                if ((string)arg == argValue)
                {
                    findArg = true;
                }
            }
            return findArg;
        }

        private static string FindArgValue(object[] args, string argValue)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if ((string)args[i] == argValue)
                    return (string)args[i + 1];
            }

            return null;
        }

        private static void runMockClient()          
        {
            Console.WriteLine("MOCK CLIENT STARTED...");

            mMainLoopState = (int) MainLoopStates.MockClientRelectorStarting;
            InitSchedulerLoop();

            while (!mFinished)
            {
                ProcessPaymentItemsCommand(mockClientMessageProcessor);
            }
        }


        private static int UnitTestLoop(string testResultDir, bool multiple) 
        {
            int result = -1;
            ConsoleKey key ;
            mFinished = false;
            result = RunUnitTests(testResultDir);

            if (multiple)
            {

                while ((key = Console.ReadKey(true).Key) != ConsoleKey.X)
                {
                    switch (key)
                    {
                        case ConsoleKey.R:
                            if (mFinished)
                            {
                                mFinished = false;
                                result = RunUnitTests(testResultDir);
                            }
                            break;
                    }
                }
            }

            return result;
        }

        private static int RunUnitTests(string testResultDir)
        {
            int result = -1;
            mMainLoopState = (int)MainLoopStates.InitializationState;
            InitSchedulerLoop();

            //wait for Initialization to complete
            while (mMainLoopState != (int) MainLoopStates.UnitTestRelectorStarted)
            {
                Thread.Sleep(50);
            }

            RunSystemUnitTests tests = new RunSystemUnitTests();
            result = tests.RunTests(testResultDir);

            mockClientMessageProcessor.UnRegisterAllMessages();
            mockClientMessageProcessor.DisconnectReflector();
            mBackgroundWorkerReceiver.CancelAsync();

            return result;
        }

        public static MockClientMessageProcessor GetClientMessageProcessor
        {
            get { return mockClientMessageProcessor; }
        }

        public static GameFacade GetGameFacade
        {
            get { return mInstance; }
        }

        
        public static void InitSchedulerLoop()
        {
            mBackgroundWorkerReceiver = new BackgroundWorker();
            mBackgroundWorkerReceiver.WorkerSupportsCancellation = true;
            mBackgroundWorkerReceiver.DoWork += new DoWorkEventHandler(BackGroundDoWork);
            mBackgroundWorkerReceiver.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackGroundComplete);
            mBackgroundWorkerReceiver.RunWorkerAsync();
        }

        private static void BackGroundDoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                switch (mMainLoopState)
                {
                    case (int) MainLoopStates.InitializationState:
                        mMainLoopState = (int) MainLoopStates.UnitTestRelectorStarting;
                        mockClientMessageProcessor.StartReflector(mInstance);
                        break;

                    case (int) MainLoopStates.UnitTestRelectorStarting:
                        mMainLoopState = (int)MainLoopStates.UnitTestRelectorStarted;
                        break;

                    case (int)MainLoopStates.MockClientRelectorStarting:
                        mMainLoopState = (int)MainLoopStates.MockClientRelectorStarted;
                        mockClientMessageProcessor.StartReflector(mInstance);
                        mockClientMessageProcessor. LoginUser();
                        break;
                        
                    default:
                        mockClientMessageProcessor.RequestMessageFromReflector();
                        mInstance.SchedulerLoop();
                        break;
                }

                if (mBackgroundWorkerReceiver.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }

                Thread.Sleep(50);
            }
        }

        private static void BackGroundComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            mFinished = true;
        }

        private static void ProcessPaymentItemsCommand(MockClientMessageProcessor mockClientMessageProcessor)
        {
            Dictionary<string, string> commandParameters = new Dictionary<string, string>();
            ConsoleKey key = Console.ReadKey(false).Key;

            switch (key)
            {
                case ConsoleKey.A:
                    commandParameters.Add("userSession", mockClientMessageProcessor.SessionId);
                    commandParameters.Add("amount", "200");
                    mockClientMessageProcessor.ProcessPaymentItemCommand("AddVirtualCoinForUser", commandParameters);
                    break;

                case ConsoleKey.B:
                    commandParameters.Add("userSession", mockClientMessageProcessor.SessionId);
                    mockClientMessageProcessor.ProcessPaymentItemCommand("GetUserBalance", commandParameters);
                    break;

                case ConsoleKey.H:
                    mockClientMessageProcessor.ProcessPaymentItemCommand("HealthCheck", commandParameters);
                    break;

                case ConsoleKey.I:
                    commandParameters.Add("userSession", mockClientMessageProcessor.SessionId);
                    commandParameters.Add("startIndex", "0");
                    commandParameters.Add("blockSize", "10");
                    commandParameters.Add("itemTypeNames", "Pants, Bags");
                    mockClientMessageProcessor.ProcessPaymentItemCommand("GetUserInventory", commandParameters);
                    break;

                case ConsoleKey.S:
                    commandParameters.Add("storeName", "Hangout_Store_Combined");
                    commandParameters.Add("startIndex", "0");
                    commandParameters.Add("blockSize", "9");
                    mockClientMessageProcessor.ProcessPaymentItemCommand("GetStoreInventory", commandParameters);
                    break;

                case ConsoleKey.O:
                    commandParameters.Add("userSession", mockClientMessageProcessor.SessionId);
                    mockClientMessageProcessor.ProcessPaymentItemCommand("GameCurrencyOffers", commandParameters);
                    break;

                case ConsoleKey.P:
                    commandParameters.Add("userSession", mockClientMessageProcessor.SessionId);
                    commandParameters.Add("offerId", "29");
                    mockClientMessageProcessor.ProcessPaymentItemCommand("PurchaseGameCurrencyPayPal", commandParameters);
                    break;

                case ConsoleKey.C:
                    mockClientMessageProcessor.LaunchMoneyAccountFunding();
 

               //     string UserId = crypt.TDesEncrypt(mockClientMessageProcessor.);

                    //commandParameters.Add("userSession", mockClientMessageProcessor.SessionId);
                    //commandParameters.Add("offerId", "29");
                    //commandParameters.Add("creditCardNumber", "4159662533655864");
                    //commandParameters.Add("creditCardType", "VISA");
                    //commandParameters.Add("expireDate", "092018");
                   // commandParameters.Add("securityCode", "000");
                   // commandParameters.Add("firstName", "Test");
                   // commandParameters.Add("lastName", "User");
                   // commandParameters.Add("address", "1 Main St");
                   // commandParameters.Add("city", "San Jose");
                   // commandParameters.Add("state", "CA");
                   // commandParameters.Add("zipCode", "95131");
                   // commandParameters.Add("countryCode", "US");
                   // commandParameters.Add("phoneNumber", "1234567890");
                  //  mockClientMessageProcessor.ProcessPaymentItemCommand("PurchaseGameCurrencyCreditCard", commandParameters);
                    break;


                case ConsoleKey.X:
                    commandParameters.Add("userSession", mockClientMessageProcessor.SessionId);
                    commandParameters.Add("currencyName", "VCOIN");
                    commandParameters.Add("offerIds", "2207");
                    mockClientMessageProcessor.ProcessPaymentItemCommand("PurchaseItems", commandParameters);
                    break;


                case ConsoleKey.G:
                    commandParameters.Add("userSession", mockClientMessageProcessor.SessionId);
                    commandParameters.Add("currencyName", "HOUTS");
                    commandParameters.Add("offerIds", "82");
                    commandParameters.Add("recipientUserId", "549");
                    commandParameters.Add("noteToRecipient", "This is a Gift");
                    mockClientMessageProcessor.ProcessPaymentItemCommand("PurchaseItemsGift", commandParameters);
                    break;

                case ConsoleKey.T:
                    RunSystemUnitTests tests = new RunSystemUnitTests();
                    tests.RunTests("");
                    break;
                    
                case ConsoleKey.F1:
                    Console.WriteLine("*** Payment Commands: ***\n");
                    Console.WriteLine("A  AddVirtualCoinForUser 200 VCOIN ");
                    Console.WriteLine("B  GetUserBalance ");
                    Console.WriteLine("C  PurchaseGameCurrencyCreditCard ");
                    Console.WriteLine("G  PurchaseItemsGift ");
                    Console.WriteLine("I  GetUserInventory ");
                    Console.WriteLine("O  PurchaseGameCurrencyOffers ");
                    Console.WriteLine("P  PurchaseGameCurrencyPayPal ");
                    Console.WriteLine("S  GetStoreInventory ");
                    Console.WriteLine("T  Automated Server Tests ");
                    Console.WriteLine("X  PurchaseItems ");
                    break;

                default:
                    break;
            }
        }
    }
}
