using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Server;
using System.Xml;

namespace Hangout.Shared.UnitTest
{
    [TestFixture]
    public class ServerAssetRepositoryTest
    {
        private class ServerAssetRepositoryTestObject : ServerAssetRepository
        {
            public ServerAssetRepositoryTestObject() : base(null, null)
            {

            }
            protected override void CallServices() { } //test code shouldn't call services!
            public void TestGetItems(XmlDocument xmlResponse) { this.GetItems(xmlResponse); }
            public void TestGetAssetXml(XmlDocument xmlResponse) { this.GetAssetXml(xmlResponse); }
        }

		string itemsXmlString = "<InventoryItems> <Item ItemType=\"" + ItemType.TOPS + "\" SmallImageURL=\"\"> <ItemId>1</ItemId>  <AssetIdList>	<AssetId>1</AssetId>	<AssetId>2</AssetId>   </AssetIdList>  </Item>  <Item ItemType=\"" + ItemType.TOPS + "\" SmallImageURL=\"\">    <ItemId>2</ItemId>  <AssetIdList>  	<AssetId>3</AssetId>	<AssetId>4</AssetId>	</AssetIdList>   </Item>   <Item ItemType=\"" + ItemType.TOPS + "\" SmallImageURL=\"\">    <ItemId>3</ItemId>  <AssetIdList>    </AssetIdList>   </Item>  </InventoryItems>";
        string assetXmlString = "<Assets> <Asset AssetId=\"1\" AssetCategory=\"ColorAsset\" AssetType=\"SkinColor\" AssetName=\"Pale\">	<AssetData><AssetColor>F6C8B0</AssetColor><Alpha>0</Alpha></AssetData></Asset>	<Asset AssetId=\"2\" AssetCategory=\"ColorAsset\" AssetType=\"EyeColor\" AssetName=\"Blue\">	<AssetData><AssetColor>71D3FF</AssetColor><Alpha>0</Alpha></AssetData></Asset>	<Asset AssetId=\"3\" AssetCategory=\"ColorAsset\" AssetType=\"EyebrowColor\" AssetName=\"Hazel\">	<AssetData><AssetColor>815E3F</AssetColor><Alpha>0</Alpha></AssetData></Asset>  <Asset AssetId=\"4\" AssetCategory=\"ColorAsset\" AssetType=\"EyeShadowColor\" AssetName=\"Light Blue\">	<AssetData><AssetColor>815E3F</AssetColor><Alpha>0</Alpha></AssetData></Asset></Assets>";

        string expectedXmlResultString = "<Items>  <Item>    <Id>1</Id>    <Assets>      <Asset ItemId=\"1\" AssetId=\"1\" AssetCategory=\"ColorAsset\" AssetType=\"SkinColor\" AssetName=\"Pale\">        <AssetData>          <AssetColor>F6C8B0</AssetColor>          <Alpha>0</Alpha>        </AssetData>      </Asset>      <Asset ItemId=\"1\" AssetId=\"2\" AssetCategory=\"ColorAsset\" AssetType=\"EyeColor\" AssetName=\"Blue\">        <AssetData>          <AssetColor>71D3FF</AssetColor>          <Alpha>0</Alpha>        </AssetData>      </Asset>    </Assets>  </Item>  <Item>    <Id>2</Id>    <Assets>      <Asset ItemId=\"2\" AssetId=\"3\" AssetCategory=\"ColorAsset\" AssetType=\"EyebrowColor\" AssetName=\"Hazel\">        <AssetData>          <AssetColor>815E3F</AssetColor>          <Alpha>0</Alpha>        </AssetData>      </Asset>      <Asset ItemId=\"2\" AssetId=\"4\" AssetCategory=\"ColorAsset\" AssetType=\"EyeShadowColor\" AssetName=\"Light Blue\">        <AssetData>          <AssetColor>815E3F</AssetColor>          <Alpha>0</Alpha>        </AssetData>      </Asset>    </Assets>  </Item>  <Item>    <Id>3</Id>    <Assets></Assets>  </Item></Items>";

		private void MockSendToClient(Message m, Guid g)
		{

		}

        [Test]
        public void TestMockDataInputAndGetData()
        {
            XmlDocument expectedXmlResult = new XmlDocument();
            expectedXmlResult.LoadXml(expectedXmlResultString);

			ServerAssetRepositoryTestObject serverAssetRepositoryTestObject = new ServerAssetRepositoryTestObject();

            XmlDocument itemsXml = new XmlDocument();
            itemsXml.LoadXml(itemsXmlString);

            XmlDocument assetsXml = new XmlDocument();
            assetsXml.LoadXml(assetXmlString);

            serverAssetRepositoryTestObject.TestGetItems(itemsXml);
            serverAssetRepositoryTestObject.TestGetAssetXml(assetsXml);

            List<ItemId> itemIds = new List<ItemId>();
            itemIds.Add(new ItemId(1));
            itemIds.Add(new ItemId(2));
            itemIds.Add(new ItemId(3));

            XmlDocument assetRepoResponse = serverAssetRepositoryTestObject.GetXmlDna(itemIds);

			Assert.AreEqual(expectedXmlResult.SelectNodes("//*").Count, assetRepoResponse.SelectNodes("//*").Count);
        }
    }
}
