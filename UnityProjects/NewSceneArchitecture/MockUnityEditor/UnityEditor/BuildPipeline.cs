using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor
{
	public class BuildPipeline
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

		public static void PushAssetDependencies()
		{
		}

		public static void PopAssetDependencies()
		{
		}

		public static System.String BuildPlayer(System.String[] levels, System.String locationPathName, UnityEditor.BuildTarget target, UnityEditor.BuildOptions options)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, System.String pathName)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, System.String pathName, UnityEditor.BuildAssetBundleOptions options)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public BuildPipeline()
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
	}
}
