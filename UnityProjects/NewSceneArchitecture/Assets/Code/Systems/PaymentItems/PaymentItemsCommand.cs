using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;
using UnityEngine;

namespace Hangout.Client
{
	public class PaymentItemsCommand
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="commandName"></param>
		/// <param name=""></param>
		/// <param name="commandArgs"></param>
		/// <returns></returns>
		public string CreatePaymentCommand(string commandName, Dictionary<string, string> commandArgs)
		{
			string command = "";

			PaymentCommand paymentCommand = null;

			switch (commandName)
			{
				case "GetUserBalance":
					paymentCommand = CreateSimpleCommand("GetUserBalance", commandArgs);
					break;

				case "AddVirtualCoinForUser":
					paymentCommand = AddVirtualCoinForUser(commandArgs);
					break;

				case "GetUserInventory":
                    paymentCommand = GetUserInventory(commandArgs);
					break;

				case "GetStoreInventory":
					paymentCommand = GetHangoutStoreInventory(commandArgs);
					break;

				case "PurchaseItems":
					paymentCommand = PurchaseItems(commandArgs);
					break;

				case "PurchaseItemsGift":
					paymentCommand = PurchaseItemsGift(commandArgs);
					break;

				case "GameCurrencyOffers":
					paymentCommand = CreateSimpleCommand("PurchaseOffers", commandArgs);
					break;

				case "PurchaseGameCurrencyPayPal":
					paymentCommand = PurchaseGameCurrencyPayPal(commandArgs);
					break;

				case "PurchaseGameCurrencyCreditCard":
					paymentCommand = PurchaseGameCurrencyCreditCard(commandArgs);
					break;

                case "SecurePaymentInfo":
                    paymentCommand = CreateCommandWithNoParams("SecurePaymentInfo");
                    break;

                default:
					throw (new Exception("Invalid Payment Items Command"));
			}

			ServiceCommandSerializer serializer = new ServiceCommandSerializer();
			command = serializer.SerializeCommandData(paymentCommand, typeof(PaymentCommand));

			return command;
		}

		public void ResponseHandler(string response)
		{
			//Debug.Log(String.Format("PaymentItems Response: {0}\n\n", response));

			try
			{
				XmlDocument xmlResponse = new XmlDocument();
				xmlResponse.LoadXml(response);

				string noun = xmlResponse.SelectSingleNode("/Response").Attributes["noun"].InnerText;
				string verb = xmlResponse.SelectSingleNode("/Response").Attributes["verb"].InnerText;

				ParseResponse(noun, verb, xmlResponse);
			}
			catch (Exception ex)
			{
				Console.LogError(String.Format("Error ResponseHandler:\n{0}\n\n", ex));
				throw ex;
			}
		}

		private void ParseResponse(string commandNoun, string commandVerb, XmlDocument xmlResponse)
		{
			switch (commandVerb)
			{
				case "PurchaseGameCurrencyPayPal":
					string payPalURL = xmlResponse.SelectSingleNode("/Response/paypalURL").InnerText;
					System.Diagnostics.Process.Start(payPalURL);
					break;
			}
		}

		private string CreateErrorDoc(string message)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("<Error><Message>" + message + "</Message></Error>");
			return sb.ToString();
		}

		private PaymentCommand CreateSimpleCommand(string commandName, Dictionary<string, string> commandArgs)
		{
			PaymentCommand cmd = new PaymentCommand();
			cmd.Verb = commandName;

			cmd.Parameters.Add("userSession", GetStringValueFromCommandArgs("userSession", commandArgs));
			cmd.Parameters.Add("startIndex", GetStringValueFromCommandArgs("startIndex", commandArgs));
			cmd.Parameters.Add("blockSize", GetStringValueFromCommandArgs("blockSize", commandArgs));

			return cmd;
		}

		private PaymentCommand AddVirtualCoinForUser(Dictionary<string, string> commandArgs)
		{
			PaymentCommand cmd = new PaymentCommand();
			cmd.Verb = "AddVirtualCoinForUser";

			cmd.Parameters.Add("userSession", GetStringValueFromCommandArgs("userSession", commandArgs));
			cmd.Parameters.Add("amount", GetStringValueFromCommandArgs("amount", commandArgs));
			cmd.Parameters.Add("ipAddress", GetMyIpAddress());

			return cmd;
		}

        private PaymentCommand GetUserInventory(Dictionary<string, string> commandArgs)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Verb = "GetUserInventory";

            cmd.Parameters.Add("userSession", GetStringValueFromCommandArgs("userSession", commandArgs));
            cmd.Parameters.Add("startIndex", GetStringValueFromCommandArgs("startIndex", commandArgs));
            cmd.Parameters.Add("blockSize", GetStringValueFromCommandArgs("blockSize", commandArgs));
            cmd.Parameters.Add("itemTypeNames", GetStringValueFromCommandArgs("itemTypeNames", commandArgs));
            return cmd;
        }

		private PaymentCommand GetHangoutStoreInventory(Dictionary<string, string> commandArgs)
		{
			PaymentCommand cmd = new PaymentCommand();
			cmd.Verb = "GetStoreInventory";

			cmd.Parameters.Add("storeName", GetStringValueFromCommandArgs("storeName", commandArgs));
			cmd.Parameters.Add("itemTypeNames", GetStringValueFromCommandArgs("itemTypeNames", commandArgs));
			cmd.Parameters.Add("startIndex", GetStringValueFromCommandArgs("startIndex", commandArgs));
			cmd.Parameters.Add("blockSize", GetStringValueFromCommandArgs("blockSize", commandArgs));
			//cmd.Parameters.Add("filter", "itemOffer_title Like '%basic%'");
			//cmd.Parameters.Add("orderby", "itemOffer_endDate desc, item_itemTypeName asc");
			return cmd;
		}

		private PaymentCommand PurchaseItems(Dictionary<string, string> commandArgs)
		{
			PaymentCommand cmd = new PaymentCommand();
			cmd.Verb = "PurchaseItems";

			cmd.Parameters.Add("userSession", GetStringValueFromCommandArgs("userSession", commandArgs));
			cmd.Parameters.Add("currencyName", GetStringValueFromCommandArgs("currencyName", commandArgs));
			cmd.Parameters.Add("offerIds", GetStringValueFromCommandArgs("offerIds", commandArgs));
			cmd.Parameters.Add("ipAddress", GetMyIpAddress());
			;

			return cmd;
		}


		private PaymentCommand PurchaseItemsGift(Dictionary<string, string> commandArgs)
		{
			PaymentCommand cmd = new PaymentCommand();
			cmd.Verb = "PurchaseItemsGift";

			cmd.Parameters.Add("userSession", GetStringValueFromCommandArgs("userSession", commandArgs));
			cmd.Parameters.Add("currencyName", GetStringValueFromCommandArgs("currencyName", commandArgs));
			cmd.Parameters.Add("offerIds", GetStringValueFromCommandArgs("offerIds", commandArgs));
			cmd.Parameters.Add("recipientUserId", GetStringValueFromCommandArgs("recipientUserId", commandArgs));
			cmd.Parameters.Add("noteToRecipient", GetStringValueFromCommandArgs("noteToRecipient", commandArgs));
			cmd.Parameters.Add("ipAddress", GetMyIpAddress());

			return cmd;
		}

		private PaymentCommand PurchaseGameCurrencyPayPal(Dictionary<string, string> commandArgs)
		{
			PaymentCommand cmd = new PaymentCommand();
			cmd.Verb = "PurchaseGameCurrencyPayPal";

			cmd.Parameters.Add("userSession", GetStringValueFromCommandArgs("userSession", commandArgs));
			cmd.Parameters.Add("offerId", GetStringValueFromCommandArgs("offerId", commandArgs));
			cmd.Parameters.Add("ipAddress", GetMyIpAddress());

			return cmd;
		}

		private PaymentCommand PurchaseGameCurrencyCreditCard(Dictionary<string, string> commandArgs)
		{
			PaymentCommand cmd = new PaymentCommand();
			cmd.Verb = "PurchaseGameCurrencyCreditCard";

			cmd.Parameters.Add("userSession", GetStringValueFromCommandArgs("userSession", commandArgs));
			cmd.Parameters.Add("offerId", GetStringValueFromCommandArgs("offerId", commandArgs));
			cmd.Parameters.Add("ipAddress", GetMyIpAddress());
			cmd.Parameters.Add("creditCardNumber", GetStringValueFromCommandArgs("creditCardNumber", commandArgs));
			cmd.Parameters.Add("creditCardType", GetStringValueFromCommandArgs("creditCardType", commandArgs));
			cmd.Parameters.Add("expireDate", GetStringValueFromCommandArgs("expireDate", commandArgs));
			cmd.Parameters.Add("securityCode", GetStringValueFromCommandArgs("securityCode", commandArgs));
			cmd.Parameters.Add("firstName", GetStringValueFromCommandArgs("firstName", commandArgs));
			cmd.Parameters.Add("lastName", GetStringValueFromCommandArgs("lastName", commandArgs));
			cmd.Parameters.Add("address", GetStringValueFromCommandArgs("address", commandArgs));
			cmd.Parameters.Add("city", GetStringValueFromCommandArgs("city", commandArgs));
			cmd.Parameters.Add("state", GetStringValueFromCommandArgs("state", commandArgs));
			cmd.Parameters.Add("zipCode", GetStringValueFromCommandArgs("zipCode", commandArgs));
			cmd.Parameters.Add("countryCode", GetStringValueFromCommandArgs("countryCode", commandArgs));
			cmd.Parameters.Add("phoneNumber", GetStringValueFromCommandArgs("phoneNumber", commandArgs));

			return cmd;
		}

        private PaymentCommand CreateCommandWithNoParams(string commandName)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Verb = commandName;

            return cmd;
        }
        
        private string GetStringValueFromCommandArgs(string key, Dictionary<string, string> commandArgs)
		{
			string value = "";

			if (!commandArgs.TryGetValue(key, out value))
			{
				//throw (new Exception(String.Format("Argument {0} not found", key)));
			}

			return value;
		}

		private string GetMyIpAddress()
		{
            /*
			string ipAddress = "";
			try
			{
				IPAddress[] ipAddrList = Dns.GetHostAddresses(Dns.GetHostName());
				if (ipAddrList.Length > 0)
				{
					ipAddress = ipAddrList[0].ToString();
				}
			}
			catch { }
			return ipAddress;
            */
            return "127.0.0.1";
        }
	}
}
