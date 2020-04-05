using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;

namespace Hangout.Server.WebServices
{
    internal class Upload : TwoFishCommandBase
    {
        public Upload() : base(System.Reflection.MethodBase.GetCurrentMethod()) { }

        /// <summary>
        /// Parse CSV file that is a Catalog File, a Store File or a combination of Catalog and Store File.
        /// Upload created Catalog and Store Files to Twofish returning the XML results form Twofish.
        /// </summary>
        /// <param name="itemInfo">ItemsInfo, used for the for the the IP Address.</param>
        /// <param name="fileData">PostFile The CSV file</param>
        /// <param name="commmonKeyValue">CommmonKeyValues</param>
        /// <param name="baseAddress">BaseAddress</param>
        /// <returns>An XML document containing the response from Catalog and or Store uploads as returned by Twofish.</returns>
        public XmlDocument UploadCatalogStore(ItemsInfo itemInfo, PostFile fileData, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            XmlDocument response = null;

            try
            {
                response = new XmlDocument();
                response.LoadXml("<response></response>");

                fileData.RemoveLeadingSpacesAndLines();

                PostFile catalogFile = CreateCatalogUpload(fileData);
                if (catalogFile.FileData != null)
                {
                    Catalog catalogCommand = new Catalog();
                    XmlDocument catalogResponse = catalogCommand.CreateCatalog(itemInfo, catalogFile, commmonKeyValue, baseAddress);
                    AddToResponse(response, "/response", "Catalog", catalogResponse);
                }

                StoreInfo storeInfo = new StoreInfo();
                storeInfo.IpAddress = itemInfo.IPAddress;

                string[] storeNameArray = FindStoreNames(fileData);

                if (storeNameArray != null)
                {
                    foreach (string storeName in storeNameArray)
                    {
                        Store storeCommand = new Store();
                        PostFile storeFile = CreateStoreUpload(fileData, storeName);
                        XmlDocument storeResponse = storeCommand.StoreBulkLoad(storeInfo, storeFile, commmonKeyValue, baseAddress);
                        AddToResponse(response, "/response", storeName, storeResponse);
                    }
                }
            }

            catch (Exception ex)
            {
                response = CreateErrorDoc(ex.Message);
                logError("UploadCatalogStore", ex);
            }

            return response;
        }

        /// <summary>
        /// Parse CSV file that is a Catalog File, a Store File or a combination of Catalog and Store File.
        /// This is to be able to review (create, test) the files to uploaded without doing the upload.
        /// </summary>
        /// <param name="fileData">PostFile CSV file</param>
        /// <returns>Ouputs a XML document containing the Catalog and or Store upload files.</returns>
        public XmlDocument GetUploadCatalogStoreFile(PostFile fileData)
        {
            XmlDocument response = null;
            try
            {
                response = new XmlDocument();
                response.LoadXml("<response></response>");

                fileData.RemoveLeadingSpacesAndLines();

                PostFile catalogFile = CreateCatalogUpload(fileData);
                if (catalogFile != null)
                {
                    AddXmlNodeFromFileData(response, "/response", "Catalog", catalogFile.FileData.ToArray());
                }

                string[] storeNameArray = FindStoreNames(fileData);

                if (storeNameArray != null)
                {
                    foreach (string storeName in storeNameArray)
                    {
                        PostFile storeFile = CreateStoreUpload(fileData, storeName);
                        AddXmlNodeFromFileData(response, "/response", storeName, storeFile.FileData.ToArray());
                    }
                }
            }

            catch (Exception ex)
            {
                response = CreateErrorDoc(ex.Message);
                logError("GetUploadCatalogStoreFile", ex);
            }

            return response;
        }

         /// <summary>
        /// First Pass through the CSV file to retrieve a unique list of Store Names
        /// Each Store need to be uploaded seperatly
        /// </summary>
        /// <param name="fileData">PostFile CSV File to parse</param>
        /// <returns>String array of unique Store Names</returns>
        private string[] FindStoreNames(PostFile fileData)
        {
            Dictionary<int, string> includeList = new Dictionary<int, string>();
            Dictionary<int, int> subList = new Dictionary<int, int>();
            Dictionary<int, int> skipList = new Dictionary<int, int>();
            Dictionary<int, string> formatList = new Dictionary<int, string>();

            string tempStoreNames = CreateUpload(fileData, "", "StoreName", "", includeList, subList, skipList, formatList);

            string[] storeNames = RemoveDups(tempStoreNames.Split('\n'));

            if (storeNames.Length > 0)
            {
                if ((string)storeNames.GetValue(0) == ",")
                {
                    storeNames = null;
                }
            }

            return storeNames;
        }

        /// <summary>
        /// Create the catalog file to upload 
        /// </summary>
        /// <param name="fileData">PostFile CSV File to parse</param>
        /// <returns>PostFile of the Catalog File to upload to Twofish</returns>
        private PostFile CreateCatalogUpload(PostFile fileData)
        {
            PostFile catalogFile = new PostFile();
            catalogFile.FileData = null;

            int[] skipColumn = FindColunmsNumberForFields(fileData, "ItemType,MaxInstances");

            if ((GetIntFromIntArray(skipColumn, 0) != 0) && (GetIntFromIntArray(skipColumn, 1) != -1))
            {
                Dictionary<int, string> includeList = new Dictionary<int, string>();
                Dictionary<int, int> subList = new Dictionary<int, int>();
                Dictionary<int, int> skipList = new Dictionary<int, int>();
                Dictionary<int, string> formatList = new Dictionary<int, string>();

                skipList.Add(GetIntFromIntArray(skipColumn, 0), GetIntFromIntArray(skipColumn, 0));
                skipList.Add(GetIntFromIntArray(skipColumn, 1), GetIntFromIntArray(skipColumn, 1));

                string headerToFind = "ItemName,ItemType,AppName,Description,ButtonName,SmallImageURL,MediumImageURL,LargeImageURL,MaxInstances";
                catalogFile = CreateUploadFile(fileData, "!TF createItem,,", headerToFind, headerToFind, includeList, subList, skipList, formatList);
            }
            return catalogFile;
        }


        /// <summary>
        /// Create the store file to upload 
        /// </summary>
        /// <param name="fileData">PostFile CSV File to parse</param>
        /// <param name="storeName">Store name</param>
        /// <returns>PostFile of the Store File to upload to Twofish</returns>
        private PostFile CreateStoreUpload(PostFile fileData, string storeName)
        {
            int[] fieldColumn = FindColunmsNumberForFields(fileData, "StoreName,OfferTitle,OfferDescription,Description,ApplicationName,EndDate");

            Dictionary<int, int> skipList = new Dictionary<int, int>();

            Dictionary<int, string> includeList = new Dictionary<int, string>();
            includeList.Add(GetIntFromIntArray(fieldColumn, 0), storeName);

            Dictionary<int, int> subList = new Dictionary<int, int>();
           // subList.Add(GetIntFromIntArray(fieldColumn, 1), GetIntFromIntArray(fieldColumn, 3));  // OfferTitle   Description
            //subList.Add(GetIntFromIntArray(fieldColumn, 2), GetIntFromIntArray(fieldColumn, 3));  //OfferDescription  Description

            Dictionary<int, string> formatList = new Dictionary<int, string>();
            formatList.Add(GetIntFromIntArray(fieldColumn, 5), "DATE");

            string headerToFind = "StoreName,AppName,StoreDescription,CurrencyName,ItemName,ItemPrice,OfferTitle,OfferDescription,TradeInItemNames,EndDate";
            string headerToUse = "StoreName,ApplicationName,StoreDescription,CurrencyName,ItemName,ItemPrice,OfferTitle,OfferDescription,TradeInItemNames,EndDate";

            //If ApplicationName name found in the header than use headerToUse as the find header.
            if (GetIntFromIntArray(fieldColumn, 4) > 0)
            {
                headerToFind = headerToUse;
            }

            return (CreateUploadFile(fileData, "!TF createStore,,", headerToFind, headerToUse, includeList, subList, skipList, formatList));
        }

        /// <summary>
        /// Splits the CSV input file into an array of lines.
        /// </summary>
        /// <param name="fileData">PostFile CSV File to parse</param>
        /// <returns>String array of CSV file lines</returns>
        private string[] FileDataToLineStringArray(PostFile fileData)
        {
            string[] lineArray = null;

            try
            {
                UTF8Encoding encoding = new UTF8Encoding();

                byte[] byteData = fileData.FileData.ToArray();

                String postFileString = encoding.GetString(byteData);

                postFileString = postFileString.Replace('\r', ' ');

                lineArray = postFileString.Split('\n');
            }

            catch { }

            return lineArray;
        }

        /// <summary>
        /// Creates the UploadFile returning a PostFile
        /// </summary>
        /// <param name="fileData">PostFile CSV input file</param>
        /// <param name="createLine">A string containg the first line in the TwoFish upload file</param>
        /// <param name="headerToFind">The header to find, name of source file fields in the order of fields to use</param>
        /// <param name="headerToUse">The header to use for the output file matched up field for field with the headerToFind</param>
        /// <param name="includeList">Required list data value index array. If the data value does not match, then skip this line.</param>
        /// <param name="subList">Substitution field index array. If the filed is blank, then use the substitution field for the data.</param>
        /// <param name="skipList">Required field index array. If the field is blank, then skip this line.</param>
        /// <param name="formatList">Format List index array.  If the field is found then format using the provided format type</param>
        /// <returns>PostFile containing the upload CSV file</returns>
        private PostFile CreateUploadFile(PostFile fileData, string createLine, string headerToFind, string headerToUse, Dictionary<int, string> includeList, Dictionary<int, int> subList, Dictionary<int, int> skipList, Dictionary<int, string> formatList)
        {
            PostFile postFile = new PostFile();
            UTF8Encoding encoding = new UTF8Encoding();

            string outputString = CreateUpload(fileData, createLine, headerToFind, headerToUse, includeList, subList, skipList, formatList);

            Byte[] byteArray = encoding.GetBytes(outputString);

            postFile.FileData.Write(byteArray, 0, byteArray.Length);

            return postFile;
        }

        /// <summary>
        /// Creates the UploadFile returning a string
        /// </summary>
        /// <param name="fileData">PostFile CSV input file</param>
        /// <param name="createLine">A string containg the first line in the TwoFish upload file</param>
        /// <param name="headerToFind">The header to find, name of source file fields in the order of fields to use</param>
        /// <param name="headerToUse"></param>
        /// <param name="includeList">Required list data value index array. If the data value does not match, then skip this line.</param>
        /// <param name="subList">Substitution field index array. If the filed is blank, then use the substitution field for the data.</param>
        /// <param name="skipList">Required field index array. If the field is blank, then skip this line.</param>
        /// <param name="formatList">Format List index array.  If the field is found then format using the provided format type</param>
        /// <returns>String containing the upload file data.</returns>
        private string CreateUpload(PostFile fileData, string createLine, string headerToFind, string headerToUse, Dictionary<int, string> includeList, Dictionary<int, int> subList, Dictionary<int, int> skipList, Dictionary<int, string> formatList)
        {
            int[] headerIndexArray = null;
            string outputString = "";
            int lineCount = 0;


            string[] lineArray = FileDataToLineStringArray(fileData);
            string lineString = GetLineFromArray(lineArray, lineCount++);

            while (lineString != null)
            {
                string[] inputItemArray = ConvertLineToItemArray(lineString);

                if (headerIndexArray == null)
                {
                    headerIndexArray = CheckForValidHeader(inputItemArray, headerToFind);
                    outputString = String.Format("{0}\r\n{1}\r\n", createLine, headerToUse);
                }
                else
                {
                    string[] outputItemArray = GetOutputItemArray(inputItemArray, headerIndexArray, includeList, subList, skipList, formatList);

                    if (outputItemArray != null)
                    {
                        string delim = "";
                        foreach (string item in outputItemArray)
                        {
                            outputString += String.Format("{0}{1}", delim, item);
                            delim = ",";
                        }
                        outputString += String.Format("\r\n");
                    }
                }
                lineString = GetLineFromArray(lineArray, lineCount++);
            }

            return outputString;
        }


        /// <summary>
        /// Returns the line from the GetLineFromArray.
        /// </summary>
        /// <param name="lineData">String array of CSV line data</param>
        /// <param name="line">Integer line number to retrieve</param>
        /// <returns>String which is the returned line. If no line is found then return null</returns>
        private string GetLineFromArray(string[] lineData, int line)
        {
            String lineString = null;
            try
            {
                if (lineData.Length > line)
                {
                    lineString = lineData[line];
                }
            }

            catch { }

            return lineString;
        }

        /// <summary>
        /// Check if a valid header is found, by matching up the headerToFind string with header in the file.
        /// </summary>
        /// <param name="inputItemArray">CSV input line in the form of an array</param>
        /// <param name="headerToFind">comma seperated string containing the header to find.</param>
        /// <returns>integer array containing the column index of the matching headers in the CSV file. 
        /// if an item is not found a -1  is placed  in the index.
        /// If the header is not found then null is returned.</returns>
        private int[] CheckForValidHeader(string[] inputItemArray, string headerToFind)
        {

            string[] headerArray = headerToFind.Split(',');
            int[] headerPositionArray = new int[headerArray.Length];
            int hearderCount = 0;

            for (int index = 0; index < headerPositionArray.Length; index++)
            {
                headerPositionArray[index] = -1;
            }

            try
            {
                foreach (string headerItem in headerArray)
                {
                    for (int index = 0; index < inputItemArray.Length; index++)
                    {
                        if (headerItem.ToLower().Trim() == inputItemArray[index].ToLower().Trim())
                        {
                            headerPositionArray[hearderCount] = index;
                            break;
                        }

                    }
                    hearderCount++;
                }

                if (headerPositionArray.Length != headerArray.Length)
                {
                    headerPositionArray = null;
                }
            }
            catch
            {
                headerPositionArray = null;
            }
            return headerPositionArray;
        }

        /// <summary>
        /// Returns an array of items repesenting the output CSV file.
        /// </summary>
        /// <param name="inputItemArray">Source csv item string array</param>
        /// <param name="sourceCSVPosition">Source csv position</param>
        /// <param name="includeList">Required list data value index array. If the data value does not match, then skip this line.</param>
        /// <param name="subList">Substitution field index array. If the filed is blank, then use the substitution field for the data.</param>
        /// <param name="skipList">Required field index array. If the field is blank, then skip this line.</param>
        /// <param name="formatList">Format List index array.  If the field is found then format using the provided format type</param>
        /// <returns></returns>
        private string[] GetOutputItemArray(string[] inputItemArray, int[] sourceCSVPosition, Dictionary<int, string> includeList, Dictionary<int, int> subList, Dictionary<int, int> skipList, Dictionary<int, string> formatList)
        {
            string[] outputItemArray = null;

            if (sourceCSVPosition.Length <= inputItemArray.Length)
            {
                try
                {
                    outputItemArray = new string[sourceCSVPosition.Length];

                    int destPosition = 0;
                    foreach (int sourcePosition in sourceCSVPosition)
                    {
                        int skipValue = -1;
                        if (skipList.TryGetValue(sourcePosition, out skipValue))
                        {
                            if (inputItemArray[sourcePosition].Trim().Length == 0)
                            {
                                outputItemArray = null;  //skip line
                                break;
                            }
                        }

                        string includeValue = "";
                        if (includeList.TryGetValue(sourcePosition, out includeValue))
                        {
                            if (inputItemArray[sourcePosition] != includeValue)
                            {
                                outputItemArray = null;  //skip line
                                break;
                            }
                        }

                        int subValue = -1;
                        bool subFlag = false;
                        if (subList.TryGetValue(sourcePosition, out subValue))
                        {
                            if (inputItemArray[sourcePosition].Trim().Length == 0)
                            {
                                outputItemArray[destPosition++] = inputItemArray[subValue];
                                subFlag = true;
                            }
                        }

                        if (!subFlag)
                        {
                            if (sourcePosition == -1)
                            {
                                outputItemArray[destPosition++] = ",";
                            }
                            else
                            {

                                outputItemArray[destPosition++] = FormatItem(inputItemArray[sourcePosition], sourcePosition, formatList);
                            }
                        }
                    }
                }

                catch
                {
                    outputItemArray = null;
                }
            }
            return outputItemArray;
        }

        /// <summary>
        /// Format the output source files columns based on custom formating required by Twofish.
        /// For now the only format type that is supported is date.
        /// </summary>
        /// <param name="item">String of the item</param>
        /// <param name="sourcePosition">Integer of the sourcePosition of data.</param>
        /// <param name="formatList">Dictionary specifying the formatList Postion and Format Type</param>
        /// <returns>If formatType == "DATE" then convert date to "yyyy/MM/dd HH:mm:ss PST" else return the original item</returns>
        private string FormatItem(string item, int sourcePosition, Dictionary<int, string> formatList)
        {
            string returnItem = item;

            string formatType = "";
            if (formatList.TryGetValue(sourcePosition, out formatType))
            {
                if (formatType == "DATE")  //output format yyyy/MM/dd HH:mm:ss zzz
                {
                    DateTime dt = Convert.ToDateTime(item);
                    returnItem = dt.ToString("yyyy/MM/dd HH:mm:ss PST");

                }
            }
            return returnItem;
        }

        /// <summary>
        /// Return column index array for the column header fields.
        /// Given a CSV file, returns an integer array with the column number references by the columnNames
        /// The columns names is a comma seperate list of columns.
        /// If the column is not found then the returned array is -1 for that position.
        /// </summary>
        /// <param name="fileData">PostFile CSV File</param>
        /// <param name="colunmNames">Comma seperate string of columns to find.</param>
        /// <returns>An interger array of the index of the matching colunmNames fields, if no match is found then a -1 is in the array for that position</returns>
        private int[] FindColunmsNumberForFields(PostFile fileData, string colunmNames)
        {
            int[] headerIndexArray = null;
            int lineCount = 0;

            string[] lineArray = FileDataToLineStringArray(fileData);
            string lineString = GetLineFromArray(lineArray, lineCount++);

            while (lineString != null)
            {
                string[] inputItemArray = ConvertLineToItemArray(lineString);
                headerIndexArray = CheckForValidHeader(inputItemArray, colunmNames);
                break;
            }
            return headerIndexArray;
        }

        /// <summary>
        /// Remove duplicates from a array of strings
        /// </summary>
        /// <param name="items">String array of items</param>
        /// <returns>String array of items with duplicates removed</returns>
        private string[] RemoveDups(string[] items)
        {
            ArrayList noDups = new ArrayList();
            for (int i = 0; i < items.Length; i++)
            {
                if (!noDups.Contains(items[i].Trim()))
                {
                    if (items[i].Trim().Length > 0)
                    {
                        noDups.Add(items[i].Trim());
                    }
                }
            }
            string[] uniqueItems = new String[noDups.Count];

            for (int i = 0; i < noDups.Count; i++)
            {
                uniqueItems[i] = (string)noDups[i];
            }

            return uniqueItems;
        }

        /// <summary>
        /// Given an index into an integer array return the array value
        /// if the item does not exits in the array then return -1
        /// </summary>
        /// <param name="intArray">Integer array of values</param>
        /// <param name="index">Index into array values</param>
        /// <returns>Index of item found else -1.</returns>
        private int GetIntFromIntArray(int[] intArray, int index)
        {
            int retValue = -1;
            try
            {
                retValue = (int)intArray.GetValue(index);
            }
            catch { }

            return retValue;
        }
         
        /// <summary>
        /// Compare the string values in 2 arrays. 
        /// </summary>
        /// <param name="array1">Array 1 to compare</param>
        /// <param name="array2">Array 2 to compare</param>
        /// <returns>True if string values in the 2 arrays match else false.</returns>
        private bool CompareArrays(Array array1, Array array2)
        {
            bool returnValue = true;

            if (array1.Length != array2.Length)
            {
                returnValue = false;
            }
            else
            {
                for (int i = 0; i < array1.Length; i++)
                {
                    if (array1.GetValue(i).ToString() != array2.GetValue(i).ToString())
                    {
                        returnValue = false;
                        break;
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Converts a CSV file Line into a array of items, one item for each column
        /// </summary>
        /// <param name="lineString">String CSV file line</param>
        /// <returns>String array of columns from the CSV file line</returns>
        private string[] ConvertLineToItemArray(string lineString)
        {
            //  , zzzz,
            //  , " 1,23 ", 

            string[] lineArray = null;
            ArrayList lineArrayList = new ArrayList();
            StringBuilder item = new StringBuilder();

            int count = 0;
            int quote = 0;
            while (count < lineString.Length)
            {
                char currentChar = lineString[count];

                if (currentChar == '"')
                {
                    if (++quote > 1)
                    {
                        quote = 0;
                    }
                }

                if (quote == 0)
                {
                    if (currentChar == ',')
                    {
                        lineArrayList.Add(item.ToString());
                        item.Remove(0, item.Length);
                    }
                    else
                    {
                        item.Append(currentChar);
                    }
                }
                else
                {
                    item.Append(currentChar);
                }
                count++;
            }
            lineArrayList.Add(item.ToString());

            lineArray = new String[lineArrayList.Count];

            for (int i = 0; i < lineArrayList.Count; i++)
            {
                lineArray[i] = (string)lineArrayList[i];
            }

            return lineArray;
        }

        /// <summary>
        /// Copies the response node from the twoFish response and adds the node to the response document.
        /// </summary>
        /// <param name="response">response node document</param>
        /// <param name="xPath">The XPath to add for the response node</param>
        /// <param name="nodeName">The name of the node to add</param>
        /// <param name="twofishResponse">Twofish response document, this is the source document.</param>
        private void AddToResponse(XmlDocument response, string xPath, string nodeName, XmlDocument twofishResponse)
        {
            XmlNode childNode = twofishResponse.SelectSingleNode ("/response");

            if (childNode == null)
            {
                twofishResponse.InnerXml = String.Format("<response><{1}>{0}</{1}></response>", twofishResponse.InnerXml, nodeName );
                childNode = twofishResponse.SelectSingleNode("/response");
            }

            XmlElement element = response.CreateElement(nodeName);
            response.SelectSingleNode(xPath).AppendChild(element);

            XmlNode newChildNode = response.ImportNode(childNode, true);
            XmlNode parentNode = response.SelectSingleNode(xPath + "/" + nodeName);

            parentNode.AppendChild(newChildNode);
        }

        /// <summary>
        /// Creates XML node containing the string representation of passed in byte data 
        /// </summary>
        /// <param name="doc">XML document to add the node into</param>
        /// <param name="xPath">XPath where the node is added.</param>
        /// <param name="nodeName">Node Name to add data to</param>
        /// <param name="byteData">Byte data that is converted to a string and then added to the node.</param>
        private void AddXmlNodeFromFileData(XmlDocument doc, string xPath, string nodeName, byte[] byteData)
        {
            try
            {
                string fileString = Encoding.UTF8.GetString(byteData, 0, byteData.Length);
                XmlDocument dataDoc = new XmlDocument();
                StringBuilder sb = new StringBuilder();

                sb.Append(String.Format("<{0}>{1}</{0}>", nodeName, fileString));
                dataDoc.LoadXml(sb.ToString());

                XmlNode childNode = dataDoc.SelectSingleNode(nodeName);

                XmlNode newChildNode = doc.ImportNode(childNode, true);
                XmlNode parentNode = doc.SelectSingleNode(xPath);

                parentNode.AppendChild(newChildNode);
            }
            catch { }
        }
    }
}

