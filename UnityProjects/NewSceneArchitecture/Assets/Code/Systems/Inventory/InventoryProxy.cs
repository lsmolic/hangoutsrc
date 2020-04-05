/**  --------------------------------------------------------  *
 *   InventoryMediator.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 08/26/2009
 *	 
 *   --------------------------------------------------------  *
 */

using PureMVC.Patterns;
using PureMVC.Interfaces;

using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Client.Gui;
using Hangout.Shared;
using Hangout.Shared.Messages;

namespace Hangout.Client
{	
	public class InventoryProxy : Proxy, IMessageRouter
	{
        private string mSecurePaymentInfo = "";
        public string SecurePaymentInfo
        {
            get { return mSecurePaymentInfo; }
        }

        private PaymentItemsCommand mPaymentItemsCommand;
		private InventoryGuiController mInventoryController;
        private ClientMessageProcessor mClientMessageProcessor = null;

		private readonly Dictionary<MessageSubType, Action<Message>> mMessageActions = new Dictionary<MessageSubType, Action<Message>>();
		
		private double mVCoin = 0.0;
		public double VCoin
		{
			get { return mVCoin; }
		}
		private double mHouts = 0.0;
		public double Houts
		{
			get { return mHouts; }
		}
		
		public InventoryProxy() 
			: base()
		{
            // Setup message callbacks
            mMessageActions.Add(MessageSubType.GetBalance, HandleUserBalanceResponse);
            mMessageActions.Add(MessageSubType.PaymentId, HandlePaymentIdResponse);
            Init();
        }

        private void Init()
        {
            mClientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
            IGuiManager guiManager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
            mInventoryController = new InventoryGuiController(guiManager);
            mPaymentItemsCommand = new PaymentItemsCommand();
        }

        public void BeginShopping()
        {
            mInventoryController.BeginShopping();
        }
        
        public void TogglePlayerInventory()
        {
			mInventoryController.TogglePlayerInventory();
        }

        public void OpenPlayerInventory()
        {
            mInventoryController.OpenPlayerInventory();
        }

        public void CloseInventory()
        {
            mInventoryController.ClosePlayerInventory();
        }


        /// <summary>
        /// Send message to server requesting an update of user gold and game currency balance
        /// </summary>
        /// <param name="notification"></param>
        public void GetUserBalance()
        {
            Dictionary<string, string> commandParameters = new Dictionary<string, string>();
            commandParameters.Add("userSession", mClientMessageProcessor.LocalSessionId.ToString());

            string paymentItemsCommand = mPaymentItemsCommand.CreatePaymentCommand("GetUserBalance", commandParameters);
            SendPaymentItemsMessage(paymentItemsCommand, (int)MessageSubType.GetBalance);
        }

        /// <summary>
        /// Send message to server requesting an update of user gold and game currency balance
        /// </summary>
        /// <param name="notification"></param>
        public void GetPaymentId()
        {
            Dictionary<string, string> commandParameters = new Dictionary<string, string>();
            string paymentItemsCommand = mPaymentItemsCommand.CreatePaymentCommand("SecurePaymentInfo", commandParameters);
            SendPaymentItemsMessage(paymentItemsCommand, (int)MessageSubType.PaymentId);
        }

        /// <summary>
        /// Send message to server requesting the secure payment id.  This id gets passed to the secure payment page
        /// </summary>
        /// <param name="notification"></param>
        private void HandlePaymentIdResponse(Message receivedMessage)
        {
            string response = (string)receivedMessage.Data[0];
            XmlDocument xmlResponse = new XmlDocument();
            xmlResponse.LoadXml(response);
            mSecurePaymentInfo = xmlResponse.SelectSingleNode("/Response/SecureInfo").InnerText;
            //Debug.Log("Got securePaymentinfo " + mSecurePaymentInfo);
        }

		public void GetPlayerInventory(int startIndex, int blockSize, Action<Message> playerInventoryCallback)
        {
            Dictionary<string, string> commandParameters = new Dictionary<string, string>();
            commandParameters.Add("userSession", mClientMessageProcessor.LocalSessionId.ToString());
            commandParameters.Add("startIndex", startIndex.ToString());
            commandParameters.Add("blockSize", blockSize.ToString());
            string paymentItemsCommand = mPaymentItemsCommand.CreatePaymentCommand("GetUserInventory", commandParameters);

			mMessageActions.Remove(MessageSubType.PlayerInventory);
			mMessageActions.Add(MessageSubType.PlayerInventory, playerInventoryCallback);

            SendPaymentItemsMessage(paymentItemsCommand, (int)MessageSubType.PlayerInventory);
        }
        /// <summary>
        /// The callback for a PlayerInventory request
        /// </summary>
        /// <param name="receivedMessage"></param>
        public void HandlePlayerInventoryResponse(Message receivedMessage)
        {
			mMessageActions.Remove(MessageSubType.PlayerInventory);
			string response = (string)receivedMessage.Data[0];
            XmlDocument xmlResponse = new XmlDocument();
            xmlResponse.LoadXml(response);

            mInventoryController.HandlePlayerInventoryResult(xmlResponse);
        }

		public void GetPlayerAnimations(string itemTypes, Action<Message> playerInventoryCallback)
        {
            Dictionary<string, string> commandParameters = new Dictionary<string, string>();
            commandParameters.Add("userSession", mClientMessageProcessor.LocalSessionId.ToString());
            commandParameters.Add("itemTypeNames", itemTypes);
            string paymentItemsCommand = mPaymentItemsCommand.CreatePaymentCommand("GetUserInventory", commandParameters);
			
			mMessageActions.Remove(MessageSubType.PlayerInventory);
			mMessageActions.Add(MessageSubType.PlayerInventory, playerInventoryCallback);

			SendPaymentItemsMessage(paymentItemsCommand, (int)MessageSubType.PlayerInventory);
        }
        /// <summary>
        /// The callback for an Animation request
        /// </summary>
        /// <param name="receivedMessage"></param>
        public void HandlePlayerAnimationResponse(Message receivedMessage)
        {
			mMessageActions.Remove(MessageSubType.PlayerInventory);
			string response = (string)receivedMessage.Data[0];
            XmlDocument xmlResponse = new XmlDocument();
            xmlResponse.LoadXml(response);

            AnimationProxy animationProxy = GameFacade.Instance.RetrieveProxy<AnimationProxy>();
            animationProxy.SetOwnedEmotesAndMoodsXml(xmlResponse);
        }

        public void GetStoreInventory(string storeName, string itemTypes, int startIndex, int blockSize, Action<Message> getStoreInventoryCallback)
        {
            Dictionary<string, string> commandParameters = new Dictionary<string, string>();
            commandParameters.Add("storeName", storeName);
            commandParameters.Add("itemTypeNames", itemTypes);
            commandParameters.Add("startIndex", startIndex.ToString());
            commandParameters.Add("blockSize", blockSize.ToString());
            string paymentItemsCommand = mPaymentItemsCommand.CreatePaymentCommand("GetStoreInventory", commandParameters);

			if (mMessageActions.ContainsKey(MessageSubType.StoreInventory))
			{
				mMessageActions.Remove(MessageSubType.StoreInventory);
			}
			mMessageActions.Add(MessageSubType.StoreInventory, getStoreInventoryCallback);
            SendPaymentItemsMessage(paymentItemsCommand, (int)MessageSubType.StoreInventory);

        }
        /// <summary>
        /// The callback for a StoreInventory request
        /// </summary>
        /// <param name="receivedMessage"></param>
        public void HandleStoreInventoryResponse(Message receivedMessage)
        {
            mMessageActions.Remove(MessageSubType.StoreInventory);

            string response = (string)receivedMessage.Data[0];
            XmlDocument xmlResponse = new XmlDocument();
            xmlResponse.LoadXml(response);
            mInventoryController.HandleStoreInventoryResult(xmlResponse);
        }

        /// <summary>
        /// The callback for a StoreInventory request
        /// </summary>
        /// <param name="receivedMessage"></param>
        private void HandleEnergyStoreResponse(Message receivedMessage)
        {
			mMessageActions.Remove(MessageSubType.StoreInventory);
			
			string response = (string)receivedMessage.Data[0];
            XmlDocument xmlResponse = new XmlDocument();
            xmlResponse.LoadXml(response);
        }

        public void PurchaseRequest(string itemOffer, string currencyName, Action<Message> purchaseCallback)
        {
            // Popup modal dialog saying purchase is being completed
            mInventoryController.ShowPurchaseModal();
            
            Dictionary<string, string> commandParameters = new Dictionary<string, string>();
            commandParameters.Add("offerIds", itemOffer);
            commandParameters.Add("currencyName", currencyName);
            commandParameters.Add("userSession", mClientMessageProcessor.LocalSessionId.ToString());
            //Debug.Log("PurchaseRequest, itemOffer = " + itemOffer);
            string paymentItemsCommand = mPaymentItemsCommand.CreatePaymentCommand("PurchaseItems", commandParameters);

			if (mMessageActions.ContainsKey(MessageSubType.Purchase))
			{
				mMessageActions.Remove(MessageSubType.Purchase);
			}
			mMessageActions.Add(MessageSubType.Purchase, purchaseCallback);

            SendPaymentItemsMessage(paymentItemsCommand, (int)MessageSubType.Purchase);
        }

		/// <summary>
		/// The callback for when a messageType inventory comes in.
		/// </summary>
		/// <param name="receivedMessage"></param>
		public void HandlePurchaseResponse(Message receivedMessage)
		{
			mMessageActions.Remove(MessageSubType.Purchase);

			string response = (string)receivedMessage.Data[0];
			XmlDocument xmlResponse = new XmlDocument();
			xmlResponse.LoadXml(response);
			//Debug.Log("HandlePurchaseResponse: " + response);
			// Check for errors
			if (!DisplayErrorDialog(xmlResponse))
			{
				// No errors, continue showing that the purchase went through
				DisplaySuccessDialog(xmlResponse);

				// Update toolbar with new balance
				UpdateUserBalanceDisplay(xmlResponse);

				// Update avatar with changes
				UpdateAvatarWithPurchasedItems(xmlResponse);

			}
			mInventoryController.HidePurchaseModal();
		}

		public void HidePurchaseModel()
		{
			mInventoryController.HidePurchaseModal();
		}

		//Sets up the dictionary of messageTypes to the callback associated with it to
		//be executed when a message comes in.
		public void ReceiveMessage(Message receivedMessage)
		{
            MessageUtil.ReceiveMessage<MessageSubType>(receivedMessage, mMessageActions);
		}

		public void RegisterSendMessageCallback(SendMessageCallback sendMessageCallback)
		{
		}

        /// <summary>
        /// Send paymentItemCommand string to server.   
        /// </summary>
        /// <param name="paymentItemsCommand"></param>
        private void SendPaymentItemsMessage(string paymentItemsCommand, int callbackActionType)
        {
            Message paymentItemsMessage = new Message();
            List<object> dataObject = new List<object>();
            dataObject.Add(paymentItemsCommand);
            paymentItemsMessage.PaymentItemsMessage(dataObject);
            paymentItemsMessage.Callback = callbackActionType;

            mClientMessageProcessor.SendMessageToReflector(paymentItemsMessage);
        }
        

        private void UpdateAvatarWithPurchasedItems(XmlDocument xmlResponse)
        {
            LocalAvatarEntity localAvatarEntity = GameFacade.Instance.RetrieveMediator<AvatarMediator>().LocalAvatarEntity;
            XmlNodeList xmlNodes = xmlResponse.SelectNodes("Response/purchaseResults/purchaseResult/offer/item/Assets");
            foreach (XmlNode assetsNode in xmlNodes)
            {
                localAvatarEntity.ApplyAssetsToAvatar(assetsNode);
            }
        }

        private void HandleUserBalanceResponse(Message receivedMessage)
        {
            string response = (string)receivedMessage.Data[0];
            XmlDocument xmlResponse = new XmlDocument();
            xmlResponse.LoadXml(response);

            UpdateUserBalanceDisplay(xmlResponse);
        }

        private void UpdateUserBalanceDisplay(XmlDocument xmlResponse)
        {
            XmlElement vcoinNode = (XmlElement)xmlResponse.SelectSingleNode("//accounts/account[@currencyName='VCOIN']");
            XmlElement houtsNode = (XmlElement)xmlResponse.SelectSingleNode("//accounts/account[@currencyName='HOUTS']");

            // Convert to double, cast to int and then back to string to strip off decimal points
            if(vcoinNode != null)
            {
				mVCoin = (int)(Double.Parse(vcoinNode.GetAttribute("balance")));
            }
            if (houtsNode != null)
            {
                mHouts = (int)(Double.Parse(houtsNode.GetAttribute("balance")));
            }
			string[] args = new string[] { mVCoin.ToString(), mHouts.ToString() };
            SendNotification(GameFacade.RECV_USER_BALANCE, args);
        }

        /// <summary>
        /// Sends popup notification if there are were errors
        /// </summary>
        /// <param name="xmlResponse"><Response noun="HangoutUsers" verb="PurchaseItems"><errors><error code="270007"><message>Error msg</message><uuid>sessionId</uuid></error></errors></Response></param>
        /// <returns>True if there are errors, False if there were none</returns>
        private bool DisplayErrorDialog(XmlDocument xmlResponse)
        {
            XmlNodeList errors = xmlResponse.SelectNodes("/Response/errors/error");
            if (errors.Count > 0)
            {
                // Just show the first error for now.  Not sure if we can have more than one
                string errorCode = ((XmlElement)errors[0]).GetAttribute("code");
                string errorText = StoreErrorMessage.Instance.GetError(errorCode);

				Debug.LogError(errorText);
				
                List<object> args = new List<object>();
                args.Add(Translation.PURCHASE_ERROR_TITLE);
                args.Add(errorText);
                GameFacade.Instance.SendNotification(GameFacade.SHOW_DIALOG, args);

                EventLogger.Log(LogGlobals.CATEGORY_SHOPPING, LogGlobals.PURCHASE_FAILED);
                return true;
            }
            return false;
        }

        private void DisplaySuccessDialog(XmlDocument xmlResponse)
        {
            XmlNode purchasedOfferNode = xmlResponse.SelectSingleNode("Response/purchaseResults/purchaseResult/offer");
            XmlNode purchasedItemNode = purchasedOfferNode.SelectSingleNode("item");
            string itemType = XmlUtility.GetStringAttribute(purchasedItemNode, "itemTypeName");

			AnimationProxy animationProxy = GameFacade.Instance.RetrieveProxy<AnimationProxy>();
			
            switch (itemType)
            {
				case "Emote":
					XmlNode assetInfoNode = purchasedItemNode.SelectSingleNode("Assets/Asset");
                    AssetInfo assetInfo = new ClientAssetInfo(assetInfoNode);
					string animationName = purchasedItemNode.SelectSingleNode("Assets/Asset/AssetData/AnimationName").InnerText;
					RigAnimationName rigAnimationName = (RigAnimationName)Enum.Parse(typeof(RigAnimationName), animationName);
					
					animationProxy.SetRigAnimationAssetInfo(rigAnimationName, assetInfo);
					animationProxy.SetOwnedEmote(rigAnimationName);
					break;
				case "Mood":
					List<AssetInfo> moodInfos = new List<AssetInfo>();
					foreach (XmlNode moodInfoNode in purchasedItemNode.SelectNodes("Assets/Asset"))
					{
						AssetInfo moodInfo = new ClientAssetInfo(moodInfoNode);
						moodInfos.Add(moodInfo);
					}
					string moodName = XmlUtility.GetStringAttribute(purchasedItemNode, "buttonName");
					moodName = moodName.Split(' ')[0];
					MoodAnimation moodAnimationName = (MoodAnimation)Enum.Parse(typeof(MoodAnimation), moodName);

					animationProxy.SetMoodAnimationAssetInfo(moodAnimationName, moodInfos);
					animationProxy.SetOwnedMood(moodAnimationName);
					break;
				case "Emoticon":
					XmlNode emoticonInfoNode = purchasedItemNode.SelectSingleNode("Assets/Asset");
					if (emoticonInfoNode == null)
					{
						Debug.LogError("EmoticonInfoNode was null: " + purchasedItemNode.OuterXml);
						break;
					}
					AssetInfo emoticonInfo = new ClientAssetInfo(emoticonInfoNode);
					animationProxy.SetEmoticonAssetInfo(emoticonInfo);
					animationProxy.SetOwnedEmoticon(emoticonInfo.Path);
					break;
				default:
					Console.Log("Successful purchase of item type: " + itemType + ".  No action taken.");
					break;
            }

            // Parse out the price of the item and add it to the log
            XmlElement priceNode = (XmlElement)purchasedOfferNode.SelectSingleNode("price/money");
            string currency = priceNode.GetAttribute("currencyName");
            string price = priceNode.GetAttribute("amount");
            string itemName = ((XmlElement)purchasedOfferNode).GetAttribute("name");

            LogPurchaseSuccess(itemName, price, currency);

        }

        private void LogPurchaseSuccess(string itemName, string price, string currency)
        {
            string extraProps = "{\"item\":\"" + itemName + "\", \"price\":" + price + "}";
            // Log for metrics
            if (currency == "HOUTS")
            {
                EventLogger.Log(LogGlobals.CATEGORY_SHOPPING, LogGlobals.CASH_PURCHASE_SUCCESS, extraProps);
            }
            else if (currency == "VCOIN")
            {
                EventLogger.Log(LogGlobals.CATEGORY_SHOPPING, LogGlobals.COIN_PURCHASE_SUCCESS, extraProps);
            }
            // Mix panel funnel metrics
            string currencyProps = "{\"currency\":\"" + currency + "\"}";
            FunnelGlobals.Instance.LogFunnel(FunnelGlobals.FUNNEL_PURCHASE, FunnelGlobals.PURCHASE_COMPLETE, currencyProps);
        }
	}
}
