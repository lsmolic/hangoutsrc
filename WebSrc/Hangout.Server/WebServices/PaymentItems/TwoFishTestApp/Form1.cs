using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;
using Hangout.Server;
using System.IO;
using Hangout.Shared;

namespace Hangout.Server.WebServices
{
    public partial class Form1 : Form
    {

        /// <summary>
        /// PaymentItemUserId,  HangoutUserId
        /// </summary>
        private Dictionary<string, string> mUserIdList = new Dictionary<string, string>();

        public Dictionary<string, string> GetUserList
        {
            get { return mUserIdList; }
        }

		private void Form1_Resize(object sender, System.EventArgs e)
		{
			this.treeView1.Size = new System.Drawing.Size(this.Width - 60, 454);  //this.Height - 148
			this.treeView1.Update();
		}


        public Form1()
        {
            InitializeComponent();

            radioButton1.Checked = false;
            radioButton2.Checked = true;

            mUserIdList.Add("545", "");
            mUserIdList.Add("547", "");
            mUserIdList.Add("549", "");
            mUserIdList.Add("551", "");
            mUserIdList.Add("554", "");
            mUserIdList.Add("556", "");
            mUserIdList.Add("558", "");
            mUserIdList.Add("559", "");
            mUserIdList.Add("576", "");
            mUserIdList.Add("574", "");
            mUserIdList.Add("652", "");
            mUserIdList.Add("721", "1000011");
            mUserIdList.Add("734", "1000008");

            CreateTwoFishCommand cmdTwoFish = new CreateTwoFishCommand();
            Type cmdType = cmdTwoFish.GetType();
            PopulateComboBox(cmdType, comboBox1);

            CreateHangoutCommand cmdHangout = new CreateHangoutCommand();
            cmdType = cmdHangout.GetType();
            PopulateComboBox(cmdType, comboBox3);

            foreach (KeyValuePair<string, string> keyValue in mUserIdList)
            {
                comboBox2.Items.Add(keyValue.Key);
            }
        }

        private void PopulateComboBox(Type cmdType, ComboBox combo)
        {
            foreach (MemberInfo methodInfo in cmdType.GetMembers())
            {
                if (methodInfo.DeclaringType.Name == cmdType.Name && methodInfo.Name != ".ctor")
                {
                    combo.Items.Add(PrettyDisplay(methodInfo.Name));
                }
            }
        }

        public string InvokeOpenFileDialog(string filter)
        {
            string fileName = "";

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = filter;

            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK) // Test result.
            {
                fileName = dialog.FileName;
            }

            return fileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedItem = "";
                Type cmdType = null;

                if (radioButton1.Checked)
                {
                    selectedItem = (string)comboBox1.SelectedItem;
                    CreateTwoFishCommand cmd = new CreateTwoFishCommand();
                    cmdType = cmd.GetType();
                }
                else
                {
                    selectedItem = (string)comboBox3.SelectedItem;
                    CreateHangoutCommand cmd = new CreateHangoutCommand();
                    cmdType = cmd.GetType();
                }

                string selectedUser = (string)comboBox2.SelectedItem;

                if (selectedItem != null)
                {
                    ServiceCommandSerializer paymentSerializer = new ServiceCommandSerializer();

                    PaymentCommand command = CreateCommand(selectedItem, selectedUser, cmdType, GetSelectedXml());
                    string xmlCommand = paymentSerializer.SerializeCommandData((ServiceCommand)command, typeof(PaymentCommand));

                    StreamWriter swCommand = new StreamWriter("c:\\twofishCommand.xml");
                    swCommand.Write(xmlCommand);
                    swCommand.Close();

                    command = (PaymentCommand)paymentSerializer.DeserializeCommandData(xmlCommand, typeof(PaymentCommand));

                    PaymentCommand commandSecond = HandleSpecialCommand(command, selectedUser, cmdType);
                    if (commandSecond != null)
                    {
                        command = commandSecond;
                    }

                    PaymentItem paymentItem = new PaymentItem();
                    XmlDocument response = paymentItem.ProcessMessage(command);

                    response = HandleSpecialResponse(response);

                    StreamWriter swResponse = new StreamWriter("c:\\twofishResponse.xml");
                    swResponse.Write(response.InnerXml);
                    swResponse.Close();

                    DisplayResponse(response);
                }
            }
            catch(Exception ex)
            {
                XmlDocument errorResponse = CreateErrorDoc(ex.Message);
                DisplayResponse(errorResponse);

            }

        }

        private Hangout.Server.WebServices.CommandParser GetCommandParser(string noun)
        {
            Hangout.Server.WebServices.CommandParser parser = null;

            Hangout.Server.WebServices.HangoutCommandBase hangoutParser = new Hangout.Server.WebServices.HangoutCommandBase("Form1");
            if (hangoutParser.GetCommandClassType(noun) != null)
            {
                parser = (Hangout.Server.WebServices.CommandParser)hangoutParser;
            }
            else
            {
                parser = new TwoFishCommandBase ("HangoutCommand");
            }
            return parser;
       }

        private PaymentCommand  CreateCommand(string commandName, string userId, Type cmdType, XmlDocument arg2)
        {
            PaymentCommand paymentCommand = null;

            try
            {
                string command = commandName.Replace(" ", "");

                MethodInfo info = cmdType.GetMethod(command);

                Object invokeParam1 = Activator.CreateInstance(cmdType);

                int numArgs = info.GetParameters().Length;

                object[] args = new object[numArgs];
                args[0] = userId;

                if (numArgs > 1)
                {
                    args[1] = this;
                }
                if (numArgs > 2)
                {
                    args[2] = arg2;
                }

                paymentCommand = (PaymentCommand)info.Invoke(invokeParam1, args);
            }

            catch { }

            return (paymentCommand);

        }


        private PaymentCommand HandleSpecialCommand(PaymentCommand command, string userID, Type cmdType)
        {
            PaymentCommand commandSecond = null;
            Dictionary<string, string > specialCommands = new Dictionary<string, string>();

            specialCommands.Add("UpdateItem", "Items");
            specialCommands.Add("PurchaseCreditCard", "Purchase");
            specialCommands.Add("PurchaseCreditCardRecurring", "Purchase");
            specialCommands.Add("PurchaseCreditCardOneClick", "Purchase");

            string value = SpecialCommand(command, specialCommands);
            XmlDocument commandFirstResponse = null;

            switch (value)
            {
                case "UpdateItem":
                    commandFirstResponse = SpecialCreateCommand("FindItem", command, userID);
                    commandSecond = CreateCommand("UpdateItem", userID, cmdType, commandFirstResponse);
                    break;

                case "PurchaseCreditCard":
                    commandFirstResponse = SpecialCreateCommand("SecureKey", command, userID);
                    commandSecond = CreateCommand("CreditCardPurchase", userID, cmdType, commandFirstResponse);
                    break;

                case "PurchaseCreditCardRecurring":
                    commandFirstResponse = SpecialCreateCommand("SecureKey", command, userID);
                    commandSecond = CreateCommand("CreditCardPurchaseRecurring", userID, cmdType, commandFirstResponse);
                    break;

                case "PurchaseCreditCardOneClick":
                    commandFirstResponse = SpecialCreateCommand("SecureKey", command, userID);
                    commandSecond = CreateCommand("CreditCardPurchaseOneClick", userID, cmdType, commandFirstResponse);
                    break;

                default:
                    break;
            }


            return commandSecond;
        }

        private string SpecialCommand(PaymentCommand command,  Dictionary<string, string > specialCommands)
        {
            string specialName = "";

            foreach (KeyValuePair<string, string> kvp in specialCommands)
            {
                string noun = kvp.Value;
                string verb = kvp.Key;

                if ((command.Noun == noun) && (command.Verb == verb))
                {
                    specialName = verb;
                    break;
                }
            }
            return specialName;
        }

        private XmlDocument SpecialCreateCommand(string commandName, PaymentCommand command, string userID)
        {
            XmlDocument response = null;

            CreateTwoFishCommand cmd = new CreateTwoFishCommand();
            Type cmdType = cmd.GetType();
            PaymentCommand firstCommand = CreateCommand(commandName, userID, cmdType, null);
            PaymentItem paymentItem = new PaymentItem();
            response = paymentItem.ProcessMessage(firstCommand);

            return  response; 
        }

        private XmlDocument HandleSpecialResponse(XmlDocument response)
        {
            Dictionary<string, string> specialResponses = new Dictionary<string, string>();

            specialResponses.Add("PayPalCheckout", "Purchase");
            specialResponses.Add("PayPalRecurringCheckout", "Purchase");
            specialResponses.Add("PurchaseGameCurrencyPayPal", "HangoutPurchase");
            specialResponses.Add("GetUploadCatalogStoreFile", "Upload");
            specialResponses.Add("FindCatalog", "Catalog");
            specialResponses.Add("StoreBulkGet", "Store");

            string noun = response.SelectSingleNode("/Response").Attributes["noun"].InnerText;
            string verb = response.SelectSingleNode("/Response").Attributes["verb"].InnerText;

            string value = SpecialResponse(noun, verb, specialResponses);

            switch (value)
            {
                case "PayPalCheckout":
                case "PurchaseGameCurrencyPayPal":
                case "PayPalRecurringCheckout":
                    string payPalURL = response.SelectSingleNode("/Response/paypalURL").InnerText;
                    System.Diagnostics.Process.Start(payPalURL);
                    break;

                case "GetUploadCatalogStoreFile":
                case "FindCatalog":
                case "StoreBulkGet":
                    XmlNodeList responseNodeList = response.SelectSingleNode("/Response").ChildNodes;
                    foreach (XmlNode responseNode in responseNodeList)
                    {
                        WriteXmlTestData(String.Format("c:\\twofish{0}.txt", responseNode.Name), responseNode.InnerText);
                    }
                    break;

                default:
                    break;

            }

            return response;
        }

        private string SpecialResponse(string commandNoun, string commandVerb, Dictionary<string, string> specialResponses)
        {
            string specialName = "";

            foreach (KeyValuePair<string, string> kvp in specialResponses)
            {
                string noun = kvp.Value;
                string verb = kvp.Key;

                if ((commandNoun == noun) && (commandVerb == verb))
                {
                    specialName = verb;
                    break;
                }
            }
            return specialName;
        }

        private void WriteXmlTestData(string fileName, string data)
        {
            System.IO.StreamWriter swCommand = new System.IO.StreamWriter(fileName);

            swCommand.Write(data);
            swCommand.Close();
        }

        private void DisplayResponse(XmlDocument response)
        {
            try
            {
                // SECTION 2. Initialize the TreeView control.
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(new TreeNode(response.DocumentElement.Name));
                TreeNode tNode = new TreeNode();
                tNode = treeView1.Nodes[0];

                // SECTION 3. Populate the TreeView with the DOM nodes.
                AddNode(response.DocumentElement, tNode);
                treeView1.ExpandAll();
            }
            catch (XmlException xmlEx)
            {
                MessageBox.Show(xmlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void AddNode(XmlNode inXmlNode, TreeNode inTreeNode)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList nodeList;
            int i;

            // Loop through the XML nodes until the leaf is reached.
            // Add the nodes to the TreeView during the looping process.
            if (inXmlNode.NodeType == XmlNodeType.Element)
            {
                foreach (XmlAttribute attr in inXmlNode.Attributes)
                {
                    inTreeNode.Text += " " + attr.Name + "=\"" + attr.Value + "\"";
                }
            }

            if (inXmlNode.HasChildNodes)
            {
                nodeList = inXmlNode.ChildNodes;
                for (i = 0; i <= nodeList.Count - 1; i++)
                {
                    xNode = inXmlNode.ChildNodes[i];
                    inTreeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = inTreeNode.Nodes[i];
                    AddNode(xNode, tNode);
                }
            }
            else
            {
                // Here you need to pull the data from the XmlNode based on the
                // type of node, whether attribute values are required, and so forth.
                string[] textArray = (inXmlNode.OuterXml).Trim().Split('\n');
                if (textArray.Length > 1)
                {
                    foreach (string textString in textArray)
                    {
                        inTreeNode.Nodes.Add(textString);
                    }
                }
                else
                {
                    inTreeNode.Text = (inXmlNode.OuterXml).Trim();
                }
            }
        }

        private void ClearBackColor()
        {
            TreeNodeCollection nodes = treeView1.Nodes;
            foreach (TreeNode node in nodes)
            {
                ClearRecursive(node);
            }
        }

        // called by ClearBackColor function 
        private void ClearRecursive(TreeNode treeNode)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.BackColor = Color.White;
                ClearRecursive(node);
            }
        } 

        private string PrettyDisplay(string item)
        {
            StringBuilder output = new StringBuilder();

            foreach (Char c in item)
            {
                if (Char.IsUpper(c))
                {
                    output.Append(' ');
                }
                output.Append(c);
            }

            return output.ToString().Trim();
        }

        private string  FindCurrenyText(string searchText)
        {
            string currencyName = "";
            TreeNodeCollection nodes = treeView1.Nodes;

            foreach (TreeNode node in nodes)
            {
                FindRecursive(node, searchText, out currencyName);
            }

            return currencyName;
         }

        private void FindRecursive(TreeNode treeNode, string searchText, out string currencyName)
        {
            currencyName = "";
            foreach (TreeNode node in treeNode.Nodes)
            {
                if (node.Text.StartsWith(searchText))
                {
                    string data = node.Text.Replace("currencyName=", "*;*currencyName=");
                    data = data.Replace("currencyId=", "*;* currencyId=");
              
                    string[] stringSeparators = new string[] {"*;*"};
                    string[] itemArray = data.Split(stringSeparators, StringSplitOptions.None);

                    string[] dataArray = itemArray[1].Split('=');
                    currencyName = dataArray[1].Replace("\"", "").Trim();
                    break;
                }
                FindRecursive(node, searchText, out currencyName);
            }
        } 

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Clipboard.SetText(e.Node.Text.Replace("response request=", "").Replace("\"", ""));
            MessageBox.Show("Node Text Copyied to Clipboard");
            ClearBackColor();
            e.Node.BackColor = Color.Yellow;
        }

        private XmlDocument GetSelectedXml()
        {
            XmlDocument selectedItem = null;

            try
            {
                if (Clipboard.ContainsText())
                {
                    selectedItem = new XmlDocument();
                    
                    string data = Clipboard.GetText();
                    if (data.StartsWith("itemOffer"))
                    {
                        int iPos = data.IndexOf("itemOffer");
                        data = data.Remove(iPos, 9);
                        data = data.Replace("endDate=", "*;*endDate=");
                        data = data.Replace("numAvailable=", "*;*numAvailable=");
                        data = data.Replace("startDate=", "*;*startDate=");
                        data = data.Replace("title=", "*;*title=");
                        data = data.Replace("id=", "*;*id=");
                        data = data.Replace("special=", "*;*special=");
                        data = data.Replace("specialType=", "*;*specialType=");
                        data = data.Replace("type=", "*;*type=");

                        string[] stringSeparators = new string[] {"*;*"};
                        string[] itemArray = data.Split(stringSeparators, StringSplitOptions.None);

                        string currenyName = FindCurrenyText("store");

                        selectedItem.LoadXml(String.Format("<Response><currency name='{0}'></currency><itemOffer></itemOffer></Response>", currenyName));
                        AddItemAttributes(selectedItem, itemArray);
                    }
                }
            }
            catch { }

            return selectedItem;
        }

        private void AddItemAttributes (XmlDocument xmlDoc, string[] attributes)
        {
            foreach (string item in attributes)
            {
                if (item.Trim().Length > 0)
                {
                    string[] kvp = item.Split('=');
                    XmlNode xmlDocNode = xmlDoc.SelectSingleNode("/Response/itemOffer");
                    XmlAttribute attribute = xmlDoc.CreateAttribute(kvp[0].Trim());
                    attribute.Value = kvp[1].Trim();
                    xmlDocNode.Attributes.Append(attribute);
                }
           }
        }

        /// <summary>
        /// Creates and error XML document response document from a message.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <returns>Error XML document</returns>
        protected XmlDocument CreateErrorDoc(string message)
        {
            XmlDocument response = new XmlDocument();
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("<response><Error><Message>" + message + "</Message></Error></response>");
                response.LoadXml(sb.ToString());
            }
            catch
            {
                response.LoadXml("<response><Error>unknown</Error></response>");
            }

            return response;
        }
    }
}
