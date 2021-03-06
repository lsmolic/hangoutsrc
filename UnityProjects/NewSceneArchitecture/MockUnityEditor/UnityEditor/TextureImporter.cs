using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor
{
	public class TextureImporter : UnityEditor.AssetImporter
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

		public TextureImporter()
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

		public System.Boolean correctGamma
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_correctGamma"))
				{
					m_functionCallCounts.Add("get_correctGamma", 0);
				}
				m_functionCallCounts["get_correctGamma"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_correctGamma"))
				{
					m_functionCallCounts.Add("set_correctGamma", 0);
				}
				m_functionCallCounts["set_correctGamma"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public System.Boolean isReadable
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_borderMipmap"))
				{
					m_functionCallCounts.Add("get_borderMipmap", 0);
				}
				m_functionCallCounts["get_borderMipmap"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_borderMipmap"))
				{
					m_functionCallCounts.Add("set_borderMipmap", 0);
				}
				m_functionCallCounts["set_borderMipmap"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public System.Boolean borderMipmap
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_borderMipmap"))
				{
					m_functionCallCounts.Add("get_borderMipmap", 0);
				}
				m_functionCallCounts["get_borderMipmap"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_borderMipmap"))
				{
					m_functionCallCounts.Add("set_borderMipmap", 0);
				}
				m_functionCallCounts["set_borderMipmap"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public System.Boolean fadeout
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_fadeout"))
				{
					m_functionCallCounts.Add("get_fadeout", 0);
				}
				m_functionCallCounts["get_fadeout"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_fadeout"))
				{
					m_functionCallCounts.Add("set_fadeout", 0);
				}
				m_functionCallCounts["set_fadeout"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public System.Boolean mipmapEnabled
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_mipmapEnabled"))
				{
					m_functionCallCounts.Add("get_mipmapEnabled", 0);
				}
				m_functionCallCounts["get_mipmapEnabled"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_mipmapEnabled"))
				{
					m_functionCallCounts.Add("set_mipmapEnabled", 0);
				}
				m_functionCallCounts["set_mipmapEnabled"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public UnityEditor.TextureImporterNormalFilter normalmapFilter
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_normalmapFilter"))
				{
					m_functionCallCounts.Add("get_normalmapFilter", 0);
				}
				m_functionCallCounts["get_normalmapFilter"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_normalmapFilter"))
				{
					m_functionCallCounts.Add("set_normalmapFilter", 0);
				}
				m_functionCallCounts["set_normalmapFilter"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public System.Int32 maxTextureSize
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_maxTextureSize"))
				{
					m_functionCallCounts.Add("get_maxTextureSize", 0);
				}
				m_functionCallCounts["get_maxTextureSize"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_maxTextureSize"))
				{
					m_functionCallCounts.Add("set_maxTextureSize", 0);
				}
				m_functionCallCounts["set_maxTextureSize"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public System.Boolean convertToNormalmap
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_convertToNormalmap"))
				{
					m_functionCallCounts.Add("get_convertToNormalmap", 0);
				}
				m_functionCallCounts["get_convertToNormalmap"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_convertToNormalmap"))
				{
					m_functionCallCounts.Add("set_convertToNormalmap", 0);
				}
				m_functionCallCounts["set_convertToNormalmap"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public UnityEditor.TextureImporterMipFilter mipmapFilter
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_mipmapFilter"))
				{
					m_functionCallCounts.Add("get_mipmapFilter", 0);
				}
				m_functionCallCounts["get_mipmapFilter"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_mipmapFilter"))
				{
					m_functionCallCounts.Add("set_mipmapFilter", 0);
				}
				m_functionCallCounts["set_mipmapFilter"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public UnityEditor.TextureImporterGenerateCubemap generateCubemap
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_generateCubemap"))
				{
					m_functionCallCounts.Add("get_generateCubemap", 0);
				}
				m_functionCallCounts["get_generateCubemap"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_generateCubemap"))
				{
					m_functionCallCounts.Add("set_generateCubemap", 0);
				}
				m_functionCallCounts["set_generateCubemap"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public System.Boolean grayscaleToAlpha
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_grayscaleToAlpha"))
				{
					m_functionCallCounts.Add("get_grayscaleToAlpha", 0);
				}
				m_functionCallCounts["get_grayscaleToAlpha"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_grayscaleToAlpha"))
				{
					m_functionCallCounts.Add("set_grayscaleToAlpha", 0);
				}
				m_functionCallCounts["set_grayscaleToAlpha"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public System.Int32 mipmapFadeDistanceStart
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_mipmapFadeDistanceStart"))
				{
					m_functionCallCounts.Add("get_mipmapFadeDistanceStart", 0);
				}
				m_functionCallCounts["get_mipmapFadeDistanceStart"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_mipmapFadeDistanceStart"))
				{
					m_functionCallCounts.Add("set_mipmapFadeDistanceStart", 0);
				}
				m_functionCallCounts["set_mipmapFadeDistanceStart"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public System.Single heightmapScale
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_heightmapScale"))
				{
					m_functionCallCounts.Add("get_heightmapScale", 0);
				}
				m_functionCallCounts["get_heightmapScale"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_heightmapScale"))
				{
					m_functionCallCounts.Add("set_heightmapScale", 0);
				}
				m_functionCallCounts["set_heightmapScale"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public UnityEditor.TextureImporterFormat textureFormat
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_textureFormat"))
				{
					m_functionCallCounts.Add("get_textureFormat", 0);
				}
				m_functionCallCounts["get_textureFormat"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_textureFormat"))
				{
					m_functionCallCounts.Add("set_textureFormat", 0);
				}
				m_functionCallCounts["set_textureFormat"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public UnityEditor.TextureImporterFormat recommendedTextureFormat
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_recommendedTextureFormat"))
				{
					m_functionCallCounts.Add("get_recommendedTextureFormat", 0);
				}
				m_functionCallCounts["get_recommendedTextureFormat"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public System.Int32 mipmapFadeDistanceEnd
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_mipmapFadeDistanceEnd"))
				{
					m_functionCallCounts.Add("get_mipmapFadeDistanceEnd", 0);
				}
				m_functionCallCounts["get_mipmapFadeDistanceEnd"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_mipmapFadeDistanceEnd"))
				{
					m_functionCallCounts.Add("set_mipmapFadeDistanceEnd", 0);
				}
				m_functionCallCounts["set_mipmapFadeDistanceEnd"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}

		public UnityEditor.TextureImporterNPOTScale npotScale
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("get_npotScale"))
				{
					m_functionCallCounts.Add("get_npotScale", 0);
				}
				m_functionCallCounts["get_npotScale"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				if (!m_functionCallCounts.ContainsKey("set_npotScale"))
				{
					m_functionCallCounts.Add("set_npotScale", 0);
				}
				m_functionCallCounts["set_npotScale"]++;
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
		}
	}
}
