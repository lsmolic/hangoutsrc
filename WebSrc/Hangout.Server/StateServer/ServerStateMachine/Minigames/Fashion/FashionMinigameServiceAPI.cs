/**  --------------------------------------------------------  *
 *   FashionMinigameServiceAPI.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/26/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using Hangout.Shared;
using Hangout.Shared.FashionGame;
using log4net;

namespace Hangout.Server
{
	public static class FashionMinigameServiceAPI
	{
		private static ILog mLogger = LogManager.GetLogger("FashionMinigameServiceAPI");
		private const int FASHION_MINIGAME_SERVICE_ID = 1;
		private const string FASHION_MINIGAME_NAME = "";
		private readonly static string mWebServicesBaseUrl = ConfigurationSettings.AppSettings["WebServicesBaseUrl"];

		private static Action<ServerAccount, string[], Action<XmlDocument>> mMockGetGameData = null;
		public static IReceipt MockGetGameData(Action<ServerAccount, string[], Action<XmlDocument>> replacementGetGameDataFunc)
		{
			mMockGetGameData = replacementGetGameDataFunc;
			return new Receipt(delegate()
			{
				mMockGetGameData = null;
			});
		}

		private static Action<ServerAccount, string, string, Action<XmlDocument>> mMockSetGameData = null;
		public static IReceipt MockSetGameData(Action<ServerAccount, string, string, Action<XmlDocument>> replacementSetGameDataFunc)
		{
			mMockSetGameData = replacementSetGameDataFunc;
			return new Receipt(delegate()
			{
				mMockSetGameData = null;
			});
		}

		public static void GetGameData(ServerAccount serverAccount, string dataKey, Action<XmlDocument> result)
		{
			GetGameData(serverAccount, new string[] { dataKey }, result);
		}

		public static void GetGameData(ServerAccount serverAccount, string[] dataKeys, Action<XmlDocument> result)
		{
			if (mMockGetGameData == null)
			{
				string dataKeysCsv = String.Join(",", dataKeys);
				mLogger.DebugFormat("FashionMinigameServiceAPI - MiniGames.GetMiniGameUserData called accountId={0} dataKeys={1}", serverAccount.AccountId, dataKeysCsv);
				WebServiceRequest setDataService = new WebServiceRequest(mWebServicesBaseUrl, "MiniGames", "GetMiniGameUserData");
				setDataService.Method = FormMethod.POST;
				setDataService.AddParam("accountId", serverAccount.AccountId.ToString());
				setDataService.AddParam("miniGameId", FASHION_MINIGAME_SERVICE_ID.ToString());
				setDataService.AddParam("miniGameName", FASHION_MINIGAME_NAME);

				setDataService.AddParam("dataKeyCSVList", dataKeysCsv);

				setDataService.GetWebResponseAsync(delegate(XmlDocument response)
				{
					mLogger.DebugFormat("FashionMinigameServiceAPI - MiniGames.GetMiniGameUserData responded accountId={0} xmlResponse={1}", serverAccount.AccountId, response.OuterXml);
					result(response);
				});
			}
			else
			{
				mMockGetGameData(serverAccount, dataKeys, result);
			}
		}

		public static void SetGameData(ServerAccount serverAccount, string dataKey, object dataValue, Action<XmlDocument> result)
		{
			if (mMockSetGameData == null)
			{
				mLogger.DebugFormat("FashionMinigameServiceAPI - MiniGames.SaveMiniGameUserData called accountId={0} dataKey={1} dataValue={2}", serverAccount.AccountId, dataKey, dataValue.ToString());
				WebServiceRequest setDataService = new WebServiceRequest(mWebServicesBaseUrl, "MiniGames", "SaveMiniGameUserData");
				setDataService.Method = FormMethod.POST;
				setDataService.AddParam("accountId", serverAccount.AccountId.ToString());
				setDataService.AddParam("miniGameId", FASHION_MINIGAME_SERVICE_ID.ToString());
				setDataService.AddParam("miniGameName", FASHION_MINIGAME_NAME);
				setDataService.AddParam("dataKey", dataKey);
				setDataService.AddParam("dataValue", dataValue.ToString());

				setDataService.GetWebResponseAsync(delegate(XmlDocument xmlResponse) 
				{
					mLogger.DebugFormat("FashionMinigameServiceAPI - MiniGames.SaveMiniGameUserData responded accountId={0} xmlResponse={1}", serverAccount.AccountId, xmlResponse.OuterXml);
					result(xmlResponse);
				});
			}
			else
			{
				mMockSetGameData(serverAccount, dataKey, dataValue.ToString(), result);
			}
		}

		/// <summary>
		/// Result is a Collection of Facebook IDs for the friends the user has hired
		/// </summary>
		public static void GetAllHiredFriends(ServerAccount serverAccount, Action<ICollection<long>> result)
		{
			GetHiredFriendsForJob(serverAccount, (IEnumerable<Jobs>)Enum.GetValues(typeof(Jobs)), result);
		}

		/// <summary>
		/// Returns the full XML for the hired friends
		/// </summary>
		public static void GetAllHiredFriends(ServerAccount serverAccount, Action<XmlDocument> result)
		{
			GetGameData(serverAccount, AllJobsCsv, result);
		}

		private static string AllJobsCsv
		{
			get
			{
				return Functionals.Reduce<string, string>
				(
					delegate(string a, string b)
					{
						if (!String.IsNullOrEmpty(a))
						{
							return String.Format("{0},{1}", a, b);
						}
						return b;
					},
					Functionals.Map<string>
					(
						delegate(object enumObj)
						{
							return enumObj.ToString();
						},
						Enum.GetValues(typeof(Jobs))
					)
				);
			}
		}

		public static void GetHiredFriendsForJob(ServerAccount serverAccount, Jobs job, Action<ICollection<long>> result)
		{
			GetHiredFriendsForJob(serverAccount, new Jobs[]{job}, result);
		}

		public static void GetHiredFriendsForJob(ServerAccount serverAccount, IEnumerable<Jobs> jobs, Action<ICollection<long>> result)
		{
			List<string> jobNames = new List<string>();
			foreach(Jobs job in jobs)
			{
				jobNames.Add(job.ToString());
			}

			GetGameData(serverAccount, jobNames.ToArray(), delegate(XmlDocument hiredFriendsXml)
			{
				List<long> resultFriends = new List<long>();
				foreach(string jobName in jobNames)
				{
					XmlNode hiredFriendsListNode = hiredFriendsXml.SelectSingleNode
					(
						"//DataKey[@KeyName='" + jobName + "']"
					);

					if( hiredFriendsListNode == null )
					{
						// No friends for this Job
						continue;
					}

					foreach(string hiredFriendId in hiredFriendsListNode.InnerText.Split(','))
					{
						resultFriends.Add(long.Parse(hiredFriendId));
					}
				}

				result(resultFriends);
			});
		}
	}
}
