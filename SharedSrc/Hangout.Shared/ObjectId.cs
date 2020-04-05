using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;

namespace Hangout.Shared
{
    [Serializable]
    [XmlRoot(ElementName="ItemId",Namespace="")]
	public abstract class ObjectId : Object, ISerializable, IXmlSerializable
    {
        
        protected uint mIdValue;

        protected abstract string SerializationString { get; }


        public uint Value
        {
            get { return mIdValue; }
        }

        public override string ToString()
        {
            return mIdValue.ToString();
        }

        //TODO: make this happy!!! this is not happy because it can be 
        //all kinds of runtime errors... but it is needed for XMLSerialization
        public ObjectId() { } 

        public ObjectId(ObjectId copyObject)
        {
            mIdValue = copyObject.Value;
        }

		public ObjectId(string idString)
		{
			mIdValue = Convert.ToUInt32(idString);
		}

        public ObjectId(uint idValue)
        {
            mIdValue = idValue;
        }

        public ObjectId(SerializationInfo info, StreamingContext context)
		{
            mIdValue = info.GetUInt32(SerializationString);
		}

        public override bool Equals(object obj)
        {
			if (obj == null)
			{
				return false;
			}

            if (obj.GetType() == this.GetType())
            {
                return ((ObjectId)obj).Value == mIdValue;
            }
			else if(obj is uint)
			{
				return mIdValue == (uint)obj;
			}
			else if (obj is int)
			{
				return mIdValue == (int)obj;
			}
			return false;
        }

		public static bool operator==(ObjectId left, ObjectId right)
		{
			bool result;
			if ((object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null)) && !(object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null)))
			{
				result = false;
			}
			else if(object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null))
			{
				result = true;
			}
			else
			{
				result = left.mIdValue == right.mIdValue;
			}
			return result;
		}

		public static bool operator !=(ObjectId left, ObjectId right)
		{
			bool result;
			if ((object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null)) && !(object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null)))
			{
				result = true;
			}
			else if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null))
			{
				result = false;
			}
			else
			{
				result = left.mIdValue != right.mIdValue;
			}
			return result;
		}

        public override int GetHashCode()
        {
            return mIdValue.GetHashCode();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(SerializationString, mIdValue);
        }


        #region IXmlSerializable Members

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public virtual void ReadXml(System.Xml.XmlReader reader)
		{
			bool wasEmpty = reader.IsEmptyElement;

			reader.Read();

			if (wasEmpty)
			{
				return;
			}
			XmlSerializer serializer = new XmlSerializer(typeof(uint));
			mIdValue = (uint)serializer.Deserialize(reader);


			reader.ReadEndElement();
		}

		public virtual void WriteXml(System.Xml.XmlWriter writer)
		{
			XmlSerializer serializer = new XmlSerializer(Value.GetType());
			XmlSerializerNamespaces nameSpace = new XmlSerializerNamespaces();
			nameSpace.Add("", "");
			try
			{
				serializer.Serialize(writer, Value, nameSpace);
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
    }
}
