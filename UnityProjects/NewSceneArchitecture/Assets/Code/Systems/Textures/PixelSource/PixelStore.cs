/*
	Created by Vilas Tewari
	
	A PixelStore is used to store the result of PixelSource Combinations
	It has dimensions of with and height
*/

using System;
using UnityEngine;
using System.Collections;

namespace Hangout.Client
{
	public class PixelStore : IDisposable
	{
		private int m_height;
		private int m_width;
		
		private HColor[] m_pixels;
		
		/*
			Properties
		*/
		public int Width
		{
			get { return m_width; }
		}
		public int Height
		{
			get { return m_height; }
		}
		public int Length
		{
			get { return m_height * m_width; }
		}
		
		/*
			Accessor
		*/
		public HColor this[ int x, int y ]
		{
			set { SetPixel( x, y, value ); }
		}
		
		public HColor this[ int x ]
		{
			get{ return m_pixels[x]; }
		}
		
		/*
			Pixel Layer Constructor
		*/
		public PixelStore( int width, int height )
		{	
			m_width = width;
			m_height = height;
			
			m_pixels = new HColor[ width * height ];
			if ( m_width < 1 || m_height < 1 )
			{
				Console.LogError( "A PixelStore cannot have 0 or negative dimensions" );
			}		
		}
		
		/*
	 		Set Texture Data. Setting a pixel out of bound does nothing
		*/
		protected void SetPixel( int x, int y, HColor pixel )
		{
			if( IndexInBounds(x, y))
			{
				m_pixels[ ToArrayCoords(x,y) ] = pixel;
			}
		}
		
		/*
			[x, y] to linear array co-ordinates
		*/
		protected int ToArrayCoords( int x, int y )
		{
			return ( y * Width ) + x;
		}
		
		/*
			Is Index in Bounds
		*/
		protected bool IndexInBounds( int x, int y )
		{
			return ( x >= 0 && x < Width && y >= 0 && y < Height );
		}
		
		public void Dispose()
		{
			m_pixels = null;
		}
	}
}