using System;
using System.Text;
using System.Web;
using MySql.Data;
using MySql.Data.MySqlClient;
using Hangout.Shared;
using Hangout.Server;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;




namespace Hangout.Server.WebServices
{
	public class InventoryService : ServiceBaseClass
    {
        private ServicesLog mServiceLog = new ServicesLog("InventoryService");

        public SimpleResponse GetItemList(string itemType)
        {
			mServiceLog.Log.InfoFormat("GetItemList");
			StringBuilder xmlBuilder = new StringBuilder();
			StringBuilder sqlOptions = new StringBuilder();
			Dictionary<uint, XElement> itemList = new Dictionary<uint, XElement>();
			try
            {
                using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.InventoryDBConnectionString))
                {
                    mysqlConnection.Open();
                    string getItemsListQuery = "SELECT * FROM ItemsList JOIN ItemsToAssetMapping ON (ItemsList.ItemId = ItemsToAssetMapping.ItemId) WHERE 1 ";
                    
                    
                    using (MySqlCommand itemsListCommand = mysqlConnection.CreateCommand())
                    {
						if(!String.IsNullOrEmpty(itemType))
						{
							sqlOptions.Append(" AND ItemsList.ItemType = @ItemType ");  
							itemsListCommand.Parameters.AddWithValue("@ItemType", itemType);
						} 


						getItemsListQuery += sqlOptions.ToString() + " ORDER BY ItemsList.ItemId;";
                        itemsListCommand.CommandText = getItemsListQuery;
                        using (MySqlDataReader itemsListReader = itemsListCommand.ExecuteReader())
                        {
							
                            while (itemsListReader.Read())
                            {
								uint itemId = Convert.ToUInt32(itemsListReader["ItemId"]);
								uint assetId = Convert.ToUInt32(itemsListReader["AssetId"]);

                                if (!itemList.ContainsKey(itemId))
                                {
									XElement itemElement = new XElement("Item");
									itemElement.Add( new XAttribute( "ItemType", itemsListReader["ItemType"] ) );
									string smallImageUrl = "";
									if (!String.IsNullOrEmpty(itemsListReader["SmallImageURL"].ToString()))
									{
										smallImageUrl += WebConfig.AssetBaseUrl + "Items/Thumbnails/" + itemsListReader["SmallImageURL"].ToString();
									}
									itemElement.Add(new XAttribute( "SmallImageURL" , smallImageUrl));

									XElement itemNumberElement = new XElement("ItemId");
									itemNumberElement.Value = itemId.ToString();

									itemElement.Add(itemNumberElement);
									

									XElement assetListElement = new XElement("AssetIdList");

									XElement assetIdElement = new XElement("AssetId");
									assetIdElement.Value = assetId.ToString();

									assetListElement.Add(assetIdElement);
									itemElement.Add(assetListElement);
									itemList.Add(itemId, itemElement);
                                }
                                else
                                {
									XElement assetIdElement = new XElement("AssetId");
									assetIdElement.Value = assetId.ToString();

									itemList[itemId].Element("AssetIdList").Add(assetIdElement);
                                }
							}
                        }
                    }
					
					
					foreach ( KeyValuePair<uint, XElement> item in itemList)
					{
						xmlBuilder.Append(item.Value.ToString());
					}
					

                }
				return new SimpleResponse("InventoryItems", xmlBuilder.ToString());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

		/// <summary>
		/// Optional assetId
		/// </summary>
		/// <param name="assetId"></param>
		/// <returns></returns>
		public SimpleResponse GetAssetList(string assetId)
		{
			mServiceLog.Log.InfoFormat("GetAssetList: assetId={0}", assetId);
			Dictionary<uint, XElement> assets = new Dictionary<uint, XElement>();

			StringBuilder xmlBuilder = new StringBuilder();

			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.InventoryDBConnectionString))
			{
				mysqlConnection.Open();
				string getAssetsQuery = "SELECT * FROM Assets ORDER BY AssetId";

				#region DatabaseCalls
				using (MySqlCommand getAssetsCommand = mysqlConnection.CreateCommand())
				{
					if (!String.IsNullOrEmpty(assetId))
					{
						getAssetsQuery = "SELECT * FROM Assets WHERE AssetId=@AssetId ORDER BY AssetId";
						getAssetsCommand.Parameters.AddWithValue("@AssetId", assetId);
					}
					getAssetsCommand.CommandText = getAssetsQuery;
					using (MySqlDataReader getAssetsReader = getAssetsCommand.ExecuteReader())
					{

						while (getAssetsReader.Read())
						{

							XElement asset = new XElement("Asset");
							asset.SetAttributeValue("AssetId", getAssetsReader["AssetId"].ToString());
							asset.SetAttributeValue("AssetType", getAssetsReader["AssetType"].ToString());
							asset.SetAttributeValue("AssetSubType", getAssetsReader["AssetSubType"].ToString());
							asset.SetAttributeValue("AssetName", getAssetsReader["AssetName"].ToString());

							string assetFilePath = getAssetsReader["AssetFilePath"].ToString();
							//asset.SetAttributeValue("FileName", assetFilePath);
							if (!String.IsNullOrEmpty(assetFilePath))
							{
								if (!assetFilePath.Contains("resources://"))
								{
									assetFilePath = WebConfig.AssetBaseUrl + assetFilePath.Trim();
								}

							}
							else
							{
								assetFilePath = "";
							}
							asset.SetAttributeValue("AssetFilePath", assetFilePath);

							string assetDataString = getAssetsReader["AssetData"].ToString();
							if (!String.IsNullOrEmpty(assetDataString))
							{
								XElement assetDataInnerXml = XElement.Parse(assetDataString);


								asset.Add(assetDataInnerXml);
							}
							else
							{
								asset.Add(new XElement("AssetData"));
							}


							uint assetIdFromDB = Convert.ToUInt32(getAssetsReader["AssetId"]);

							assets.Add(assetIdFromDB, asset);
						}
					}
					
					foreach (KeyValuePair<uint, XElement> asset in assets)
					{
						xmlBuilder.Append(asset.Value.ToString());
					}
					
				}

				#endregion


				return new SimpleResponse("Assets",xmlBuilder.ToString());
			}
		}

		public SimpleResponse AddNewAsset(string assetType, string assetSubType, string assetName, string assetExtention, string assetData)
		{
			mServiceLog.Log.InfoFormat("AddNewAsset: assetType={0}, assetSubType={1}, assetName={2}, assetExtention={3}, assetData={4}",
							assetType, assetSubType, assetName, assetExtention, assetData);

			AssetType aType;
			try
			{
				aType = (AssetType)Enum.Parse(typeof(AssetType), assetType);
			}
			catch (System.ArgumentNullException nullEx)
			{
				throw nullEx;
			}
			catch (System.ArgumentException argEx)
			{
				throw argEx;
			}

			AssetSubType aSubType;
			try
			{
				aSubType = (AssetSubType)Enum.Parse(typeof(AssetSubType), assetSubType);
			}
			catch (System.ArgumentNullException nullEx)
			{
				throw nullEx;
			}
			catch (System.ArgumentException argEx)
			{
				throw argEx;
			}

			string assetFilePath = "''";
			if (!String.IsNullOrEmpty(assetExtention))
			{
				assetFilePath = " CONCAT( @nextId, '" + assetExtention + "')";
			}

			string insertAssetQuery = "SET @nextId = '';" +
										"SELECT @nextId :=(MAX(AssetId)+1) FROM Assets;" +
										"INSERT INTO Assets ( AssetId, AssetType, AssetSubType, AssetName, AssetFilePath, AssetData ) " +
										"VALUES ( @nextId, @AssetType, @AssetSubType, @AssetName, " + assetFilePath + " ,@AssetData );" +
										"SELECT @nextId ;";
			try
			{
				using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.InventoryDBConnectionString))
				{
					mysqlConnection.Open();

					using (MySqlCommand newAssetCommand = mysqlConnection.CreateCommand())
					{
						newAssetCommand.CommandText = insertAssetQuery;
						newAssetCommand.Parameters.AddWithValue("@AssetType", aType.ToString());
						newAssetCommand.Parameters.AddWithValue("@AssetSubType", aSubType.ToString());
						newAssetCommand.Parameters.AddWithValue("@AssetName", assetName);
						newAssetCommand.Parameters.AddWithValue("@AssetData", assetData);

						object response = newAssetCommand.ExecuteScalar();
						string assetId = response.ToString();

						return new SimpleResponse("NewAssetId", assetId);
					}
				}
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
			throw new Exception("FAIL");
		}

		public SimpleResponse UpdateAsset(string assetType, string assetSubType, string assetName, string assetId, string assetData, string newFileName)
		{
			mServiceLog.Log.InfoFormat("UpdateAsset: assetType={0}, assetSubType={1}, assetName={2}, assetId={3}, assetData={4}, newFileName={5}",
							assetType, assetSubType, assetName, assetId, assetData, newFileName);
			string updateAssetQueryString = "UPDATE Assets SET " +
				"AssetType=@AssetType, AssetSubType=@AssetSubType, AssetName=@AssetName, " +
				"AssetFilePath=@AssetFilePath, AssetData=@AssetData " +
				"WHERE AssetId=@AssetId";
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.InventoryDBConnectionString))
			{
				mysqlConnection.Open();

				using (MySqlCommand updateAssetCommand = mysqlConnection.CreateCommand())
				{
					updateAssetCommand.CommandText = updateAssetQueryString;
					updateAssetCommand.Parameters.AddWithValue("@AssetId", assetId);
					updateAssetCommand.Parameters.AddWithValue("@AssetType", assetType);
					updateAssetCommand.Parameters.AddWithValue("@AssetSubType", assetSubType);
					updateAssetCommand.Parameters.AddWithValue("@AssetName", assetName);
					updateAssetCommand.Parameters.AddWithValue("@AssetFilePath", newFileName);
					updateAssetCommand.Parameters.AddWithValue("@AssetData", assetData);
					updateAssetCommand.ExecuteNonQuery();

					return new SimpleResponse("Success", "true");
				}
			}
		}

		public SimpleResponse DeleteAsset(string assetId)
		{
			mServiceLog.Log.InfoFormat("DeleteAsset: assetId={0}", assetId);
			XmlDocument response = GetAssetList(assetId);
			XmlNode asset = response.SelectSingleNode("//Asset");
			if(asset == null)
			{
				throw new Exception("No Asset Exists for Id: " + assetId);
			}

			string deleteAssetQueryString = "DELETE FROM Assets WHERE AssetId=@AssetId";
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.InventoryDBConnectionString))
			{
				mysqlConnection.Open();

				using (MySqlCommand deleteAssetCommand = mysqlConnection.CreateCommand())
				{
					deleteAssetCommand.CommandText = deleteAssetQueryString;
					deleteAssetCommand.Parameters.AddWithValue("@AssetId", assetId);
					deleteAssetCommand.ExecuteNonQuery();
				}
			}

			return new SimpleResponse("FileName", asset.Attributes["FileName"].Value);
			 
		}

		public SimpleResponse GeneratePaymentItemsCSV(string uploadToTwoFish)
		{
            mServiceLog.Log.InfoFormat("GeneratePaymentItemsCSV");
            StringBuilder xmlBuilder = new StringBuilder();
			try
            {
                using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.InventoryDBConnectionString))
                {
                    mysqlConnection.Open();
                    string getItemsListQuery = "SELECT * FROM ItemsList";
                    
                    Dictionary<uint, List<uint>> itemList = new Dictionary<uint, List<uint>>();
                    using (MySqlCommand itemsListCommand = mysqlConnection.CreateCommand())
                    {
                        itemsListCommand.CommandText = getItemsListQuery;
                        using (MySqlDataReader itemsListReader = itemsListCommand.ExecuteReader())
                        {
							xmlBuilder.Append("ItemName,ItemType,AppName,Description,ButtonName,SmallImageURL,MediumImageURL,LargeImageURL,MaxInstances,StoreName,StoreDescription,CurrencyName,ItemPrice,OfferTitle,OfferDescription,TradeInItemNames,EndDate\n");
                            while (itemsListReader.Read())
                            {
								string smallImageUrl = "";
								string mediumImageUrl = "";
								string largeImageUrl = "";
                                string devAssetBaseUrl = "http://www.assetbase.net/Items/Thumbnails";
								if (!String.IsNullOrEmpty(itemsListReader["SmallImageURL"].ToString()))
								{
									smallImageUrl = devAssetBaseUrl + "/" + itemsListReader["SmallImageURL"].ToString();
								}
								if (!String.IsNullOrEmpty(itemsListReader["MediumImageURL"].ToString()))
								{
									mediumImageUrl = devAssetBaseUrl + "/" + itemsListReader["MediumImageURL"].ToString();
								}
								if (!String.IsNullOrEmpty(itemsListReader["LargeImageURL"].ToString()))
								{
									largeImageUrl = devAssetBaseUrl + "/" + itemsListReader["LargeImageURL"].ToString();
								}

                                if (IsValidEntry(itemsListReader))
                                {
                                    xmlBuilder.Append
                                        (
                                            itemsListReader["ItemId"].ToString() + "," +
                                            itemsListReader["ItemType"].ToString() + "," +
                                            "HANGOUT_FAMILY_Application_1" + "," +
                                            Escape(itemsListReader["OfferDescription"].ToString()) + "," +  // Use OfferDescription for ItemDescription
                                            itemsListReader["OfferTitle"].ToString() + "," +                // Use OfferTitle for ButtonName
                                            smallImageUrl + "," +
                                            mediumImageUrl + "," +
                                            largeImageUrl + "," +
                                            itemsListReader["MaxInstances"].ToString() + "," +
                                            GetStoreName(itemsListReader) + "," +
                                            Escape(itemsListReader["StoreDescription"].ToString()) + "," +
                                            itemsListReader["CurrencyName"].ToString() + "," +
                                            itemsListReader["ItemPrice"].ToString() + "," +
                                            Escape(itemsListReader["OfferTitle"].ToString()) + "," +
                                            Escape(itemsListReader["OfferDescription"].ToString()) + "," +
                                            itemsListReader["TradeInItemNames"].ToString() + "," +
                                            itemsListReader["EndDate"].ToString() + "\n"
                                        );
                                }
							}
                        }
                    }
                }

				//CreateHangoutCommand cmd = new CreateHangoutCommand();
				//cmdType = cmd.GetType();
				//PaymentCommand command = CreateCommand("Upload Catalog Store", null, cmdType, GetSelectedXml());

				return new SimpleResponse("CSV", xmlBuilder.ToString());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        private string Escape(string entry)
        {
            // Make sure the entry is not blank, otherwise twofish will fill the data in with something random
            if (entry == "") entry = " ";

            return "\"" + entry + "\"";
        }

        private string GetStoreName(MySqlDataReader itemsListReader)
        {
            string stage = System.Configuration.ConfigurationSettings.AppSettings["Stage"];
            if (itemsListReader["CurrencyName"].ToString() == "VCOIN")
            {
                return itemsListReader["StoreName"].ToString() + "_COIN" + "_LIVE";
            }
            else
            {
				return itemsListReader["StoreName"].ToString() + "_CASH" + "_LIVE";
            }
        }

        private bool IsValidEntry(MySqlDataReader itemsListReader)
        {
            bool isValid = true;

            if (itemsListReader["StoreName"].ToString() == String.Empty) isValid = false;
            if (itemsListReader["ItemPrice"].ToString() == "0") isValid = false;

            return isValid;
        }
	}
}
