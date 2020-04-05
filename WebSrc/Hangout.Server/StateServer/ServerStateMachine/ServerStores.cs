using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Configuration;
using log4net;

using Hangout.Shared;

namespace Hangout.Server
{

    public class ServerStores
    {
		private Object lockObject = new Object();
        private static ILog mLogger = LogManager.GetLogger("ServerStores");

        private BackgroundWorker mBackgroundWorker = null;

        private Dictionary<string, XmlDocument> mStoreXml = new Dictionary<string, XmlDocument>();
        private Dictionary<string, DataSet> mStoreDataSet = new Dictionary<string, DataSet>();

        private Dictionary<string, string[]> mCombinedStores = new Dictionary<string, string[]>();

        private int mRefreshFullValue = -1;
        private int mRefreshIncrementalValue = -1;

        private AutoResetEvent mAutoTerminateEvent = new AutoResetEvent(false);

        private bool mDebug = false;
        private string mDebugDir = "";

        private readonly PaymentItemsManager mPaymentItemsManager;
        public PaymentItemsManager PaymentItemsManager
        {
            get { return mPaymentItemsManager; }
        }

        /// <summary>
        /// Initialize the Store Tables with GetStoreInventory PaymentItems cached store data
        /// </summary>
        public ServerStores(PaymentItemsManager paymentItemsManager)
        {
            mPaymentItemsManager = paymentItemsManager;

            GetStoreArrayFromConfigFile();
            UpdateStoreInfo();
        }

        /// <summary>
        /// Reads the ServerStores app config file node
        /// </summary>
        private void GetStoreArrayFromConfigFile()
        {
            try
            {
				//no need to lock here because this should only be called from the main thread when this object is instantiated
				StoreNamesSection storeConfig = (StoreNamesSection)ConfigurationManager.GetSection("ServerStores");

				StoreNamesCollection storeNames = storeConfig.StoreNames;

				foreach (StoreConfigElement storeElement in storeNames)
				{
					if (storeElement.Name != null && storeElement.Name.Length > 0)
					{
						AddStore(storeElement.Name);
					}
				}

				StoreNamesCollection combinedStore = storeConfig.CombinedStores;

				foreach (StoreConfigElement storeElement in combinedStore)
				{
					if (storeElement.Name != null && storeElement.Name.Length > 0)
					{
						string[] mergeStores = new string[] { storeElement.Store, storeElement.MergeStore };
						mLogger.Debug("GetStoreArrayFromConfigFile.AddCombinedStore(" + storeElement.Name + ")");
						AddCombinedStore(storeElement.Name, mergeStores);
					}
				}

				mRefreshFullValue = ConvertToInt(storeConfig.Refresh.Value, -1);
				mRefreshIncrementalValue = ConvertToInt(storeConfig.Refresh.Incremental, -1);

				mDebug = ConvertToBool(storeConfig.Refresh.debug, false);
				mDebugDir = storeConfig.Refresh.dir.Trim();

				if (String.IsNullOrEmpty(mDebugDir))
				{
					mDebug = false;
				}
            }

            catch (Exception ex)
            {
                mLogger.Error(String.Format("Error in GetStoreArrayFromConfigFile Error: {0} ", ex));
            }
        }

        /// <summary>
        /// Clean up
        /// </summary>
        ~ServerStores()
        {
            mAutoTerminateEvent.Set();
        }


        /// <summary>
        /// Add a store name to the list of stores
        /// </summary>
        /// <param name="storeName">Name of store to add</param>
        private void AddStore(string storeName)
        {
			lock (lockObject)
			{
				XmlDocument xmlStoreData = new XmlDocument();
				mStoreXml.Add(storeName, xmlStoreData);
			}
        }

        /// <summary>
        /// Add a store name to the list of stores
        /// </summary>
        /// <param name="storeName">Name of store to add</param>
        private void AddCombinedStore(string combinedStoreName, string[] storeNames)
        {
			lock (lockObject)
			{
				XmlDocument xmlStoreData = new XmlDocument();
				mStoreXml.Add(combinedStoreName, xmlStoreData);
				mCombinedStores.Add(combinedStoreName, storeNames);
			}
        }

        /// <summary>
        /// Get the cached XML Store response
        /// </summary>
        /// <param name="storeName">The name of the store to retreive the XML response</param>
        /// <returns>XML cached store response</returns>
        public XmlDocument GetStoreResponseXML(string storeName)
        {
            XmlDocument response = new XmlDocument();
            XmlDocument responseClone = new XmlDocument();

			lock (lockObject)
            {
                mStoreXml.TryGetValue(storeName, out response);
                responseClone.LoadXml(response.InnerXml);
            }
            return responseClone;
        }


        /// <summary>
        /// Get the cached stores dataset 
        /// </summary>
        /// <param name="storeName">The store name of the store to get the cached data for</param>
        /// <returns>The cached dataset else null</returns>
        public DataSet GetStoreResponseDataSet(string storeName)
        {
            DataSet storeDataSetData = null;
            DataSet storeDataSetClone = null;

			lock (lockObject)
            {
                if (mStoreDataSet.TryGetValue(storeName, out storeDataSetData))
                {
                    storeDataSetClone = storeDataSetData.Copy();
                }
            }
            return storeDataSetClone;
        }


        /// <summary>
        /// Checks if this is a Cached Store by checking if the storeName key 
        /// is contained in the StoreXml dictionary
        /// </summary>
        /// <param name="storeName">Find if this Store is cached </param>
        /// <returns>True if the store is cached else false</returns>
        public bool IsCachedStore(string storeName)
        {
            bool returnValue = false;
			lock (lockObject)
			{
				if (mStoreXml.ContainsKey(storeName))
				{
					returnValue = true;
				}
			}
            return returnValue;
        }

        /// <summary>
        /// Find and Item by ItemId in the Store and return the items row
        /// The method makes a copy of the store items table 
        /// adds the "itemInstance_Id" column (relationship to parent) to the item clone table
        /// adds the "assets" column to the table (used for the Assest XML)
        /// gets the row if it exists for the item.
        /// </summary>
        /// <param name="itemId">the itemId of the row to find in the items table</param>
        /// <returns>returns the item data row if the row is found or else returns null</returns>
        public DataRow FindItemInStore(string itemId)
        {
            DataRow itemRow = null;
			lock (lockObject)
			{
				foreach (KeyValuePair<string, DataSet> kvp in mStoreDataSet)
				{
					DataSet storeDataSet = (DataSet)kvp.Value;
					DataTable itemTableOrg = storeDataSet.Tables["item"];

					DataTable itemTable = itemTableOrg.Copy();
					DataColumn itemInstanceId = new DataColumn("itemInstance_Id", System.Type.GetType("System.Int32"));
					itemInstanceId.ColumnMapping = MappingType.Hidden;
					itemTable.Columns.Add(itemInstanceId);

					DataView itemTableView = new DataView(itemTable);
					itemTableView.Sort = "id asc";

					int rowIndex = itemTableView.Find(itemId);
					if (rowIndex != -1)
					{
						DataRowView itemRowView = itemTableView[rowIndex];
						itemRow = itemRowView.Row;
						break;
					}
				}
			}
            return itemRow;
        }

        /// <summary>
        /// Background worker thread to update the GetStoreInventory PaymentItems cached store data
        /// </summary>
        private void UpdateStoreInfo()
        {
            mBackgroundWorker = new BackgroundWorker();
            mBackgroundWorker.WorkerSupportsCancellation = false;
            mBackgroundWorker.DoWork += new DoWorkEventHandler(bwMainDoWork);
            mBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwMainRunWorkerCompleted);
            mBackgroundWorker.RunWorkerAsync(mStoreXml);
        }


        /// <summary>
        /// Background worker thread that does the GetStoreInventory PaymentItems call
        /// Waits 15 minutes and then refreshed the store data
        /// </summary>
        /// <param name="sender">sender not used</param>
        /// <param name="e">copy of storeData arguments</param>
        private void bwMainDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (true)
                {
					BackgroundMain(e);
					//no need to lock here.. mRefreshFullValue should only be set from the main thread as this object gets instantiated
					if (mRefreshFullValue == -1)
					{
						break;
					}

					TimeSpan timeSpan = new TimeSpan(0, mRefreshFullValue, 0);
					int eventIndex = WaitHandle.WaitAny(new WaitHandle[] { mAutoTerminateEvent }, timeSpan, false);
					if (eventIndex != WaitHandle.WaitTimeout)
					{
						break;
					}
                }
            }

            catch (Exception ex)
            {
                mLogger.Error(String.Format("Error in bwMainDoWork {1} ", ex));
            }
        }


        /// <summary>
        /// Background Main thread that does the GetStoreInventory PaymentItems call
        /// </summary>
        /// <param name="e">copy of storeData arguments</param>
        private void BackgroundMain(DoWorkEventArgs e)
        {
			string storeName = "";
            try
            {
				//we don't need to lock around mCombinedStores because it only gets set up from the main thread when this object is instantiated
				Dictionary<string, XmlDocument> storeDataCopy = (Dictionary<string, XmlDocument>)e.Argument;

				foreach (KeyValuePair<string, XmlDocument> kvp in storeDataCopy)
				{
					storeName = kvp.Key;
					if (!mCombinedStores.ContainsKey(storeName))
					{
						GetAndUpdateTheStoreInventory(storeName);
					}
				}

				foreach (KeyValuePair<string, string[]> kvp in mCombinedStores)
				{
					MergeTwoStores(kvp.Key, kvp.Value[0], kvp.Value[1]);
				}
            }

            catch (Exception ex)
            {
                mLogger.Error(String.Format("Error in BackgroundMain for Store {0} Error: {1} ", storeName, ex));
            }
        }


        /// <summary>
        /// Get the store PaymentItems response and update the store data with a new response
        /// </summary>
        /// <param name="storeName">The name of the store</param>
        private void GetAndUpdateTheStoreInventory(string storeName)
        {
            try
            {
                PaymentItemsProcess paymentItemProcess = new PaymentItemsProcess();
                ServiceCommandSerializer serializer = new ServiceCommandSerializer();

                PaymentCommand paymentItemCommand = new PaymentCommand();
                paymentItemCommand.Verb = "GetStoreInventory";

                // Add stage name to storeName in request.   Don't store the stage name in variable, so the code doesn't have to care about stage.
                string stage = System.Configuration.ConfigurationSettings.AppSettings["Stage"];
                paymentItemCommand.Parameters.Add("storeName", storeName + "_" + stage);
                string xmlPaymentItemsMessage = serializer.SerializeCommandData(paymentItemCommand, typeof(PaymentCommand));
                string response = paymentItemProcess.ProcessMessageBlocking(xmlPaymentItemsMessage, null, null);

                XmlDocument xmlResponse = new XmlDocument();
                xmlResponse.LoadXml(response);

                xmlResponse = RemoveItemTypes(xmlResponse);
                xmlResponse = AddAssetsToStoreInventoryItems(xmlResponse, storeName);

                CopyResponseToStoreData(xmlResponse, storeName);
            }

            catch (Exception ex)
            {
                mLogger.Error(String.Format("Error in GetAndUpdateTheStoreInventory for Store {0} Error: {1} ", storeName, ex));
            }
        }


        /// <summary>
        /// Update the store data with a new response
        /// </summary>
        /// <param name="response">xml string store response received from the PaymentItems</param>
        /// <param name="key">The key is the store name to copy</param>
        private void CopyResponseToStoreData(XmlDocument xmlDoc, string key)
        {
            string response = xmlDoc.InnerXml;

            try
            {
                //  MemoryStream schema = LoadSchemaFile("StoreImportSchemaFull.xsd");
                ServerPaymentItemsDataSets dataSets = new ServerPaymentItemsDataSets();
                DataSet dsStoreCreated = dataSets.CreateStoreDataSet("StoreDataSet", true);

                XmlNode itemOffersNode = xmlDoc.SelectSingleNode("/Response/store");

                XmlNodeList itemOffersNodeList = xmlDoc.SelectNodes("/Response/store/itemOffers/itemOffer");

				lock (lockObject)
				{
					if (mDebug)
					{
						xmlDoc.Save(String.Format(@"{0}CopyResponseToStoreXml_{1}.xml", mDebugDir, key));
					}

                    mStoreXml[key].InnerXml = xmlDoc.InnerXml;
                    //DataSet dsStore = LoadXMLInToDataSet(itemOffersNode, schema, "StoreDataSet");
                    DataSet dsStore = dataSets.LoadStoreDataXMLInToDataSet(itemOffersNodeList, dsStoreCreated, true);

                    if (mDebug)
                    {
                        dsStore.WriteXml(String.Format(@"{0}CopyResponseToStoreData_{1}.xml", mDebugDir, key));
                    }

                    mStoreDataSet[key] = dsStore;
                }
            }

            catch (Exception ex)
            {
                mLogger.Error(String.Format("Error in CopyResponseToStoreData for Store {0} Error: {1}\n Response: {2}", key, ex, response));
            }
        }

        /// <summary>
        /// Add the item Assets To StoreInventory Items node
        /// </summary>
        /// <param name="response">xml string store response received from the PaymentItems</param>
        /// <param name="storeName">The name of the store used for debugging.</param>
        /// <returns>The reesponse xml document with the items assets added</returns>
        private XmlDocument AddAssetsToStoreInventoryItems(XmlDocument response, string storeName)
        {

            XmlDocument storeResponse = new XmlDocument();
            storeResponse.InnerXml = response.InnerXml;

            try
            {
                if (response == null)
                {
                    throw new Exception("Error: Cached Response for PaymentItems User Inventory Response is not available");
                }
                else
                {
                    XmlNode itemOffersNode = response.SelectSingleNode("/Response/store");

                    XmlNodeList itemOffersNodeList = response.SelectNodes("/Response/store/itemOffers/itemOffer");

                    ServerPaymentItemsDataSets dataSets = new ServerPaymentItemsDataSets();
                    DataSet dsStoreCreated = dataSets.CreateStoreDataSet("StoreDataSet", false);
                    DataSet dsStore = dataSets.LoadStoreDataXMLInToDataSet(itemOffersNodeList, dsStoreCreated, false);

                    //MemoryStream schema = LoadSchemaFile("StoreImportSchema.xsd");
                    //DataSet dsStore = LoadXMLInToDataSet(itemOffersNode, schema, "StoreDataSet");

                    if (mDebug)
                    {
                        dsStore.WriteXml(String.Format(@"{0}AddAssetsToStoreInventoryItems_{1}.xml", mDebugDir, storeName));
                    }

                    DataTable dtOffer = dsStore.Tables["itemOffer"];
                    DataTable dtItem = dsStore.Tables["item"];

                    DataColumn itemAssets = new DataColumn("Assets", System.Type.GetType("System.String"));
                    itemAssets.ColumnMapping = MappingType.Element;
                    dtItem.Columns.Add(itemAssets);

                    foreach (DataRow offerRow in dtOffer.Rows)
                    {
                        DataRow[] itemRowArray = offerRow.GetChildRows("itemOffer_item");
                        DataRow itemRow = itemRowArray[0];

                        string hangoutItemId = itemRow["name"].ToString();
                        itemRow["Assets"] = String.Format("<![CDATA[{0}]]>", mPaymentItemsManager.GetAssetDataSetDna(hangoutItemId));
                    }

                    StringWriter sw = new StringWriter();
                    dsStore.WriteXml(sw);

                    XmlDocument responseTemp = new XmlDocument();
                    responseTemp.LoadXml(UnEscapeXml(sw.ToString()));

                    XmlNode storeItemOffersNode = storeResponse.SelectSingleNode("/Response/store/itemOffers");

                    storeItemOffersNode.RemoveAll();

                    XmlNodeList newOfferList = responseTemp.SelectNodes("/StoreDataSet/itemOffer");

                    foreach (XmlNode newOffer in newOfferList)
                    {
                        XmlNode newChildNode = storeResponse.ImportNode(newOffer, true);
                        storeItemOffersNode.AppendChild(newChildNode);
                    }

                    //storeResponse.LoadXml(responseTemp.SelectSingleNode("/StoreDataSet").InnerXml);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(String.Format("Error in AddAssestsToStoreInventoryItems Error: {0}\n Response: {1}", ex, response));
            }

            return storeResponse;
        }

        /// <summary>
        /// Removes the Item Types node from the store response
        /// </summary>
        /// <param name="response">The store xml response </param>
        /// <returns>The response with the Item Types node removed</returns>
        private XmlDocument RemoveItemTypes(XmlDocument response)
        {
            try
            {
                XmlNode nodeStore = response.SelectSingleNode("/Response/store");
                XmlNode nodeItemTypes = response.SelectSingleNode("/Response/store/itemTypes");

                if((nodeStore != null) && (nodeItemTypes != null))
                {
                    nodeStore.RemoveChild(nodeItemTypes);
                }
            }

            catch (Exception ex)
            {
                mLogger.Error(String.Format("Error in RemoveItemTypes Error: {0}\n Response: {1}", ex, response));
            }

            return response;
		}

       


		/// <summary>
        /// Background worker completed
        /// </summary>
        /// <param name="sender">sender not used</param>
        /// <param name="e">ServiceCommandAsync parameters containing the results and information required for the callback</param>
        private void bwMainRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        /// <summary>
        /// Merge two PaymentItems Stores into a one store
        /// </summary>
        /// <param name="storeName">The Merged Store name</param>
        /// <param name="storesToCombine1">Store 1 to merge</param>
        /// <param name="storesToCombine2">Store 2 to merge</param>
        private void MergeTwoStores(string storeName, string storesToCombine1, string storesToCombine2)
        {
            XmlDocument response1 = new XmlDocument();
            XmlDocument response2 = new XmlDocument();

            XmlDocument responseClone = null;

            try
            {
				lock (lockObject)
                {
                    mStoreXml.TryGetValue(storesToCombine1, out response1);
                    mStoreXml.TryGetValue(storesToCombine2, out response2);

                    int count1 = response1.SelectNodes("/Response/store/itemOffers/itemOffer").Count;
                    int count2 = response2.SelectNodes("/Response/store/itemOffers/itemOffer").Count;

                    if (count1 > count2)
                    {
                        responseClone = MergeItemOfferNodes(storeName, response1, response2);
                    }
                    else
                    {
                        responseClone = MergeItemOfferNodes(storeName, response2, response1);
                    }

                    CopyResponseToStoreData(responseClone, storeName);
                }
            }

            catch (Exception ex)
            {
                mLogger.Error(String.Format("Error in MergeTwoStores for Store {0} Error: {1} ", storeName, ex));
            }
        }


        /// <summary>
        /// Merge the Item Offer nodes
        /// </summary>
        /// <param name="storeName">name of the store</param>
        /// <param name="destinationXml">destination xml document</param>
        /// <param name="mergeXml">The document to merge the itemOffer nodes into destination xml document</param>
        /// <returns>destination xml document with the merged nodes</returns>
        private XmlDocument MergeItemOfferNodes(string storeName, XmlDocument destinationXml, XmlDocument mergeXml)
        {
            XmlDocument destinationXmlClone = new XmlDocument();

            destinationXmlClone.InnerXml = destinationXml.InnerXml;

            XmlNode nodeStore = destinationXmlClone.SelectSingleNode("/Response/store");
            nodeStore.Attributes.RemoveAll();
            AddAttribute(destinationXmlClone, nodeStore, "name", storeName);

            XmlNode node1 = destinationXmlClone.SelectSingleNode("/Response/store/itemOffers");

            XmlNodeList nodeList2 = mergeXml.SelectNodes("/Response/store/itemOffers/itemOffer");
            foreach (XmlNode node2 in nodeList2)
            {
                XmlNode newChildNode = destinationXmlClone.ImportNode(node2, true);
                node1.AppendChild(newChildNode);
            }

            return destinationXmlClone;
        }


        /// <summary>
        ///  Load xml node into a releational dataset
        /// </summary>
        /// <param name="xmlNode">xmlNode to load into the dataset</param>
        /// <param name="xmlSchema">xmlschema used to parse the xml data</param>
        /// <param name="dataSetName">The Name of the dataset to create</param>
        /// <returns>A releational dataset containing the xml document</returns>
        public DataSet LoadXMLInToDataSet(XmlNode xmlNode, MemoryStream xmlSchema, string dataSetName)
        {
            DataSet dsXmlData = new DataSet(dataSetName);

            try
            {
                XmlReader xmlReader = new XmlNodeReader(xmlNode);

                xmlSchema.Position = 0;
                dsXmlData.ReadXmlSchema(xmlSchema);
                dsXmlData.ReadXml(xmlReader);

                dsXmlData.DataSetName = dataSetName;
            }

            catch (Exception ex)
            {
                mLogger.Error("LoadXMLInToDataSet", ex);
            }

            return dsXmlData;
        }


        /// <summary>
        ///  Load xml node into a releational dataset
        /// </summary>
        /// <param name="xmlNode">xmlNode to load into the dataset</param>
        /// <param name="dsXmlData">The dataset to load the xml data into</param>
        /// <returns>The dataset with the xml data loaded</returns>
        public DataSet LoadXMLInToDataSet(XmlNode xmlNode, DataSet dsXmlData)
        {
            try
            {
                XmlReader xmlReader = new XmlNodeReader(xmlNode);
                dsXmlData.ReadXml(xmlReader);
            }

            catch (Exception ex)
            {
                mLogger.Error("LoadXMLInToDataSet", ex);
            }

            return dsXmlData;
        }


        /// <summary>
        /// Loads the schema file used to parse the PaymentItems response
        /// </summary>
        /// <param name="fileName">file name and path of the schema file to load</param>
        /// <returns>MemoryStream containg the schema file</returns>
        public MemoryStream LoadSchemaFile(string fileName)
        {
            FileStream file = File.OpenRead(fileName);
            MemoryStream schema = new MemoryStream();

            schema.SetLength(file.Length);
            file.Read(schema.GetBuffer(), 0, (int)file.Length);

            schema.Flush();
            file.Close();

            return schema;
        }


        /// <summary>
        /// Add Attribute to the XMLDocument
        /// </summary>
        /// <param name="xmlDoc">XMLDocument to add the atttibute to</param>
        /// <param name="xPath">XPath of the node to add the attribute to.</param>
        /// <param name="attributeName">The attribute name</param>
        /// <param name="attributeValue">The attribut value</param>
        private void AddAttribute(XmlDocument xmlDoc, XmlNode xmlNode, string attributeName, string attributeValue)
        {
            XmlAttribute attribute = xmlDoc.CreateAttribute(attributeName);
            attribute.Value = attributeValue;
            xmlNode.Attributes.Append(attribute);
        }


        /// <summary>
        /// Convert string value to integer, if the passed in value is not a valid integer than return the default value
        /// </summary>
        /// <param name="value">string value to convert to an integer</param>
        /// <param name="defaultValue">default integer value</param>
        /// <returns>string value converted to integer, if the passed in value is not a valid integer than return the default value</returns>
        public int ConvertToInt(string value, int defaultValue)
        {
            int returnValue = defaultValue;

            int tempValue = -1;
            if (int.TryParse(value, out tempValue))
            {
                returnValue = tempValue;
            }

            return returnValue;
        }

        /// <summary>
        /// Convert string value to bool, if the passed in value is not a valid bool than return the default value
        /// </summary>
        /// <param name="value">string value to convert to an bool</param>
        /// <param name="defaultValue">default integer value</param>
        /// <returns>string value converted to bool, if the passed in value is not a valid bool than return the default value</returns>
        private bool ConvertToBool(string value, bool defaultValue)
        {
            bool returnValue = defaultValue;

            bool tempValue = false;
            if (bool.TryParse(value, out tempValue))
            {
                returnValue = tempValue;
            }

            return returnValue;
        }

        /// <summary>
        /// Unescape the XML characters for '&lt;' and '&gt;'
        /// </summary>
        /// <param name="xmlString">The input xml string</param>
        /// <returns>The unescaped xml string replacing '&lt;' and '&gt;'</returns>
        private string UnEscapeXml(string xmlString)
        {
            string xmlReturnString = "";

            if (!string.IsNullOrEmpty(xmlString))
            {
                xmlReturnString = xmlString.Replace("&lt;", "<");
                xmlReturnString = xmlReturnString.Replace("&gt;", ">");
            }

            return xmlReturnString;
        }
    }


    /// <summary>
    /// Define the ServerStores custom section containing a collection of store elements
    /// and a collection combined store elements
    /// </summary>
    public class StoreNamesSection : ConfigurationSection
    {
        [ConfigurationProperty("refreshTime")]
        public RefreshConfigElement Refresh
        {
            get { return ((RefreshConfigElement)base["refreshTime"]); }
        }

        /// <summary>
        /// The collection of the stores
        /// </summary>
        [ConfigurationProperty("stores", IsDefaultCollection = false)]
        public StoreNamesCollection StoreNames
        {
            get { return (StoreNamesCollection)base["stores"]; }
        }

        /// <summary>
        /// The collections of combined stores
        /// </summary>
        [ConfigurationProperty("combined", IsDefaultCollection = false)]
        public StoreNamesCollection CombinedStores
        {
            get { return (StoreNamesCollection)base["combined"]; }
        }
    }

    /// <summary>
    /// The Refresh Configuration Element
    /// </summary>
    public class RefreshConfigElement : ConfigurationElement
    {
        /// <summary>
        /// The full configuration property specifies the full store refresh time in minutes
        /// </summary>
        [ConfigurationProperty("full", IsRequired = true)]
        public string Value
        {
            get { return (string)this["full"]; }
            set { this["full"] = value; }
        }

        /// <summary>
        /// The incremental configuration property specifies the incremental store refresh time in minutes
        /// </summary>
        [ConfigurationProperty("incremental")]
        public string Incremental
        {
            get { return (string)this["incremental"]; }
            set { this["incremental"] = value; }
        }

        /// <summary>
        /// The debug configuration property specifies the debug mode
        /// </summary>
        [ConfigurationProperty("debug")]
        public string debug
        {
            get { return (string)this["debug"]; }
            set { this["debug"] = value; }
        }

        /// <summary>
        /// The debug directory configuration property 
        /// </summary>
        [ConfigurationProperty("dir")]
        public string dir
        {
            get { return (string)this["dir"]; }
            set { this["dir"] = value; }
        }
    }
    
    /// <summary>
    /// The Store Configuration Element 
    /// </summary>
    public class StoreConfigElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor for the Store configuration element
        /// </summary>
        public StoreConfigElement()
        {
        }

        /// <summary>
        /// Constructor for the Store configuration element defining the store name
        /// used for reading the stores collection
        /// </summary>
        /// <param name="elementName">The name of the store</param>
        public StoreConfigElement(string elementName)
        {
            Name = elementName;
        }

        /// <summary>
        /// Constructor for the Store configuration element defining the store name
        /// newStore name and mergeStore name used for reading the combined store collection
        /// </summary>
        /// <param name="elementName">The name of the store</param>
        /// <param name="mergeStore1">The first store to merge</param>
        /// <param name="mergeStore2">The second store to merge</param>
        public StoreConfigElement(String elementName, string mergeStore1, string mergeStore2)
        {
            Name = elementName;
            Store = mergeStore1;
            MergeStore = mergeStore2;
        }

        /// <summary>
        /// The Store Name property
        /// </summary>
        [ConfigurationProperty("name", IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value;  }
        }

        /// <summary>
        /// The Merge Store 1 property
        /// </summary>
        [ConfigurationProperty("mergeStore1", IsRequired = false)]
        public string Store
        {
            get { return (string)this["mergeStore1"]; }
            set { this["mergeStore1"] = value; }
        }

        /// <summary>
        /// The Merge Store 2 property
        /// </summary>
        [ConfigurationProperty("mergeStore2", IsRequired = false)]
        public string MergeStore
        {
            get { return (string)this["mergeStore2"];}
            set { this["mergeStore2"] = value; }
        }
    }


    /// <summary>
    /// The Store Names Collection 
    /// </summary>
    public class StoreNamesCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Override of create new element
        /// </summary>
        /// <returns>The new StoreConfigElement</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new StoreConfigElement();
        }
       
        /// <summary>
        /// Override of GetElementKey
        /// </summary>
        /// <param name="element">The Element name to get from the config file</param>
        /// <returns>Returns the StoreConfigElement retrieved from the config file</returns>
        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((StoreConfigElement)element).Name;
        }
    }
}


     
