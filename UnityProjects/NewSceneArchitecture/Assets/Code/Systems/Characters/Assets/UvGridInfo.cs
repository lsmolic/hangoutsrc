/*
   Created by Vilas Tewari on 2009-07-29.

	A placeholder class that holds information on
	what material index is mapped to what textureZone
*/

using UnityEngine;
using System.Collections;

public class UvGridInfo : MonoBehaviour
{
	[System.Serializable]
	public class UvData
	{
		public string m_name;
		public string m_textureZone;
		public int m_gridWidth;
		public int m_gridHeight;
		
		/*
			Properties
		*/
		public string Name
		{
			get{ return m_name; }
		}
		public string TextureZone
		{
			get { return m_textureZone; }
		}
		public int Width
		{
			get { return m_gridWidth; }
		}
		public int Height
		{
			get { return m_gridHeight; }
		}
	}
	
	// The index of the UvData in the array corresponds to the material index
	public UvData[] m_uvGrids;
	
	public int GetIndexOfShell( string shellName )
	{
		for( int x = 0; x < m_uvGrids.Length; ++x )
		{
			if ( m_uvGrids[x].Name == shellName )
			{
				return x;
			}
		}
		Hangout.Client.Console.LogError("No shell with name " + shellName + " exists");
		return -1;
	}
}
