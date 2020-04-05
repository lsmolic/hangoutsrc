using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Hangout.Shared;

namespace ServerAdminTool
{
    public partial class Form1 : Form
    {
        readonly AdminClientMessageProcessor mAdminClientProcessor = null;
        public delegate void UpdateTreeDataDelegate(string xmlString);

        public Form1()
        {
            mAdminClientProcessor = new AdminClientMessageProcessor();
            mAdminClientProcessor.SetUpdateTreeCallback(UpdateTreeDataThreadSafe);

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Prepopulate serverComboBox1 with IPs. 
            // TODO:  This should eventually come from the boss server service
            serverComboBox1.Items.Add("127.0.0.1");
            serverComboBox1.Items.Add("64.106.173.25"); // Dev025

            serverComboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mAdminClientProcessor.RequestUpdate();
        }


        public void TestXmlToTree()
        {
            UpdateTreeData("<Zones><Zone name='samir' date = 'ping'><ZoneId id='blah'>1</ZoneId><DistributedObject><DoId>20</DoId></DistributedObject></Zone><Zone><ZoneId>2</ZoneId></Zone></Zones>");
        }


        private void UpdateTreeDataThreadSafe(string xmlString)
        {
            treeView1.Invoke(new UpdateTreeDataDelegate(UpdateTreeData), new object[] { xmlString });
        }

        private void UpdateTreeData(string xmlString)
        {
            try
            {  

                // Create XML Doc
                XmlDocument dom = new XmlDocument();
                dom.LoadXml(xmlString);

                // Initialize the TreeView control
                treeView1.Nodes.Clear();
                //treeView1.Nodes.Add(new TreeNode(dom.DocumentElement.Name));
                treeView1.Nodes.Add(new TreeNode("Server Data"));
                TreeNode tNode = new TreeNode();
                tNode = treeView1.Nodes[0];

                // Populate TreeView with the DOM nodes
                AddNode(dom.DocumentElement, tNode);
                treeView1.ExpandAll();
            }
            catch (XmlException xmlException)
            {
                MessageBox.Show(xmlException.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
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
            if (inXmlNode.HasChildNodes)
            {
                nodeList = inXmlNode.ChildNodes;
                for (i = 0; i <= nodeList.Count - 1; i++)
                {
                    xNode = inXmlNode.ChildNodes[i];
                    TreeNode ntNode = new TreeNode(xNode.Name);
                    inTreeNode.Nodes.Add(ntNode);

                    XmlAttributeCollection attribs = xNode.Attributes;
                    if (attribs != null)
                    {
                        ntNode.Text += ":   ";
                        for (int j = 0; j < attribs.Count; j++)
                        {
                            ntNode.Text += (attribs[j].Name + " = " + attribs[j].InnerText);
                            if (j<attribs.Count-1)
                            {
                                ntNode.Text += ",  ";
                            }
                        }
                    }

                    tNode = inTreeNode.Nodes[i];
                    AddNode(xNode, tNode);
                }
            }
            else
            {
                // Here you need to pull the data from the XmlNode based on the
                // type of node, whether attribute values are required, and so forth.
                inTreeNode.Text = (inXmlNode.OuterXml).Trim();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Connect to selected IP
            mAdminClientProcessor.Connect(serverComboBox1.SelectedItem.ToString());

            // Update tree
            mAdminClientProcessor.RequestUpdate();
        }
    }
}
