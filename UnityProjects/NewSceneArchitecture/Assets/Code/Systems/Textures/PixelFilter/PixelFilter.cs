/*
   Created by Vilas Tewari on 2009-08-08.

	A PixelFilter is used by PixelSourceLayers to sample data from PixelSources
	It enables PixelSourceLayers to tweak / adjust the pixelSource with changing the PixelSource data
	This lets us share PixelSources between multiple LayeredTextures and Layers
*/

using UnityEngine;
using System.Collections;

namespace Hangout.Client
{
	public abstract class PixelFilter
	{
		/* What PixelSourceLayer do we belong to */
		protected PixelSourceLayer mParent;
		
		public PixelSourceLayer Parent
		{
			set{ mParent = value; }
		}
		
		public abstract Color FilterColor
		{
			set;
			get;
		}
		
		/*
			Filter a pixel based on some parameters and return a color value
			This base class doesn't modify the pixel source
		*/
		public abstract HColor FilterPixel( HColor source );
		
		protected void MarkAsDirty()
		{
			if ( mParent != null )
				mParent.MarkAsDirty();
		}
	}
}