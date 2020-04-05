/*
	Created by Vilas Tewari

	TexturePixelSource Is a PixelSource that gets it's data from a Texture2D
	TexturePixelSource color info is stored as a Color Array
*/

using UnityEngine;
using System.Collections;


namespace Hangout.Client
{
	public class TexturePixelSource : PixelSource
	{
		private HColor[] mPixels;

		/// <summary>
		/// Texture Layer from Texture2D Constructor
		/// </summary>
		public TexturePixelSource(int width, int height, Texture2D source)
			: base(width, height)
		{
			mPixels = new HColor[width * height];
			SetPixels(source);
		}

		/// <summary>
		/// Texture Layer from Texture2D Constructor
		/// </summary>
		public TexturePixelSource(Texture2D source)
			: base(source.width, source.height)
		{
			mPixels = new HColor[source.width * source.height];
			SetPixels(source);
		}

		public override bool IsPixelClear(int x, int y)
		{
			return this[x, y].a == 0;
		}

		public override bool IsPixelWhite(int x, int y)
		{
			return HColor.IsWhite(this[x, y]);
		}

		/// <summary>
		/// Get Texture Color. If you request a pixel out of bounds we return a blank pixel
		/// </summary>
		protected override HColor GetPixel(int x, int y)
		{
			return mPixels[(y * Width) + x];
		}

		/// <summary>
		/// Set Data from Texture2D
		/// </summary>
		private bool SetPixels( Texture2D source )
		{
			if ( !TextureOperations.IsReadableWritable( source ))
			{
				Console.LogError( "TexturePixelSource GetData(): source Texture2D(" + source.name + ") is compressed and therefore not readable" );
				return false;
			}
			else
			{
				// If the source is not the desired size resize it
				if ( source.width != Width || source.height != Height )
				{
					source = TextureOperations.ResizeTexture( source, Width, Height );
				}
				// Get Mip level 0 pixels
				for( int x = 0; x < Width; ++x )
				{
					for( int y = 0; y < Height; ++y )
					{
						mPixels[(y * Width) + x] = new HColor( source.GetPixel(x,y) );
					}
				}
				return true;
			}
		}
	}
}