using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;

namespace Hangout.Shared
{
	[Serializable]
	public class UserProperties : ISerializable, IXmlSerializable
	{
		private static string kSerializationString = "UserProperties";

		public Dictionary<UserAccountProperties, object> mUserPropertiesDictionary = new Dictionary<UserAccountProperties, object>();

		public override string ToString()
		{
			string returnString = string.Empty;

			foreach(KeyValuePair<UserAccountProperties, object> userProperty in mUserPropertiesDictionary)
			{
				returnString += userProperty.Key.ToString() + "=" + userProperty.Value.ToString() + " | ";
			}

			return returnString;
		}

        public bool TryGetProperty(UserAccountProperties property)
        {
            return mUserPropertiesDictionary.ContainsKey(property);
        }

		public bool TryGetProperty(UserAccountProperties property, out object returnObject)
		{
			returnObject = null;
			if(mUserPropertiesDictionary.TryGetValue(property, out returnObject))
			{
				return true;
			}
			return false;
		}

		public void SetProperty(UserAccountProperties property, object propertyValue)
		{
			mUserPropertiesDictionary[property] = propertyValue;
		}

		#region ISerializable Members

		public UserProperties()
		{
            
		}

		public UserProperties(SerializationInfo info, StreamingContext context)
		{
			string serializedUserPropertyString = (string)info.GetValue(kSerializationString, typeof(string));
			mUserPropertiesDictionary = StringToUserPropertyDictionary(serializedUserPropertyString);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			XmlDocument xmlDocument = null;
			if(UserPropertiesToXml(this, out xmlDocument))
			{
				info.AddValue(kSerializationString, xmlDocument.OuterXml, typeof(string));
			}
		}

		#endregion

		#region IXmlSerializable Members

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public virtual void ReadXml(System.Xml.XmlReader reader)
		{
			if (reader.IsEmptyElement)
			{
				return;
			}
			string deserializedXmlString = reader.ReadOuterXml();
			mUserPropertiesDictionary = StringToUserPropertyDictionary(deserializedXmlString);
		}

		private Dictionary<UserAccountProperties, object> StringToUserPropertyDictionary(string xmlDocumentString)
		{
			Dictionary<UserAccountProperties, object> userPropertyDictionary = new Dictionary<UserAccountProperties, object>();
			XmlDocument deserializedXmlDocument = new XmlDocument();
			try
			{
				deserializedXmlDocument.LoadXml(xmlDocumentString);
			}
			catch (System.Exception ex)
			{
				throw new System.Exception("Error converting string to xml: >" + xmlDocumentString + "<\n"
					, ex);
			}

			XmlNode userPropertiesXmlNode = deserializedXmlDocument.SelectSingleNode(ConstStrings.kUserProperties);
			if(userPropertiesXmlNode != null)
			{
				XmlNodeList userPropertyXmlNodeList = userPropertiesXmlNode.SelectNodes(ConstStrings.kUserProperty);
				foreach(XmlNode userPropertyXmlNode in userPropertyXmlNodeList)
				{
					string nameString = string.Empty;
					string typeString = string.Empty;
					string valueString = string.Empty;

					if ((XmlUtil.TryGetAttributeFromXml(ConstStrings.kName, userPropertyXmlNode, out nameString) &&
						XmlUtil.TryGetAttributeFromXml(ConstStrings.kType, userPropertyXmlNode, out typeString) &&
						XmlUtil.TryGetAttributeFromXml(ConstStrings.kValue, userPropertyXmlNode, out valueString)))
					{
						try
						{
							//try and convert the string name to the UserAccountProperties enum
							UserAccountProperties userAccountProperty = (UserAccountProperties)Enum.Parse(typeof(UserAccountProperties), nameString, false);

							Type typeValue = Type.GetType(typeString);

							object objectValue = null;

							try
							{
								objectValue = Convert.ChangeType(valueString, typeValue);
							}
							catch (System.Exception)
							{
								try
								{
									//look for a constructor on our type that takes a string as a parameter
									System.Reflection.ConstructorInfo typeConstructorInfo = typeValue.GetConstructor(new Type[1] { typeof(string) });
									//call that constructor with our valueString
									objectValue = typeConstructorInfo.Invoke(new object[1] { valueString });
								}
								catch (System.Exception exInner)
								{
									throw new System.Exception("Cannot cast from string to type: " + typeValue.Name + ". Does this type implement a explicit cast operator from a string?", exInner);
								}
							}
							userPropertyDictionary.Add(userAccountProperty, objectValue);
						}
						catch (System.Exception)
						{
							continue;
						}
					}
					else
					{
						continue;
					}
				}
			}
			return userPropertyDictionary;
		}
		
		public virtual void WriteXml(System.Xml.XmlWriter writer)
		{
			try
			{
				foreach (KeyValuePair<UserAccountProperties, object> userProperty in mUserPropertiesDictionary)
				{
					writer.WriteStartElement(ConstStrings.kUserProperty);
						writer.WriteAttributeString(ConstStrings.kName, userProperty.Key.ToString());
						Type objectType = userProperty.Value.GetType();
						writer.WriteAttributeString(ConstStrings.kType, objectType.FullName + "," + System.Reflection.Assembly.GetAssembly(objectType));
						writer.WriteAttributeString(ConstStrings.kValue, userProperty.Value.ToString());
					writer.WriteEndElement();
				}
			}
			catch (System.Xml.XmlException xmlEx)
			{
				throw xmlEx;
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}

		#endregion

		public static bool UserPropertiesToXml(UserProperties userProperties, out XmlDocument returnXmlDocument)
		{
			returnXmlDocument = new XmlDocument();
			try
			{
				System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

				XmlSerializer serializer = new XmlSerializer(typeof(UserProperties));
				serializer.Serialize(memoryStream, userProperties);

				string xmlString = ASCIIEncoding.UTF8.GetString(memoryStream.ToArray());

				//HACK: similiarly to creating an xml file via right clicking the project and making a new file,
				// when windows creates an xml file, it tags on a hidden character at the beginning of the file.  we have seen this problem before
				// apparently this ALSO happens when creating an xml file with the XmlWriter.. which happens when we Serialize UserProperties to Xml
				// to prevent against this error when loading a string generated from an xml writer, we grab the substring of <UserProperties> ... </UserProperties>
				// this manages to successfully strip out the beginning junk character
				// we should eventually find a better way to do this.. but for now, it does the trick
				int indexOfUserPropertiesTag = xmlString.IndexOf("<" + ConstStrings.kUserProperties + ">");
				xmlString = xmlString.Substring(indexOfUserPropertiesTag, xmlString.Length - indexOfUserPropertiesTag);

				returnXmlDocument.LoadXml(xmlString);

				return true;
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}

		public static bool UserPropertiesFromXml(XmlNode xmlNode, out UserProperties userProperties)
		{
			userProperties = null;
			try
			{
				XmlReader xmlReader = new XmlNodeReader(xmlNode);
				XmlSerializer serializer = new XmlSerializer(typeof(UserProperties));
				userProperties = (UserProperties)serializer.Deserialize(xmlReader);
				return true;
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}
	}
}
