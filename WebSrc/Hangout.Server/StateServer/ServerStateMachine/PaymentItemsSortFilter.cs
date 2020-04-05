using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;
using Hangout.Shared;
using log4net;

namespace Hangout.Server
{
    public class PaymentItemsSortFilter
    {
        private static ILog mLogger = LogManager.GetLogger("PaymentItemsSortFilter");

        public PaymentItemsSortFilter()
        {
        }

        /// <summary>
        /// Filter and Sort the Store Response PaymentItems Response.
        /// The Filter is similiar to the where clause in a SQL Statement. ("AND and "OR" can be used");
        /// All nodes are refered to by the NodeName_Attribute (TableName_Column) to filter on.
        /// For example Filter ==> itemOffer_title Like '%basic%'  Sort ==> itemOffer_endDate desc, item_itemTypeName asc
        /// </summary>
        /// <param name="response">The returned PaymentItems response</param>
        /// <param name="filter">The Filter string to use in a form similiar to where clause in a SQL statement</param>
        /// <param name="sort">Sort string to use in a form similiar to the orderby clause in a SQL statement</param>
        /// <param name="startIndex">For Paging the start index of the block of data to return</param>
        /// <param name="blockSize">For Paging the block size of the data to return</param>
        /// <returns></returns>
        public static XmlDocument FilterSortTheStoreResponse(XmlDocument response, object dataSetObjectResponse, string filter, string sort, int startIndex, int blockSize)
        {
            DataSet dsStore = (DataSet)dataSetObjectResponse;
            XmlDocument filterSortResponse = new XmlDocument();

            try
            {
                if (dsStore.Tables.Count == 0)
                {
                    filterSortResponse = CreateErrorDoc("Error: Cached Response for PaymentItems GetStoreInventory is not available");
                }
                else
                {
                    string orignalLength = dsStore.Tables[0].Rows.Count.ToString();
                    DataTable dtSort = CreateSortTableFromDataSet(dsStore);
                    AddDataRowsToSortTableFromDataSet(dtSort, dsStore.Tables[0].Rows);

                    DataRow[] sortedRows = SortFilterData(dtSort, filter, sort);
                    DataSet dsFinal = CopySortedDataToOrigClone(sortedRows, "itemOffers", dsStore, startIndex, blockSize);
                    StringWriter sw = new StringWriter();
                    dsFinal.WriteXml(sw);
                    filterSortResponse.LoadXml(UnEscapeXml(sw.ToString()));
                    XmlNode filterSortNode = filterSortResponse.SelectSingleNode("itemOffers");
                    filterSortResponse = ReplaceNode(response, "/Response/store", "itemOffers", filterSortNode);
                    filterSortResponse = AddItemsOffersAttributes(filterSortResponse, startIndex, blockSize, "false", orignalLength, sortedRows.Length);
                }
            }

            catch (Exception ex)
            {
                mLogger.Error("FilterSortTheStoreResponse Error: ", ex);
                filterSortResponse = CreateErrorDoc(ex.ToString());
            }

            return filterSortResponse;
        }

        /// <summary>
        /// For each item purchased, add the asset info from the asset database
        /// </summary>
        /// <param name="response"></param>
        /// <param name="serverStores"></param>
        /// <returns></returns>
        public static XmlDocument AddAssetsToPurchaseResponse(XmlDocument response, ServerStores serverStores)
        {
            XmlNodeList itemNodes = response.SelectNodes("Response/purchaseResults/purchaseResult/offer/item");
            foreach (XmlElement itemNode in itemNodes)
            {
                string itemId = itemNode.GetAttribute("name");
                string assetData = "<Assets>" + serverStores.PaymentItemsManager.GetAssetDataSetDna(itemId) + "</Assets>";
                XmlDocument assetXml = new XmlDocument();
                assetXml.LoadXml(assetData);
                XmlNode assetNode = assetXml.SelectSingleNode("Assets");

                XmlNode newChildNode = response.ImportNode(assetNode, true);
                itemNode.AppendChild(newChildNode);
            }
            
            return response;
        }

        /// <summary>
        /// Add Items node to the user inventory itemInstance node
        /// </summary>
        /// <param name="response">The Hangout user inventory xml response</param>
        /// <param name="handler">The ServerStores object, containing the item inventory information</param>
        /// <returns>The Hangout user inventory xml response with the user inventory node added to the itemInstance nodes</returns>
        public static XmlDocument AddItemsToUserInventory(XmlDocument response, ServerStores serverStores)
        {

            XmlDocument inventoryResponse = new XmlDocument();

            try
            {
                if (response == null)
                {
                    inventoryResponse = CreateErrorDoc("Error: Cached Response for PaymentItems User Inventory Response is not available");
                }
                else
                {
                    XmlNode itemInstancesNode = response.SelectSingleNode("/");

                    string itemTypeNames = itemInstancesNode.SelectSingleNode("/Response/itemInstances").Attributes["itemTypeNames"].InnerText.Trim();
       
                    //<itemInstances startIndex="5" blockSize="10" total="55">
                    int startIndex = serverStores.ConvertToInt(itemInstancesNode.SelectSingleNode("/Response/itemInstances").Attributes["startIndex"].InnerText, -1);
                    int blockSize = serverStores.ConvertToInt(itemInstancesNode.SelectSingleNode("/Response/itemInstances").Attributes["blockSize"].InnerText, -1);

                    int startPosition = 0;
                    int endPosition = itemInstancesNode.SelectNodes("Response/itemInstances/itemInstance").Count;

                    if (UseBlockSize(startIndex, blockSize, endPosition, itemTypeNames.Length))
                    {
                        startPosition = startIndex;
                        int maxEndPosition = blockSize + startIndex;
                        if (maxEndPosition < endPosition)
                        {
                            endPosition = maxEndPosition;
                        }
                    }

                    ServerPaymentItemsDataSets dataSets = new ServerPaymentItemsDataSets();
                    DataSet dsUserInventoryCreated = dataSets.CreateUserInventoryDataSet("InstanceDataSet");
                   // DataSet dsInstance = serverStores.LoadXMLInToDataSet(itemInstancesNode, dsUserInventoryCreated);

                    DataSet dsInstance = dataSets.LoadUserInventoryXMLIntoDataSet(itemInstancesNode, dsUserInventoryCreated, startPosition, endPosition);

                    //MemoryStream schema = serverStores.LoadSchemaFile("UserInventoryImportSchema.xsd");
                    //DataSet dsInstance = serverStores.LoadXMLInToDataSet(itemInstancesNode, schema, "InstanceDataSet");

                    DataTable dtInstance = dsInstance.Tables["itemInstance"];
                    DataTable dtItem = dsInstance.Tables["item"];

                    foreach (DataRow instanceRow in dtInstance.Rows)
                    {
                       
                        string itemId = instanceRow["itemId"].ToString();
                        DataRow itemRow = serverStores.FindItemInStore(itemId);
                        if (itemRow != null)
                        {
                            string hangoutItemId = instanceRow["itemName"].ToString();
                            itemRow["itemInstance_Id"] = instanceRow["itemInstance_Id"];
                            dtItem.ImportRow(itemRow);
                        }
                    }

                    StringWriter sw = new StringWriter();
                    dsInstance.WriteXml(sw);

                    XmlDocument responseTemp = new XmlDocument();
                    responseTemp.LoadXml(UnEscapeXml(sw.ToString()));

                    if (itemTypeNames.Length > 0)
                    {
                        responseTemp = FilterUserInventoryByItemTypes(responseTemp, itemTypeNames, startIndex, blockSize);
                    }

                    inventoryResponse.LoadXml(responseTemp.SelectSingleNode("/InstanceDataSet").InnerXml);
                }
            }

            catch (Exception ex)
            {
                mLogger.Error("Inventory response Error: ", ex);
                inventoryResponse = CreateErrorDoc(ex.ToString());
            }

            return inventoryResponse;
        }


        private static XmlDocument FilterUserInventoryByItemTypes(XmlDocument inventoryResponse, string itemTypeNames, int startIndex, int blockSize)
        {
            XmlDocument responseTemp = new XmlDocument();

            try
            {
                //<itemInstances startIndex="5" blockSize="10" total="55">
                responseTemp.LoadXml("<InstanceDataSet><Response noun='HangoutUsers' verb='GetUserInventory'><itemInstances></itemInstances></Response></InstanceDataSet>");
                XmlNodeList itemInstanceList = inventoryResponse.SelectNodes(GetItemTypesXPath("/InstanceDataSet/Response/itemInstances/itemInstance", itemTypeNames));

                AddAttribute(responseTemp, "/InstanceDataSet/Response/itemInstances", "startIndex", startIndex.ToString());
                AddAttribute(responseTemp, "/InstanceDataSet/Response/itemInstances", "blockSize", blockSize.ToString());
                AddAttribute(responseTemp, "/InstanceDataSet/Response/itemInstances", "total", itemInstanceList.Count.ToString());

                int startPosition = 0;
                int endPosition = itemInstanceList.Count;

                if (UseBlockSize(startIndex, blockSize, endPosition, 0))
                {
                    startPosition = startIndex;
                    int maxEndPosition = blockSize + startIndex;
                    if (maxEndPosition < endPosition)
                    {
                        endPosition = maxEndPosition;
                    }
                }

                for (int itemNumber = startPosition; itemNumber < endPosition; itemNumber++)
                {
                    XmlNode newNode = itemInstanceList[itemNumber];
                    responseTemp = AddNode(responseTemp, "/InstanceDataSet/Response/itemInstances", newNode);
                }
            }

            catch (Exception ex)
            {
                mLogger.Error("FilterUserInventoryByItemTypes Error: ", ex);
                responseTemp = CreateErrorDoc(ex.ToString());
            }

            return responseTemp;
        }


        //String.Format("/InstanceDataSet/Response/itemInstances/itemInstance[item/@itemTypeName='{0}']", itemTypeName));
        private static string GetItemTypesXPath(string baseXPath, string itemTypeNames)
        {
            string xPathQuery = "";
            string[] itemTypeNameArray = itemTypeNames.Split(',');
            string delim = "";

            foreach(string itemTypeName in itemTypeNameArray)
            {
                xPathQuery += String.Format("{0}item/@itemTypeName='{1}'", delim, itemTypeName.Trim());
                delim = " or ";

            }
            xPathQuery = String.Format("{0}[{1}]", baseXPath, xPathQuery);  

            return xPathQuery ;
        }


        /// <summary>
        /// Unescape the XML characters for '&lt;' and '&gt;'
        /// </summary>
        /// <param name="xmlString">The input xml string</param>
        /// <returns>The unescaped xml string replacing '&lt;' and '&gt;'</returns>
        private static string UnEscapeXml(string xmlString)
        {
            string xmlReturnString = "";

            if (!string.IsNullOrEmpty(xmlString))
            {
                xmlReturnString = xmlString.Replace("&lt;![CDATA[", "");
                xmlReturnString = xmlReturnString.Replace("]]&gt;", "");
                xmlReturnString = xmlReturnString.Replace("&lt;", "<");
                xmlReturnString = xmlReturnString.Replace("&gt;", ">");
                xmlReturnString = xmlReturnString.Replace("&quot;", "\"");
            }

            return xmlReturnString;
        }

        /// <summary>
        /// The Sorting, Filtering and Paging will be done post retrieve data from PaymentItems
        /// Therefore the GetStoreInventory will return all rows.
        /// </summary>
        /// <param name="command">PaymentCommand for the GetStoreInventory</param>
        /// <returns>PaymentCommand without the filtering for the GetStoreInventory</returns>
        public static PaymentCommand RemoveFilterFromCommand(PaymentCommand command)
        {
            PaymentCommand newCommand = new PaymentCommand();

            newCommand.Noun = command.Noun;
            newCommand.Verb = command.Verb;
 
            foreach (string key in command.Parameters.AllKeys)
            {
                switch (key)
                {
                    case "itemTypeNames":
                    case "filter":
                    case "orderBy":
                    case "startIndex":
                    case "blockSize":
                        break;

                    default:
                        newCommand.Parameters.Add(key, command.Parameters[key]);
                        break;
                }
            }

            return newCommand;
        }

        /// <summary>
        /// Given a PaymentCommand returns the Filter Paramaters specified
        /// </summary>
        /// <param name="command">PaymentCommand</param>
        /// <returns>The Filter Paramaters specified in the Payment Command</returns>
        public static ResultFilter GetFilterParameters(PaymentCommand command)
        {
            ResultFilter resultFilterInfo = new ResultFilter();

            resultFilterInfo.ItemTypeNames = GetValue(command.Parameters, "itemTypeNames", "");
            resultFilterInfo.StartIndex = GetValue(command.Parameters, "startIndex", "");
            resultFilterInfo.BlockSize = GetValue(command.Parameters, "blockSize", "");
            resultFilterInfo.Filter = GetValue(command.Parameters, "filter", "");
            resultFilterInfo.OrderBy = GetValue(command.Parameters, "orderBy", "");

            return resultFilterInfo;
        }

         /// <summary>
        /// Create a Sort table (denormialized flat table) from a dataset
        /// column names are set to be DataSet TableName_Column
        /// Need to replace column type for datetime so that filtering and sorting is datetime based
        /// </summary>
        /// <param name="ds">The relational dataset</param>
        /// <returns>returns the denormialized flat dataTable </returns>
        private static DataTable CreateSortTableFromDataSet(DataSet ds)
        {
            string[] dateTimeFields = { "endDate", "startDate" };
            DataTable dtSort = new DataTable("SortTable");

            foreach (DataTable dt in ds.Tables)
            {
                string tableName = dt.TableName;
                DataColumnCollection columnNames = dt.Columns;

                foreach (DataColumn column in columnNames)
                {
                    string newColumnName = String.Format("{0}_{1}", tableName, column.ColumnName);
                    if (IsDateTimeField(column.ColumnName, dateTimeFields))
                    {
                        dtSort.Columns.Add(newColumnName, Type.GetType("System.DateTime"));
                    }
                    else
                    {
                        dtSort.Columns.Add(newColumnName, column.DataType);
                    }
                }
            }
            return dtSort;
        }

        /// <summary>
        /// Copy data rows from the dataset to the flat sort table
        /// </summary>
        /// <param name="dtSort">The destination DataTable to copy the rows to</param>
        /// <param name="dataSetRows">The source dataset rows</param>
        private static void AddDataRowsToSortTableFromDataSet(DataTable dtSort, DataRowCollection dataSetRows)
        {
            foreach (DataRow offerRow in dataSetRows)
            {
                object offerRowData = offerRow.ItemArray;

                string[] dateTimeFields = {"endDate", "startDate"};

                DataRow newRow = dtSort.NewRow();
                CopyRowData(newRow, offerRow, dateTimeFields);

                DataRow[] itemRows = offerRow.GetChildRows("itemOffer_item");
                DataRow[] priceRows = offerRow.GetChildRows("itemOffer_price");

                foreach (DataRow priceRow in priceRows)
                {
                    object priceRowData = priceRow.ItemArray;
                    DataRow[] moneyRows = priceRow.GetChildRows("price_money");

                    foreach (DataRow moneyRow in moneyRows)
                    {
                        object moneyRowData = moneyRow.ItemArray;
                        CopyRowData(newRow, moneyRow);
                    }
                }

                foreach (DataRow itemRow in itemRows)
                {
                    object itemRowData = itemRow.ItemArray;
                    CopyRowData(newRow, itemRow);
                }

                dtSort.Rows.Add(newRow);
            }
        }

        /// <summary>
        /// Created a clone of the orignal and copy the orignal releational rows to the clone database
        /// The copy is done only for the rows that are part of the sorted filtered rows table
        /// The row order is keep the same as the sorted filtered rows table
        /// Only rows in the paging range (speciefied by startIndex, blockSize) are copyied
        /// </summary>
        /// <param name="sortedRows">Sorted Rows row set</param>
        /// <param name="cloneName">The Name of the clone dataset</param>
        /// <param name="dsStore">The oringnal dataset</param>
        /// <param name="startIndex">The start index for paging</param>
        /// <param name="blockSize">The size of the paging block</param>
        /// <returns>The sorted, filtered, and paged dataset</returns>
        private static DataSet CopySortedDataToOrigClone(DataRow[] sortedRows, string cloneName, DataSet dsStore, int startIndex, int blockSize)
        {
            DataRowCollection offerRows = dsStore.Tables[0].Rows;

            //DataSet dsFinal = new DataSet(cloneName);
           // dsFinal = dsStore.Clone();

            ServerPaymentItemsDataSets dataSets = new ServerPaymentItemsDataSets();
            DataSet dsFinal = dataSets.CreateStoreDataSet(cloneName, true);
            dsFinal.DataSetName = cloneName;

            int startPosition = 0;
            int endPosition = sortedRows.Length;

            if (UseBlockSize(startIndex, blockSize, endPosition, 0))
            {
                startPosition = startIndex;
                int maxEndPosition = blockSize + startIndex;
                if (maxEndPosition < endPosition)
                {
                    endPosition = maxEndPosition;
                }
            }
            for (int rowIndex = startPosition; rowIndex < endPosition; rowIndex++)  //foreach (DataRow row in sortedRows)
            {
                DataRow row = sortedRows[rowIndex];
                DataRow offerRow = offerRows.Find((object)row["itemOffer_itemOffer_Id"]);

                if (offerRow != null)
                {

                    DataRow[] itemRow = offerRow.GetChildRows("itemOffer_item");
                    DataRow[] priceRow = offerRow.GetChildRows("itemOffer_price");
                    DataRow[] moneyRow = priceRow[0].GetChildRows("price_money");

                    DataRow[] propertiesRow = itemRow[0].GetChildRows("item_properties");
                  //  DataRow[] propertyRow = propertiesRow[0].GetChildRows("properties_property");
  
                    dsFinal.Tables["itemOffer"].ImportRow(offerRow);
                    dsFinal.Tables["item"].ImportRow(itemRow[0]);
                    dsFinal.Tables["price"].ImportRow(priceRow[0]);
                    dsFinal.Tables["money"].ImportRow(moneyRow[0]);

                    //if (propertiesRow.Length > 0)
                   // {
                     //   dsFinal.Tables["properties"].ImportRow(propertiesRow[0]);
                    //    foreach (DataRow property in propertyRow)
                     //   {
                     //       dsFinal.Tables["property"].ImportRow(property);
                     //   }
                   // }
                }
            }

            return dsFinal;
        }

        /// <summary>
        /// Sort and Filter the Data using the Select method.
        /// </summary>
        /// <param name="dtSort">DataTable to Sort</param>
        /// <param name="filter">Filter string similiar to where clause in a SQL statement</param>
        /// <param name="sort">Sort string similiar to the orderby clause in a SQL statement</param>
        /// <returns>A datarow array containing the sorted and ordered items</returns>
        private static DataRow[] SortFilterData(DataTable dtSort, string filter, string sort)
        {
            return (dtSort.Select(filter, sort));
        }


        /// <summary>
        /// Copy row data from a source data row (in one table) to a destination data row (in another table)
        /// </summary>
        /// <param name="newRow">The destination data row</param>
        /// <param name="oldRow">The source data row</param>
        private static void CopyRowData(DataRow newRow, DataRow oldRow)
        {
            CopyRowData(newRow, oldRow, null);
        }


        /// <summary>
        /// Copy row data from a source data row (in one table) to a destination data row (in another table)
        /// When copying date time fields the date time needs to be converted from a string to 
        /// datetime that can be filtered and/or sorted
        /// </summary>
        /// <param name="newRow">The destination data row</param>
        /// <param name="oldRow">The source data row</param>
        /// <param name="dateTimeFields">string array containing the name of the dateTime fields</param>
        private static void CopyRowData(DataRow newRow, DataRow oldRow, string[] dateTimeFields)
        {
            string columnPrefix = oldRow.Table.TableName;

            foreach (DataColumn oldColumn in oldRow.Table.Columns)
            {
                string newColumnName = String.Format("{0}_{1}", columnPrefix, oldColumn.ColumnName);

                if (IsDateTimeField(oldColumn.ColumnName, dateTimeFields))
                {
                    string value = (string) oldRow[oldColumn];
                    value = value.Replace("PST", "").Replace("PDT", "").Trim();

                    DateTime dateTime = DateTime.ParseExact(value, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    newRow[newColumnName] = dateTime;
                }
                else
                {
                   newRow[newColumnName] = oldRow[oldColumn];
                }
            }
        }

        /// <summary>
        /// Compares the data table column name to date time field name 
        /// If they compare the a date time field has been found
        /// </summary>
        /// <param name="columnName">Data table column name</param>
        /// <param name="dateTimeFields">date time field string array containing a list of the date time fields</param>
        /// <returns>If a datetime field return true, if not return false</returns>
        private static bool IsDateTimeField(string columnName, string[] dateTimeFields)
        {
            bool isDateTimeField = false;
            if (dateTimeFields != null)
            {
                foreach (string dateTimeField in dateTimeFields)
                {
                    if (dateTimeField == columnName)
                    {
                        isDateTimeField = true;
                        break;
                    }
                }
            }
            return isDateTimeField;
        }


        /// <summary>
        /// Checks if paging values are valid.
        /// Start index and block size are positive integers
        /// The number of rows in the table is greater than the block size
        /// </summary>
        /// <param name="startIndex">start index value</param>
        /// <param name="blockSize">block size value</param>
        /// <param name="numOfRowsInTable"></param>
        /// <returns>True if paging values are valid else false</returns>
        private static bool UseBlockSize(int startIndex, int blockSize, int numOfRowsInTable, int skip)
        {
            bool useBlockSize = false;
            if ((startIndex > -1) && (blockSize > -1) && (skip == 0))
            {
                if (numOfRowsInTable > blockSize) 
                {
                    useBlockSize = true;
                }
            }
           return useBlockSize;
        }


        /// <summary>
        /// Add items offers attributes to the sorted, filtered, paged response xml
        /// </summary>
        /// <param name="filterSortResponse">filtered, sorted pages response xml document</param>
        /// <param name="startIndex">start index value</param>
        /// <param name="blockSize">block size value</param>
        /// <param name="skipCount">The skip count value</param>
        /// <param name="originalTotal">The original total rows value</param>
        /// <param name="total">The new total rows value</param> 
        /// <returns>filtered, sorted pages response xml document with the ItemsOffers attributes added</returns>
        private static XmlDocument AddItemsOffersAttributes(XmlDocument filterSortResponse, int startIndex, int blockSize, string skipCount, string originalTotal, int total)
        {
            string filter = "false";

            if (startIndex == -1)
            {
                startIndex = 0;
            }

            if (blockSize == -1)
            {
                blockSize = 0;
            }

            int originalTotalValue = total;
            int.TryParse(originalTotal, out originalTotalValue);

            if (originalTotalValue > total)
            {
                filter = "true";
            }

            AddAttribute(filterSortResponse, "/Response/store/itemOffers", "startIndex", startIndex.ToString());
            AddAttribute(filterSortResponse, "/Response/store/itemOffers", "blockSize", blockSize.ToString());
            AddAttribute(filterSortResponse, "/Response/store/itemOffers", "skipCount", "false");
            AddAttribute(filterSortResponse, "/Response/store/itemOffers", "total", total.ToString());
            AddAttribute(filterSortResponse, "/Response/store/itemOffers", "filter", filter);

            return filterSortResponse;
        }


        /// <summary>
        /// Add Attribute to the XMLDocument
        /// </summary>
        /// <param name="xmlDoc">XMLDocument to add the atttibute to</param>
        /// <param name="xPath">XPath of the node to add the attribute to.</param>
        /// <param name="attributeName">The attribute name</param>
        /// <param name="attributeValue">The attribut value</param>
        private static void AddAttribute(XmlDocument xmlDoc, string xPath, string attributeName, string attributeValue)
        {
            XmlNode xmlDocNode = xmlDoc.SelectSingleNode(xPath);
            XmlAttribute attribute = xmlDoc.CreateAttribute(attributeName);
            attribute.Value = attributeValue;
            xmlDocNode.Attributes.Append(attribute);
        }


        /// <summary>
        /// Replace a xml document node with a new node
        /// </summary>
        /// <param name="xmlDoc">Source XML document</param>
        /// <param name="xPath">xPath to node to replace</param>
        /// <param name="nodeName">The node name to replace</param>
        /// <param name="newNode">The new xml node</param>
        /// <returns>XmlDocument with the new node replacing the old node</returns>
        private static XmlDocument ReplaceNode(XmlDocument xmlDoc, string xPath, string nodeName, XmlNode newNode)
        {
            XmlNode newChildNode = xmlDoc.ImportNode(newNode, true);

            XmlNode parentNode = xmlDoc.SelectSingleNode(xPath); ;
            XmlNode oldNode = xmlDoc.SelectSingleNode(xPath + "/" + nodeName); ;

            parentNode.ReplaceChild(newChildNode, oldNode);

            return xmlDoc;
        }

        /// <summary>
        /// Add a node to an xml document
        /// </summary>
        /// <param name="xmlDoc">Source XML document</param>
        /// <param name="xPath">xPath to insert point</param>
        /// <param name="newNode">The new xml node</param>
        /// <returns>XmlDocument with the new node added</returns>
        private static XmlDocument AddNode(XmlDocument xmlDoc, string xPath, XmlNode newNode)
        {
            XmlNode newChildNode = xmlDoc.ImportNode(newNode, true);

            XmlNode parentNode = xmlDoc.SelectSingleNode(xPath); ;

            parentNode.AppendChild(newChildNode);

            return xmlDoc;
        }

        /// <summary>
        /// Get the value from the name value collection of Parameters
        /// </summary>
        /// <param name="Parameters">NameValueCollection containing Parameters</param>
        /// <param name="key">The key to locate in the NameValueCollection</param>
        /// <param name="defaultValue">The default value returned if the key is not found</param>
        /// <returns>The value for the key from the Parameters NameValueCollection, if no key is found then the default value</returns>
        private static string GetValue(NameValueCollection Parameters, string key, string defaultValue)
        {
            string value = defaultValue;

            try
            {
                value = Parameters[key];
            }

            catch { }

            return value;
        }

        /// <summary>
        /// Creates and error XML document response document from a message.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <returns>Error XML document</returns>
        protected static XmlDocument CreateErrorDoc(string message)
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

            StateServerAssert.Assert(new Exception(response.OuterXml));
            return response;
        }
     }
}
