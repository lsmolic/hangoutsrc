using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Threading;
using System.Configuration;
using System.Diagnostics;
using Hangout.Shared.UnitTest;
using Hangout.Shared;

namespace Hangout.Client
{
    public class SystemUnitTestsBase
    {
        static AutoResetEvent autoEvent = new AutoResetEvent(false);

        private MockClientMessageProcessor mClientMessageProcessor = Program.GetClientMessageProcessor;

        private delegate void AssertObjectDelegate(object object1);
        private delegate void AssertStringOneArgDelegate(string string1);
        private delegate void AssertStringTwoArgDelegate(string string1, string string2);

        private string mAssertFailMessage = "";
        private string mAssertFilename = "";
        private string mAssertFunction = "";
        private int mAssertLineNumber = -1;
        private string mAssertType = "";

        protected string GetMyIpAddress
        {
            get { return mClientMessageProcessor.GetMyIpAddress(); }
        }

        protected Guid SessionId
        {
            get { return new Guid (mClientMessageProcessor.SessionId); }
            set { mClientMessageProcessor.SetSessionId(value); }
        }

        protected void RegisterMessage(MessageType messageType, Action<Message> callback)
        {
            mClientMessageProcessor.RegisterMessageType(messageType, callback);
        }

        protected void ConnectToStateServer()
        {
            mClientMessageProcessor.StartReflector(Program.GetGameFacade);
            WaitForEvent(MessageType.Connect);
        }

        protected void TestComplete()
        {
            TestDebugInfo("TestComplete", MessageType.Admin, mAssertFunction);
            autoEvent.Set();
        }
        
        protected void StartTest(Message message)
        {
            mAssertFailMessage = "";
            mAssertFilename = "";
            mAssertFunction = "";
            mAssertLineNumber = -1;
            mAssertType = "";

            StackFrame functionThatEnteredAssert = GetTheTestFunctionInfo();

            if (functionThatEnteredAssert != null)
            {
                mAssertFunction = functionThatEnteredAssert.GetMethod().Name;
                mAssertFilename = functionThatEnteredAssert.GetFileName();
                mAssertLineNumber = functionThatEnteredAssert.GetFileLineNumber();
            }

            TestInfo("Test", message.MessageType, mAssertFunction);

            TestDebugInfo("**1**BeginTest ", message.MessageType, mAssertFunction);

            mClientMessageProcessor.SendMessageToReflector(message);
            WaitForEvent(message.MessageType);

        }

        private void WaitForEvent(MessageType messageType)
        {
            autoEvent.Reset();
            if (!autoEvent.WaitOne(30000))
            {
                TestDebugInfo("**2**TimeOut Event ", messageType, mAssertFunction);
                if (String.IsNullOrEmpty(mAssertType))
                {
                    TestDebugInfo("**3**Test Timed Out", messageType, mAssertFunction);
                    Assert.Fail("Test Timed Out");
                }
                else
                {
                    TestDebugInfo("**4**AssertionFailedException", messageType, mAssertFunction);
                    throw new AssertionFailedException(mAssertFailMessage, mAssertFunction, mAssertFilename, mAssertLineNumber);
                }
            }
            else
            {
                TestDebugInfo("**5**Event Test Complete", messageType, mAssertFunction);
                if (!String.IsNullOrEmpty(mAssertType))
                {
                    TestDebugInfo("**6**AssertionFailedException", messageType, mAssertFunction);
                    throw new AssertionFailedException(mAssertFailMessage, mAssertFunction, mAssertFilename, mAssertLineNumber);
                }
            }
            TestDebugInfo("WaitOne End", messageType, mAssertFunction);
        }

        protected object ReceiveMessage(Message message)
        {
            TestDebugInfo("ReceiveMessage", message.MessageType, mAssertFunction);
   
            object data = null;

            if (message.Data.Count > 0)
            {
                data = message.Data[0];
            }
            return data;
        }

        protected void ProcessPaymentItemCommand(string command, Dictionary<string, string> commandArgs)
        {
            string paymentItemsCommand = mClientMessageProcessor.CreatePaymentCommand(command, commandArgs);
            Message PaymentItemsMessage = new Message();
            List<object> dataObject = new List<object>();
            dataObject.Add(paymentItemsCommand);

            PaymentItemsMessage.PaymentItemsMessage(dataObject);
            StartTest(PaymentItemsMessage);
        }

        protected void VerifyStringData(object data)
        {
            AssertObject("IsNotNull", data);
            AssertString("IsNotEmptyString", data.ToString());
        }

        
        protected XmlDocument VerifyXMLDocument(string document)
        {
            XmlDocument xmlDoc = new XmlDocument();

            AssertObject("IsNotNull", document);
            AssertString("IsNotEmptyString", document.Trim());
 
            try
            {
                xmlDoc.LoadXml(document);
            }

            catch 
            {
                Assert.Fail("XML document did not load");
            }

            return xmlDoc;
        }

        protected string SelectSingleNodeWithVerify(XmlDocument document, string xPath, string compareString)
        {
            string element = "";

            element = GetSingleNodeElement(document, xPath);
            AssertString("AreEqual", element, compareString);

            //Console.WriteLine("SelectSingleNodeWithVerify Node:{0}, Element:{1} Compare:{2}", xPath, element, compareString);
            
            return element;
        }

        protected void SelectNodeListWithVerify(XmlNodeList nodeList, string xPath)
        {
            string attributeName = "";

            foreach (XmlNode node in nodeList)
            {
                attributeName = GetSingleNodeFirstAttributeName(node, xPath);
                // Console.WriteLine("SelectSingleNodeWithVerify Node:{0}, AttributeName:{1} ", node.Name, attributeName);
                AssertString("IsNotEmptyString", attributeName.Trim());
            }
        }
        
        protected void SelectNodeListWithVerify(XmlDocument document, string nodeListXpath, List<string> xPathsToVerify)
        {
            XmlNodeList nodeList = document.SelectNodes(nodeListXpath);

            AssertObject("IsNotNull", nodeList);

            foreach (string item in xPathsToVerify)
            {
                SelectNodeListWithVerify(nodeList, item);
            }
        }

        protected string GetSingleNodeElement(XmlNode node, string xPath)
        {
            string element = "";

            XmlNode selectedNode = node.SelectSingleNode(xPath);
            AssertObject("IsNotNull", selectedNode);
            element = selectedNode.InnerText.Trim();

            AssertString("IsNotEmptyString", element.Trim());

            return (element);
        }

        protected string GetSingleNodeFirstAttributeName(XmlNode node, string xPath)
        {
            string attibuteName = "";

            XmlNode selectedNode = node.SelectSingleNode(xPath);
            AssertObject("IsNotNull", selectedNode);
            attibuteName = selectedNode.Attributes[0].Name;

            AssertString("IsNotEmptyString", attibuteName.Trim());

            return (attibuteName);
        }


        private void TestInfo(string title, MessageType messageType, string testName)
        {
            Console.Write(String.Format("{0}: {1} \r\n", title, testName));
        }


        private void TestDebugInfo(string title, MessageType messageType, string testName)
        {
            DateTime dt = DateTime.Now;
            string time = String.Format("{0}.{1}", dt.Second.ToString(), dt.Millisecond.ToString());

           // Console.Write(String.Format("{0}: {1} Name: {2} Time: {3}\r\n", title, messageType, testName, time));
        }

        protected void AssertObject(string method, object objectToTest)
        {
            AssertObjectDelegate assertDelegate = null;
            switch(method)
            {
                case "IsNull":
                    assertDelegate = new AssertObjectDelegate(Assert.IsNull);
                    break;

                case "IsNotNull":
                    assertDelegate = new AssertObjectDelegate(Assert.IsNotNull);
                    break;
            }

            if (assertDelegate != null)
            {
                CallObjectAssertMethod(assertDelegate, objectToTest);
            }
        }

        
        private void CallObjectAssertMethod(AssertObjectDelegate assertMethod, object objectToTest)
        {
            try
            {
                assertMethod.Invoke(objectToTest);
            }

            catch (AssertionFailedException ex)
            {
                mAssertFailMessage = ex.Message;
                mAssertType = assertMethod.Method.Name;
                TestDebugInfo("WaitOne End", MessageType.Admin, mAssertFunction);

                autoEvent.Set();
             }
        }

        protected void AssertString(string method, string stringToTest)
        {
            AssertStringOneArgDelegate assertDelegate = null;

            switch (method)
            {
                case "IsNotEmptyString":
                    assertDelegate = new AssertStringOneArgDelegate(Assert.IsNotEmptyString);
                    break;
            }

            if (assertDelegate != null)
            {
                CallStringAssertMethod(assertDelegate, stringToTest);
            }
        }

        private void CallStringAssertMethod(AssertStringOneArgDelegate assertMethod, string objectToTest) 
        {
            try
            {
                assertMethod.Invoke(objectToTest);
            }

            catch (AssertionFailedException ex)
            {
                mAssertFailMessage = ex.Message;
                mAssertType = assertMethod.Method.Name;
                TestDebugInfo("WaitOne End", MessageType.Admin, mAssertFunction);

                autoEvent.Set();
            }
        }


        protected void AssertString(string method, string stringToTest1, string stringToTest2)
        {
            AssertStringTwoArgDelegate assertDelegate = null;

            switch (method)
            {
                case "AreEqual":
                    assertDelegate = new AssertStringTwoArgDelegate(Assert.AreEqual<string>);
                    break;
            }

            if (assertDelegate != null)
            {
                CallStringAssertMethod(assertDelegate, stringToTest1, stringToTest2);
            }
        }

        private void CallStringAssertMethod(AssertStringTwoArgDelegate assertMethod, string stringToTest1, string stringToTest2)
        {
            try
            {
                assertMethod.Invoke(stringToTest1, stringToTest2);
            }

            catch (AssertionFailedException ex)
            {
                mAssertFailMessage = ex.Message;
                mAssertType = assertMethod.Method.Name;
                TestDebugInfo("WaitOne End", MessageType.Admin, mAssertFunction);

                autoEvent.Set();
            }
        }




        private StackFrame GetTheTestFunctionInfo()
        {
            // Find the function that called Assert
            StackTrace stackTrace = new StackTrace();
            StackFrame thisFunctionsFrame = stackTrace.GetFrame(0);
            StackFrame functionThatEnteredAssert = null;
            foreach (StackFrame frame in stackTrace.GetFrames())
            {
                if (IsATestMethod(frame.GetMethod()))
                {
                    functionThatEnteredAssert = frame;
                    break;
                }
            }
            return functionThatEnteredAssert;
        }

        private bool IsATestMethod(System.Reflection.MethodBase method)
        {
            foreach (Attribute attrib in Attribute.GetCustomAttributes(method))
            {
                if (attrib.GetType() == typeof(Test))
                {
                    return true;
                }
            }
            return false;
        }

        protected AvatarId ConvertStringToAvatarIdWithAssert(string avatarStringId)
        {
            try
            {
                return (new AvatarId(Convert.ToUInt16(avatarStringId)));
            }

            catch
            {
                Assert.Fail(String.Format("ConvertStringToAvatarIdWithAssert Failed: {0}", avatarStringId));
            }

            return null;
        }

        protected long ConvertToLongWithAssert(string value)
        {
            try
            {
                return Convert.ToInt64(value);
            }

            catch
            {
                Assert.Fail(String.Format("ConvertToLongWithAssert Failed: {0}", value));
            }

            return -1;

        }

        // Show how to use GetSection.
        protected string GetConfigFileAppSettingWithAssert(string section, string settingKey)
        {
            try
            {
                Console.WriteLine("GetConfigFileAppSetting section:{0} key:{1}", section, settingKey);

                // Get the configuration file.
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // Get the AppSetins section.
                ConfigurationSection appSettingSection = config.GetSection(section);

                XmlDocument documentConfig = new XmlDocument();

                documentConfig.LoadXml(appSettingSection.SectionInformation.GetRawXml());

                string settingValue = documentConfig.SelectSingleNode(section + "/add[@key='" + settingKey + "']/@value").InnerXml;

                Console.WriteLine("GetConfigFileAppSetting section:{0} key:{1} value:{2}", section, settingKey, settingValue);

                return settingValue;
            }
            catch
            {
                Assert.Fail(String.Format("GetConfigFileAppSettingWithAssert Failed: {0} {1}", section, settingKey));
            }

            return null;
        }
     }
}
