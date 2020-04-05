/**  --------------------------------------------------------  *
 *   TweakablesHandler.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/08/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using Hangout.Shared;

namespace Hangout.Client
{
	public class TweakablesHandler 
	{
		/// Only used in Editor
		private FileWatcher mWatcher = null;
		
		private object mTweakedObject;
		
		public TweakablesHandler(string path, object objectWithTweakableFields) 
		{
			if( path == null ) 
			{
				throw new ArgumentNullException("path");
			}
			
			if( objectWithTweakableFields == null ) 
			{
				throw new ArgumentNullException("objectWithTweakableFields");
			}
			
			mTweakedObject = objectWithTweakableFields;
			
			if( Application.isEditor ) 
			{
				string pathInResources = ProtocolUtility.SplitProtocol(path).Second;
				FileInfo fileInfo = new FileInfo(Application.dataPath + "/Resources/" + pathInResources + ".xml");
				
				if( !fileInfo.Exists ) 
				{
					throw new FileNotFoundException(fileInfo.FullName);
				}
				
				mWatcher = new FileWatcher(fileInfo);
				
				mWatcher.Changed += delegate() 
				{
					XmlDocument doc = new XmlDocument(); 
					doc.Load(fileInfo.FullName);
					LoadTweakables(doc);
				};
			}
			
			XmlDocument resourcesDoc = XmlUtility.LoadXmlDocument(path);
			LoadTweakables(resourcesDoc);
		}
		
		private object Parse(System.Type toType, string fromString) 
		{
			object result = null;
			
			if( toType == typeof(float) )
			{
				result = float.Parse(fromString);
			}
			else if( toType == typeof(int) )
			{
				result = int.Parse(fromString);
			}
			else if( toType == typeof(string) )
			{
				result = fromString;
			}
			else if( toType == typeof(Color) )
			{
				result = ColorUtility.HexToColor(fromString);	
			}
            else if (toType == typeof(long))
            {
                result = long.Parse(fromString);
            }
            else if (toType == typeof(AvatarId))
            {
                result = new AvatarId(uint.Parse(fromString));
            }
            else
            {
                throw new Exception("There is no Parse function available for a " + toType.Name + " tweakable.");
            }
			
			return result;
		}
		
		private void LoadTweakables(XmlDocument doc) 
		{
			// Scan the XML data and save all the names of the tweakables with their values
			Dictionary<string, string> tweakableData = new Dictionary<string, string>(); // key = tweakable name, value = tweakable value
			foreach( XmlNode node in doc.SelectNodes("//Tweakable") ) 
			{
				if( node.Attributes["name"] == null ) 
				{
					throw new Exception("Unable to find 'name' attribute in Tweakable from Xml file");
				}
				
				string tweakableName = node.Attributes["name"].InnerText;
				if( tweakableData.ContainsKey(tweakableName) ) 
				{
					throw new Exception("Tweakable (" + tweakableName + ") appears twice in the same file");
				}
				
				if( node.Attributes["value"] == null ) 
				{
					throw new Exception("Unable to find 'value' attribute in Tweakable from Xml file");
				}
				tweakableData.Add(tweakableName, node.Attributes["value"].InnerText);
			}
			
			foreach( FieldInfo fieldInfo in mTweakedObject.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly) ) 
			{
				foreach(Attribute attrib in Attribute.GetCustomAttributes(fieldInfo)) 
				{
					if( attrib is Tweakable ) 
					{
						string tweakValue;
						if (tweakableData.TryGetValue(((Tweakable)attrib).Name, out tweakValue))
						{
							object parsedValue = Parse(fieldInfo.FieldType, tweakValue);
							fieldInfo.SetValue(mTweakedObject, parsedValue);
						}
						else
						{
							throw new Exception("No Tweakable attribute was found with the name " + ((Tweakable)attrib).Name);
						}
					}
				}
			}
		}
	}
}
