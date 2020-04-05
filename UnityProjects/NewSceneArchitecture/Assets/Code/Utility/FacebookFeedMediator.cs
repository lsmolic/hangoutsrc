/**  --------------------------------------------------------  *
 *   FacebookFeed.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 11/05/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using System.Xml;

using Hangout.Client.Gui;
using Hangout.Shared;
using Hangout.Shared.FashionGame;

using PureMVC.Patterns;

using UnityEngine;

namespace Hangout.Client
{
	public class FacebookFeedMediator : Mediator
	{
		private readonly IScheduler mScheduler;
		private readonly ClientAssetRepository mAssetRepo;
		private TaskCollection mTaskCollection = null;
		private readonly JSDispatcher mJavascriptDispatch = new JSDispatcher();
	
		public FacebookFeedMediator()
		{
			mScheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			mAssetRepo = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
		}
	
		/// <summary>
		/// Loads and sends a feed from the given path and name. Javascript will hide unity until the feed
		/// is taken care of and then when Unity is shown again, onFeedSent will be executed.
		/// </summary>
		public void PostFeed(long? targetId, string feedCopyDataPath, string feedDataName, Hangout.Shared.Action onFeedSent, string param)
		{
			if( String.IsNullOrEmpty(feedCopyDataPath) )
			{
				throw new ArgumentNullException("feedCopyDataPath");
			}
			if (String.IsNullOrEmpty(feedDataName))
			{
				throw new ArgumentNullException("feedDataName");
			}
			if (onFeedSent == null)
			{
				throw new ArgumentNullException("onFeedSent");
			}

			// Log the post
			ConnectionProxy connectionProxy = GameFacade.Instance.RetrieveProxy<ConnectionProxy>();
			long fromId = connectionProxy.FacebookAccountId;
			string extraProps = String.Format("{0}|{1}", fromId, targetId);
			EventLogger.Log(LogGlobals.CATEGORY_FACEBOOK, LogGlobals.FACEBOOK_FEED_POST, feedDataName, extraProps);

			mAssetRepo.LoadAssetFromPath<XmlAsset>(feedCopyDataPath, delegate(XmlAsset asset)
			{
				mTaskCollection.Add(mScheduler.StartCoroutine(SendFeedJavascript(targetId, asset.XmlDocument, feedDataName, onFeedSent, param)));
			});
		}

		private IEnumerator<IYieldInstruction> SendFeedJavascript(long? targetId, XmlDocument xml, string feedDataName, Hangout.Shared.Action onFeedSent, string param)
		{	
			XmlNode feedDataNode = xml.SelectSingleNode("//FeedData[@name='" + feedDataName + "']");
			if( feedDataNode == null )
			{
				throw new Exception("Unable to find a FeedData node named (" + feedDataName + ")\n" + xml.OuterXml);
			}

			// User Message
			XmlNode defaultUserMessage = feedDataNode.SelectSingleNode("DefaultUserMessage");
			if( defaultUserMessage == null )
			{
				throw new Exception("Unable to find node (DefaultUserMessage) in FeedData:\n" + feedDataNode.OuterXml);
			}

			// User Message
			XmlNode userMessagePrompt = feedDataNode.SelectSingleNode("UserMessagePrompt");
			if (userMessagePrompt == null)
			{
				throw new Exception("Unable to find node (UserMessagePrompt) in FeedData:\n" + feedDataNode.OuterXml);
			}

			// Action
			XmlNode actionName = feedDataNode.SelectSingleNode("Action/Name");
			if (actionName == null)
			{
				throw new Exception("Unable to find node (Action/Name) in FeedData:\n" + feedDataNode.OuterXml);
			}

			XmlNode actionUrl = feedDataNode.SelectSingleNode("Action/Url");
			if (actionUrl == null)
			{
				throw new Exception("Unable to find node (Action/Url) in FeedData:\n" + feedDataNode.OuterXml);
			}
			
			// Attachment
			XmlNode attachmentName = feedDataNode.SelectSingleNode("Attachment/Name");
			if (attachmentName == null)
			{
				throw new Exception("Unable to find node (Attachment/Name) in FeedData:\n" + feedDataNode.OuterXml);
			}

			XmlNode attachmentUrl = feedDataNode.SelectSingleNode("Attachment/Url");
			if (attachmentUrl == null)
			{
				throw new Exception("Unable to find node (Attachment/Url) in FeedData:\n" + feedDataNode.OuterXml);
			}

			XmlNode attachmentCaption = feedDataNode.SelectSingleNode("Attachment/Caption");
			if (attachmentCaption == null)
			{
				throw new Exception("Unable to find node (Attachment/Caption) in FeedData:\n" + feedDataNode.OuterXml);
			}

			XmlNode attachmentDescription = feedDataNode.SelectSingleNode("Attachment/Description");
			if (attachmentDescription == null)
			{
				throw new Exception("Unable to find node (Attachment/Description) in FeedData:\n" + feedDataNode.OuterXml);
			}

			XmlNode attachmentImgSrcUrl = feedDataNode.SelectSingleNode("Attachment/ImageSrcUrl");
			if (attachmentImgSrcUrl == null)
			{
				throw new Exception("Unable to find node (Attachment/ImageSrcUrl) in FeedData:\n" + feedDataNode.OuterXml);
			}

			XmlNode attachmentImgLinkUrl = feedDataNode.SelectSingleNode("Attachment/ImageLinkUrl");
			if (attachmentImgLinkUrl == null)
			{
				throw new Exception("Unable to find node (Attachment/ImageLinkUrl) in FeedData:\n" + feedDataNode.OuterXml);
			}

            string targetIdStr = "";
            if (targetId != null)
            {
                targetIdStr = targetId.ToString();
            }

			ConnectionProxy connectionProxy = GameFacade.Instance.RetrieveProxy<ConnectionProxy>();
			string referrer = connectionProxy.FacebookAccountId.ToString();

			bool jsReturned = false;
			mJavascriptDispatch.RequestFeedPost
			(
				targetIdStr,
                feedDataName,
				userMessagePrompt.InnerText.Replace("{0}", param),
				defaultUserMessage.InnerText.Replace("{0}", param),
				actionName.InnerText.Replace("{0}", param),
				actionUrl.InnerText.Replace("{0}", param).Replace("{1}", referrer),
				attachmentName.InnerText.Replace("{0}", param),
				attachmentUrl.InnerText.Replace("{0}", param).Replace("{1}", referrer),
				attachmentCaption.InnerText.Replace("{0}", param),
				attachmentDescription.InnerText.Replace("{0}", param),
				attachmentImgSrcUrl.InnerText.Replace("{0}", param),
				attachmentImgLinkUrl.InnerText.Replace("{0}", param).Replace("{1}", referrer),
				delegate(string result)
				{
					jsReturned = true;
				}
			);

			if (!Application.isEditor)
			{
				yield return new YieldWhile(delegate() { return !jsReturned; });
			}
			else
			{
				yield return new YieldUntilNextFrame();
			}

			onFeedSent();
		}

		public override void OnRegister()
		{
			base.OnRegister();
			mTaskCollection = new TaskCollection();
		}

		public override void OnRemove()
		{
			base.OnRemove();

			mTaskCollection.Dispose();
		}
	}
}
