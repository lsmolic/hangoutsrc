using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Xml.Serialization;
using System.Configuration;
using System.Data.SqlClient;
using System.Xml;
using Hangout.Server;
using Hangout.Shared;
using Affirma.ThreeSharp.Wrapper;
using System.Threading;

namespace Hangout.Server.WebServices 
{
	public class FileStoreService : ServiceBaseClass
	{
		private ServicesLog mServiceLog = new ServicesLog("FileStoreService");

		public SimpleResponse CreateBucket(string bucketName)
		{
			StringBuilder xmlResponse = new StringBuilder();
			ThreeSharpWrapper amazon = new ThreeSharpWrapper(WebConfig.AmazonAccessKeyId, WebConfig.AmazonSecretAccessKey);
			if(!String.IsNullOrEmpty(bucketName))
			{
				amazon.AddBucket(bucketName);
			}
			return new SimpleResponse("Success", "true");
		}

		/// <summary>
		/// Uploads a file to AmazonS3 then returns back the full Url for that file
		/// </summary>
		/// <returns></returns>
		public SimpleResponse AddFileToAmazon(string fileName, HangoutPostedFile fileToUpload)
		{
			ThreeSharpWrapper amazon = new ThreeSharpWrapper(WebConfig.AmazonAccessKeyId, WebConfig.AmazonSecretAccessKey);
			amazon.AddPublicFileObject(WebConfig.AmazonS3BucketName, fileName, fileToUpload.InputStream);

			return new SimpleResponse("Success", "true");
		}

		public SimpleResponse SyncBuckets(string sourceBucketName, string destinationBucketName)
		{
			if(String.IsNullOrEmpty(sourceBucketName) || String.IsNullOrEmpty(destinationBucketName))
			{
				throw new NullReferenceException("You must provide a source and destination bucket name! ");
			}
			ThreeSharpWrapper amazon = new ThreeSharpWrapper(WebConfig.AmazonAccessKeyId, WebConfig.AmazonSecretAccessKey);
			string bucketContents = amazon.ListBucket(WebConfig.AmazonS3BucketName);
			if(!String.IsNullOrEmpty(bucketContents))
			{
				XmlDocument contentXml = new XmlDocument();
				contentXml.LoadXml(bucketContents);

				//xmlns="http://s3.amazonaws.com/doc/2006-03-01/
				XmlNamespaceManager nsMgr = new XmlNamespaceManager(contentXml.NameTable);
				nsMgr.AddNamespace("aws", "http://s3.amazonaws.com/doc/2006-03-01/");



				XmlNodeList contentsNodeList = contentXml.SelectNodes("//aws:Contents", nsMgr);
				foreach (XmlNode contentNode in contentsNodeList)
				{
					string key = "";
					string timestamp = "";
					string eTag = "";
					XmlNode keyNode = contentNode.SelectSingleNode(".//aws:Key", nsMgr);
					if(keyNode != null)
					{
						key = keyNode.InnerText;
					}
					XmlNode timestampNode = contentNode.SelectSingleNode(".//aws:LastModified", nsMgr);
					if (timestampNode != null)
					{
						timestamp = timestampNode.InnerText;
					}
					XmlNode eTagNode = contentNode.SelectSingleNode(".//aws:ETag", nsMgr);
					if (eTagNode != null)
					{
						eTag = eTagNode.InnerText;
					}
					amazon.CopyObject(sourceBucketName, key, timestamp, eTag, destinationBucketName, key);
					System.Threading.Thread.Sleep(5000);
				}		
			}
			
			return new SimpleResponse("Success", "true");
		}

		public SimpleResponse DeleteFileStoredAtAmazon(string fileName)
		{
			ThreeSharpWrapper amazon = new ThreeSharpWrapper(WebConfig.AmazonAccessKeyId, WebConfig.AmazonSecretAccessKey);
			amazon.DeleteObject(WebConfig.AmazonS3BucketName, fileName);
			return new SimpleResponse("Success", "true");
		}

	}
		
}
