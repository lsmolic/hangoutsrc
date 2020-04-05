using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;

namespace Hangout.Server
{
    public class ServerPaymentItemsDataSets
    {

        /// <summary>
        /// Create the Store Data Set tables, columns and relationships
        /// </summary>
        /// <param name="dataSetName">The data set name</param>
        /// <param name="isFull">If it is full then add the assets node</param>
        /// <returns>The Store dataset </returns>
        public DataSet CreateStoreDataSet(string dataSetName, bool isFull)
        {
            DataSet dsImport = new DataSet(dataSetName);

            DataTable itemOfferTable = AddTableToDataSet(dsImport, "itemOffer");
            AddColumnToDataTable(itemOfferTable, "endDate", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(itemOfferTable, "numAvailable", System.Type.GetType("System.Int16"), MappingType.Attribute);
            AddColumnToDataTable(itemOfferTable, "startDate", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(itemOfferTable, "title", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(itemOfferTable, "description", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(itemOfferTable, "id", System.Type.GetType("System.UInt16"), MappingType.Attribute);
            //AddColumnToDataTable(itemOfferTable, "tradeInItems", System.Type.GetType("System.String"), MappingType.Element);
            AddColumnToDataTable(itemOfferTable, "itemOffer_Id", System.Type.GetType("System.Int32"), MappingType.Hidden);
            AddColumnToDataTable(itemOfferTable, "properties", System.Type.GetType("System.String"), MappingType.Element);

            AddPrimarKeyUniqueAutoIncrement(itemOfferTable, "itemOffer_Id");

            DataTable itemTable = CreateItemTable(dsImport, isFull);

            DataTable priceTable = AddTableToDataSet(dsImport, "price");
            AddColumnToDataTable(priceTable, "price_Id", System.Type.GetType("System.Int32"), MappingType.Hidden);
            AddColumnToDataTable(priceTable, "itemOffer_Id", System.Type.GetType("System.Int32"), MappingType.Hidden);

            AddPrimarKeyUniqueAutoIncrement(priceTable, "price_Id");

            DataTable moneyTable = AddTableToDataSet(dsImport, "money");
            AddColumnToDataTable(moneyTable, "currencyId", System.Type.GetType("System.UInt64"), MappingType.Attribute);
            AddColumnToDataTable(moneyTable, "currencyName", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(moneyTable, "amount", System.Type.GetType("System.Decimal"), MappingType.Attribute);
            AddColumnToDataTable(moneyTable, "price_Id", System.Type.GetType("System.Int32"), MappingType.Hidden);

            AddDataRelationship(dsImport, "price_money", priceTable.Columns["price_Id"], moneyTable.Columns["price_Id"]);
            AddDataRelationship(dsImport, "itemOffer_item", itemOfferTable.Columns["itemOffer_Id"], itemTable.Columns["itemOffer_Id"]);
            AddDataRelationship(dsImport, "itemOffer_price", itemOfferTable.Columns["itemOffer_Id"], priceTable.Columns["itemOffer_Id"]);

            return dsImport;
        }

        /// <summary>
        /// Create the User Inventory Data Set tables, columns and relationships
        /// </summary>
        /// <param name="dataSetName">The data set name</param>
        /// <returns>The User Inventory dataset </returns>
        public DataSet CreateUserInventoryDataSet(string dataSetName)
        {
            DataSet dsImport = new DataSet(dataSetName);

            DataTable responseTable = AddTableToDataSet(dsImport, "Response");
            AddColumnToDataTable(responseTable, "noun", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(responseTable, "verb", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(responseTable, "Response_Id", System.Type.GetType("System.Int32"), MappingType.Hidden);

            AddPrimarKeyUniqueAutoIncrement(responseTable, "Response_Id");

            DataTable itemInstancesTable = AddTableToDataSet(dsImport, "itemInstances");
            AddColumnToDataTable(itemInstancesTable, "startIndex", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(itemInstancesTable, "blockSize", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(itemInstancesTable, "total", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(itemInstancesTable, "itemInstances_Id", System.Type.GetType("System.Int32"), MappingType.Hidden);
            AddColumnToDataTable(itemInstancesTable, "Response_Id", System.Type.GetType("System.Int32"), MappingType.Hidden);

            AddPrimarKeyUniqueAutoIncrement(itemInstancesTable, "itemInstances_Id");

          //  DataTable itemTypesTable = AddTableToDataSet(dsImport, "itemTypes");
          //  AddColumnToDataTable(itemTypesTable, "itemTypes_Id", System.Type.GetType("System.Int32"), MappingType.Hidden);
           // AddColumnToDataTable(itemTypesTable, "itemInstances_Id", System.Type.GetType("System.Int32"), MappingType.Hidden);

           // AddPrimarKeyUniqueAutoIncrement(itemTypesTable, "itemTypes_Id");

          //  DataTable itemTypeTable = AddTableToDataSet(dsImport, "itemType");
           // AddColumnToDataTable(itemTypeTable, "id", System.Type.GetType("System.UInt16"), MappingType.Hidden);
          //  AddColumnToDataTable(itemTypeTable, "name", System.Type.GetType("System.String"), MappingType.Hidden);
          //  AddColumnToDataTable(itemTypeTable, "instances", System.Type.GetType("System.Byte"), MappingType.Hidden);
            //AddColumnToDataTable(itemTypeTable, "itemTypes_Id", System.Type.GetType("System.Int32"), MappingType.Hidden);

            DataTable itemInstanceTable = AddTableToDataSet(dsImport, "itemInstance");
            AddColumnToDataTable(itemInstanceTable, "id", System.Type.GetType("System.UInt16"), MappingType.Attribute);
            //AddColumnToDataTable(itemInstanceTable, "createdDate", System.Type.GetType("System.String"), MappingType.Attribute);
            //AddColumnToDataTable(itemInstanceTable, "userId", System.Type.GetType("System.UInt16"), MappingType.Attribute);
            //AddColumnToDataTable(itemInstanceTable, "appId", System.Type.GetType("System.UInt64"), MappingType.Attribute);
            //AddColumnToDataTable(itemInstanceTable, "appName", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(itemInstanceTable, "itemId", System.Type.GetType("System.UInt16"), MappingType.Attribute);
            AddColumnToDataTable(itemInstanceTable, "itemName", System.Type.GetType("System.String"), MappingType.Attribute);
            //AddColumnToDataTable(itemInstanceTable, "gift", System.Type.GetType("System.Boolean"), MappingType.Attribute);
            //AddColumnToDataTable(itemInstanceTable, "giftGiverUserId", System.Type.GetType("System.String"), MappingType.Attribute);
            //AddColumnToDataTable(itemInstanceTable, "giftVisibility", System.Type.GetType("System.String"), MappingType.Attribute);
            //AddColumnToDataTable(itemInstanceTable, "giftMessage", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(itemInstanceTable, "properties", System.Type.GetType("System.String"), MappingType.Element);
            AddColumnToDataTable(itemInstanceTable, "itemInstance_Id", System.Type.GetType("System.Int32"), MappingType.Hidden);
            AddColumnToDataTable(itemInstanceTable, "itemInstances_Id", System.Type.GetType("System.Int32"), MappingType.Hidden);

            AddPrimarKeyUniqueAutoIncrement(itemInstanceTable, "itemInstance_Id");

            DataTable itemTable = CreateItemTable(dsImport, true);
            AddColumnToDataTable(itemTable, "itemInstance_Id", System.Type.GetType("System.Int32"), MappingType.Hidden);

           // AddDataRelationship(dsImport, "itemTypes_itemType", itemTypesTable.Columns["itemTypes_Id"], itemTypeTable.Columns["itemTypes_Id"]);
            AddDataRelationship(dsImport, "itemInstance_item", itemInstanceTable.Columns["itemInstance_Id"], itemTable.Columns["itemInstance_Id"]);
           // AddDataRelationship(dsImport, "itemInstances_itemTypes", itemInstancesTable.Columns["itemInstances_Id"], itemTypesTable.Columns["itemInstances_Id"]);
            AddDataRelationship(dsImport, "itemInstances_itemInstance", itemInstancesTable.Columns["itemInstances_Id"], itemInstanceTable.Columns["itemInstances_Id"]);
            AddDataRelationship(dsImport, "Response_itemInstances", responseTable.Columns["Response_Id"], itemInstancesTable.Columns["Response_Id"]);

            return dsImport;

        }

        private DataTable CreateItemTable(DataSet ds, bool isFull)
        {
            DataTable itemTable = AddTableToDataSet(ds, "item");
            AddColumnToDataTable(itemTable, "id", System.Type.GetType("System.UInt16"), MappingType.Attribute);
            AddColumnToDataTable(itemTable, "name", System.Type.GetType("System.String"), MappingType.Attribute);
            //AddColumnToDataTable(itemTable, "appName", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(itemTable, "itemTypeName", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(itemTable, "buttonName", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(itemTable, "description", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(itemTable, "smallImageUrl", System.Type.GetType("System.String"), MappingType.Attribute);
            //  AddColumnToDataTable(itemTable, "mediumImageUrl", System.Type.GetType("System.String"), MappingType.Attribute);
            //  AddColumnToDataTable(itemTable, "largeImageUrl", System.Type.GetType("System.String"), MappingType.Attribute);
            AddColumnToDataTable(itemTable, "available", System.Type.GetType("System.Int16"), MappingType.Attribute);
            AddColumnToDataTable(itemTable, "itemOffer_Id", System.Type.GetType("System.Int32"), MappingType.Hidden);

            if (isFull)
            {
                AddColumnToDataTable(itemTable, "Assets", System.Type.GetType("System.String"), MappingType.Element);
            }

            return itemTable;
        }

        /// <summary>
        /// Add the table to the data set
        /// </summary>
        /// <param name="ds">the data set</param>
        /// <param name="tableName">The name of the the table to add</param>
        /// <returns>The datatable that was added</returns>
        private DataTable AddTableToDataSet(DataSet ds, string tableName)
        {
            ds.Tables.Add(tableName);
            return ds.Tables[tableName];
        }

        /// <summary>
        /// Add a column to the data table
        /// </summary>
        /// <param name="dt">The datatable</param>
        /// <param name="columnName">the column name</param>
        /// <param name="dataType">the data columns data type</param>
        /// <param name="mappingType">The data columns data mapping type (element, attribute, hidden) </param>
        private void AddColumnToDataTable(DataTable dt, string columnName, Type dataType, MappingType mappingType)
        {
            DataColumn column = new DataColumn(columnName, dataType, "", mappingType);
            dt.Columns.Add(column);
        }


        /// <summary>
        /// Add a data relationship to the data set
        /// </summary>
        /// <param name="ds">The data set</param>
        /// <param name="relationshipName">The relationship name</param>
        /// <param name="parentColumn">The parent column</param>
        /// <param name="childColumn">The child column</param>
        private void AddDataRelationship(DataSet ds, string relationshipName, DataColumn parentColumn, DataColumn childColumn)
        {
            DataRelation dataRelation = new DataRelation(relationshipName, parentColumn, childColumn, true);
            dataRelation.Nested = true;
            ds.Relations.Add(dataRelation);
        }

        /// <summary>
        /// Defines the column as Primary Key, Unique and Auto Increment with a seed of 0 and a step of 1
        /// </summary>
        /// <param name="dataTable">The data table</param>
        /// <param name="columnName">The column name</param>
        private void AddPrimarKeyUniqueAutoIncrement(DataTable dataTable, string columnName)
        {
            DataColumn[] primaryKey = new DataColumn[1];
            primaryKey[0] = dataTable.Columns[columnName];
            dataTable.PrimaryKey = primaryKey;
            dataTable.Columns[columnName].Unique = true;

            dataTable.Columns[columnName].AutoIncrement = true;
            dataTable.Columns[columnName].AutoIncrementSeed = 0;
            dataTable.Columns[columnName].AutoIncrementStep = 1;
        }

        /// <summary>
        /// Load the Store Data Set with the store xml data
        /// </summary>
        /// <param name="xmlStoreNodeList">The itemOffer node list</param>
        /// <param name="dsStore">The dataset to load</param>
        /// <param name="isFull">If true then add data to the asset value column</param>
        /// <returns>The dataset loaded with the store xml data</returns>
        public DataSet LoadStoreDataXMLInToDataSet(XmlNodeList xmlStoreNodeList, DataSet dsStore, bool isFull)
        {
            foreach (XmlNode xmlNode in xmlStoreNodeList)
            {
                DataTable dtItemOffer = dsStore.Tables["itemOffer"];
                DataRow itemOfferRow = AddRowValues(xmlNode, "", dtItemOffer);
                dtItemOffer.Rows.Add(itemOfferRow);
                int offerIndex = dtItemOffer.Rows.Count - 1;

                DataTable dtItem = dsStore.Tables["item"];
                DataRow itemRow = AddRowValues(xmlNode, "item", dtItem);
                itemRow["itemOffer_Id"] = offerIndex;

                if (isFull)
                {
                    string assetValue = xmlNode.SelectSingleNode("item/Assets").InnerXml;
                    itemRow["Assets"] = assetValue;
                }
                dtItem.Rows.Add(itemRow);

                DataTable dtPrice = dsStore.Tables["price"];
                DataRow priceRow = AddRowValues(xmlNode, "price", dtPrice);
                priceRow["itemOffer_Id"] = offerIndex;
                dtPrice.Rows.Add(priceRow);
                int priceIndex = dtPrice.Rows.Count - 1;

                DataTable dtMoney = dsStore.Tables["money"];
                DataRow moneyRow = AddRowValues(xmlNode, "price/money",  dtMoney);
                moneyRow["price_Id"] = priceIndex;
                dtMoney.Rows.Add(moneyRow);

            }
            return dsStore;
        }

        /// <summary>
        /// Load the User Inventory Data Set with the user inventory xml data
        /// </summary>
        /// <param name="xmlNode">The user inventory response node</param>
        /// <param name="dsStore">The dataset to load</param>
        /// <param name="startPosition">For filtering the start position (index)</param>
        /// <param name="endPosition">For filtering the end position (index)</param>
        /// <returns></returns>
        public DataSet LoadUserInventoryXMLIntoDataSet(XmlNode xmlNode, DataSet ds, int startPosition, int endPosition)
        {
            Dictionary<string, string> rowIndex = new Dictionary<string, string>();

            int responseIndex = AddNewRow(ds, xmlNode, "Response", "Response", rowIndex);

            rowIndex.Add("Response_Id", responseIndex.ToString());
            int itemInstancesIndex = AddNewRow(ds, xmlNode, "Response/itemInstances", "itemInstances", rowIndex);

           // rowIndex.Clear();
            //rowIndex.Add("itemInstances_Id", itemInstancesIndex.ToString());
            //int itemTypesIndex = AddNewRow(ds, xmlNode, "Response/itemInstances/itemTypes", "itemTypes", rowIndex);

         //   rowIndex.Clear();
           // rowIndex.Add("itemTypes_Id", itemTypesIndex.ToString());
            //AddNewRowMulitple(ds, xmlNode, "Response/itemInstances/itemTypes/itemType", "itemType", rowIndex);

            rowIndex.Clear();
            rowIndex.Add("itemInstances_Id", itemInstancesIndex.ToString());

            XmlNodeList nodeList = xmlNode.SelectNodes("Response/itemInstances/itemInstance");

            //foreach (XmlNode xmlNodeItem in nodeList)

            for (int rowNumber = startPosition; rowNumber < endPosition; rowNumber++)
            {
                XmlNode xmlNodeItem = nodeList[rowNumber];
                int itemInstanceIndex = AddNewRow(ds, xmlNodeItem, "", "itemInstance", rowIndex);
                Dictionary<string, string> rowIndex1 = new Dictionary<string, string>();
                rowIndex1.Add("itemInstance_Id", itemInstanceIndex.ToString());

                if (!String.IsNullOrEmpty(xmlNodeItem.InnerText))
                {
                    AddNewRow(ds, xmlNodeItem, "item", "item", rowIndex1);
                }
            }
            return ds;
        }


        /// <summary>
        /// Given a node that contains a nodelist of rows to add, add each row to the data table.
        /// </summary>
        /// <param name="ds">The dataset to add the row to</param>
        /// <param name="xmlNode">The xmlnode containing the nodelist of rows to add</param>
        /// <param name="xPath">The xpath for the node list</param>
        /// <param name="dataTableName">The datatable to add the rows to</param>
        /// <param name="rowIndex"></param>
        private void AddNewRowMulitple(DataSet ds, XmlNode xmlNode, string xPath, string dataTableName, Dictionary<string, string> rowIndex)
        {
            XmlNodeList nodeList = xmlNode.SelectNodes(xPath);
            foreach (XmlNode xmlNodeItem in nodeList)
            {
                AddNewRow(ds, xmlNodeItem, "", dataTableName, rowIndex);
            }
        }

        /// <summary>
        /// Adds a row to the table with the data from the xmlNode 
        /// </summary>
        /// <param name="ds">The dataset to add the row to</param>
        /// <param name="xmlNode">The xmlNode containing the data</param>
        /// <param name="xPath">The xpath to the node containing the data</param>
        /// <param name="dataTableName">The data table to update</param>
        /// <param name="rowIndex">A row index dictionary containng name value pairs representing the for a row the name of the 
        /// column that is the index and value of the index</param>
        /// <returns>The index of the updated row</returns>
        private int AddNewRow(DataSet ds, XmlNode xmlNode, string xPath, string dataTableName, Dictionary<string, string> rowIndex)
        {
            DataTable dt = ds.Tables[dataTableName];
            DataRow newRow = AddRowValues(xmlNode, xPath, dt);

            foreach (KeyValuePair<string, string> kvp in rowIndex)
            {
                newRow[kvp.Key] = kvp.Value;
            }

            dt.Rows.Add(newRow);
            return (dt.Rows.Count - 1);
        }

        /// <summary>
        /// Add the values to the rows columns
        /// </summary>
        /// <param name="xmlNode">The xmlNode containing the values</param>
        /// <param name="xPath">The xpath to the node containing the values</param>
        /// <param name="dataTable">The dataTable to add the row to</param>
        /// <returns>The data row with the values added</returns>
        private DataRow AddRowValues(XmlNode xmlNode, string xPath, DataTable dataTable)
        {
            DataRow dataRow = dataTable.NewRow();

            foreach (DataColumn column in dataTable.Columns)
            {
                if (column.ColumnMapping != MappingType.Hidden)
                {
                    string columnName = column.ColumnName;
                    string attributeValue = GetTheNodeAttributeValue(xmlNode, xPath, columnName);
                    if (CanUpdateValue(attributeValue, column.DataType))
                    {
                        dataRow[columnName] = attributeValue;
                    }
                }
            }
            return dataRow;
        }


        /// <summary>
        /// Determines if the data column can be updated. 
        /// If the column data type is not a string and the value is empty 
        /// then do not update. Do not want update and int field if the 
        /// value is blank.
        /// </summary>
        /// <param name="value">The value of the data</param>
        /// <param name="dataType">The columns data type.</param>
        /// <returns>True if can update the column else false</returns>
        private bool CanUpdateValue(string value, Type dataType)
        {
            bool returnValue = true;

            if (dataType != System.Type.GetType("System.String"))
            {
                if (value.Trim().Length == 0)
                {
                    returnValue = false;
                }
            }

            return returnValue;
        }


        /// <summary>
        /// Retruns the node attribute value given the xpath and the the attribute name.
        /// </summary>
        /// <param name="xmlNode">The xmlNode for the attribute</param>
        /// <param name="xPath">The xPath for the attribute</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>The attribute string value</returns>
        private string GetTheNodeAttributeValue(XmlNode xmlNode, string xPath, string attributeName)
        {
            string value = "";
            try
            {
                if (xPath == "")
                {
                    value = xmlNode.Attributes[attributeName].InnerText;
                }
                else
                {
                    value = xmlNode.SelectSingleNode(xPath).Attributes[attributeName].InnerText;
                }
            }

            catch { }
            return value;
        }

    }
}
