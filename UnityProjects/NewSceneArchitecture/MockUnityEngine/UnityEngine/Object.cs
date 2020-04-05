using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine
{
	public class Object
	{
		private static int mNextInstanceId = 0;
		private int mInstanceId = mNextInstanceId++;

		// Mock data:
		private Dictionary<string, int> m_functionCallCounts;
		public Dictionary<string, int> FunctionCallCounts
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				return m_functionCallCounts;
			}
		}

		public override System.Boolean Equals(System.Object o)
		{
			//Mock Data:
			if (m_functionCallCounts == null)
			{
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if (!m_functionCallCounts.ContainsKey("Boolean Equals(System.Object)"))
			{
				m_functionCallCounts.Add("Boolean Equals(System.Object)", 0);
			}
			m_functionCallCounts["Boolean Equals(System.Object)"]++;

			if( o is UnityEngine.Object && ((UnityEngine.Object)o).GetInstanceID() == this.mInstanceId)
			{
				return true;
			}

			return false;
		}

		public System.Int32 GetInstanceID()
		{
			//Mock Data:
			if (m_functionCallCounts == null)
			{
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if (!m_functionCallCounts.ContainsKey("Int32 GetInstanceID()"))
			{
				m_functionCallCounts.Add("Int32 GetInstanceID()", 0);
			}
			m_functionCallCounts["Int32 GetInstanceID()"]++;

			return mInstanceId;
		}

		public static UnityEngine.Object Instantiate(UnityEngine.Object original, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static UnityEngine.Object Instantiate(UnityEngine.Object original)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static void Destroy(UnityEngine.Object obj, System.Single t)
		{
		}

		public static void Destroy(UnityEngine.Object obj)
		{
		}

		public static void DestroyImmediate(UnityEngine.Object obj, System.Boolean allowDestroyingAssets)
		{
		}

		public static void DestroyImmediate(UnityEngine.Object obj)
		{
		}

		public static UnityEngine.Object[] FindObjectsOfType(System.Type type)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static UnityEngine.Object FindObjectOfType(System.Type type)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static void DontDestroyOnLoad(UnityEngine.Object target)
		{
		}

		public static void DestroyObject(UnityEngine.Object obj, System.Single t)
		{
		}

		public static void DestroyObject(UnityEngine.Object obj)
		{
		}

		public static UnityEngine.Object[] FindSceneObjectsOfType(System.Type type)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static UnityEngine.Object[] FindObjectsOfTypeIncludingAssets(System.Type type)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static UnityEngine.Object[] FindObjectsOfTypeAll(System.Type type)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static implicit operator System.Boolean(UnityEngine.Object exists)
		{
			return exists != null;
		}

		public static System.Boolean operator ==(UnityEngine.Object x, UnityEngine.Object y)
		{
			return x.Equals(y);
		}

		public static System.Boolean operator !=(UnityEngine.Object x, UnityEngine.Object y)
		{
			return !(x == y);
		}

		public Object()
		{
			//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
			if (!m_functionCallCounts.ContainsKey("Void .ctor()"))
			{
				m_functionCallCounts.Add("Void .ctor()", 0);
			}
			m_functionCallCounts["Void .ctor()"]++;

		}

		public System.String name
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_name"))
				{
					m_functionCallCounts.Add("get_name", 0);
				}
				m_functionCallCounts["get_name"]++;

				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_name"))
				{
					m_functionCallCounts.Add("set_name", 0);
				}
				m_functionCallCounts["set_name"]++;

			}
		}

		public UnityEngine.HideFlags hideFlags
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_hideFlags"))
				{
					m_functionCallCounts.Add("get_hideFlags", 0);
				}
				m_functionCallCounts["get_hideFlags"]++;

				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_hideFlags"))
				{
					m_functionCallCounts.Add("set_hideFlags", 0);
				}
				m_functionCallCounts["set_hideFlags"]++;

			}
		}
	}
}