using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor
{
	public class Editor : UnityEngine.ScriptableObject
	{
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

		public void Repaint()
		{
			//Mock Data:
			if (m_functionCallCounts == null)
			{
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if (!m_functionCallCounts.ContainsKey("Void Repaint()"))
			{
				m_functionCallCounts.Add("Void Repaint()", 0);
			}
			m_functionCallCounts["Void Repaint()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public Editor()
		{
			//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
			if (!m_functionCallCounts.ContainsKey("Void .ctor()"))
			{
				m_functionCallCounts.Add("Void .ctor()", 0);
			}
			m_functionCallCounts["Void .ctor()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public UnityEngine.Object target
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_target"))
				{
					m_functionCallCounts.Add("get_target", 0);
				}
				m_functionCallCounts["get_target"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_target"))
				{
					m_functionCallCounts.Add("set_target", 0);
				}
				m_functionCallCounts["set_target"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public virtual void OnInspectorGUI()
		{

			if (m_functionCallCounts == null)
			{
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if (!m_functionCallCounts.ContainsKey("OnInspectorGUI"))
			{
				m_functionCallCounts.Add("OnInspectorGUI", 0);
			}
			m_functionCallCounts["OnInspectorGUI"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}
}