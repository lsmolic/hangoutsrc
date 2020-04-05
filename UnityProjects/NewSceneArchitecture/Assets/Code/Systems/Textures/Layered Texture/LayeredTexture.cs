/*   
	Created by Vilas Tewari.

	A LayeredTexture is an object that maintains an ordered set of PixelsSourceLayers
	A LayeredTexture ( LT ) can flatten all it's PixelsSourceLayers to produce a Texture2D
	
	Layer Operations:
	Layers have a name, index, and PixelBlendMode. The index determines the order of flattening.
	You can Add Layers, Remove Layers, Swap out PixelSource's of a Layer, change Layers PixelBlendMode
	
	LayerFlattening:
	is a process in which all Layers combine their PixelSources sequentially using
	their specific BlendModes. LayerFlattening is expensive
	
	Since Layer Operations change the output Texture2D of the LT, a LT maintains a IsDirty property
	Each Layer keeps track of if it is Dirty or not, this enables LayerFlattening to only reprocess dirty Layers
	
	A complex Texture maintains a Mask for each layer
	A Mask is a float Array that stores the 0 to 1 influence of each pixel of a layer, in the final result
	This allows us to perform some layer specific operations on the final combined result without recombining all the layers
*/

using System;
using UnityEngine;
using System.Collections.Generic;
using Hangout.Shared;


namespace Hangout.Client
{
	public class LayeredTexture : IDisposable
	{
		// Dimension of the complex texture
		protected int mWidth;
		protected int mHeight;

		// Layers in flatten order. Last layer is on top
		protected List<PixelSourceLayer> mLayers;

		// A Rect that represents Dirty Pixels in the LT
		protected Rect mDirtyPixels;
		protected List<PixelSourceLayer> mDirtyLayers;

		// The Flattened result
		protected Texture2D mResultTexture;
		protected bool mIsResultCompressed;

		// Wrap mode of the flattened result
		private TextureWrapMode mWrapMode = TextureWrapMode.Clamp;

		// Layer lookup by name
		private readonly IDictionary<string, PixelSourceLayer> mLayerLookup = new Dictionary<string, PixelSourceLayer>();

		// Do we want the result to have an alpha channel
		private bool mRequireAlphaChannel;

		// Does the Complex Texture have unflattened changes
		public bool IsDirty
		{
			get { return HasDirtyPixels(); }
		}
		public int LayerCount
		{
			get { return mLayers.Count; }
		}
		public TextureWrapMode WrapMode
		{
			get { return mWrapMode; }
			set { mWrapMode = value; }
		}
		public int Width
		{
			get { return mWidth; }
		}
		public int Height
		{
			get { return mHeight; }
		}

		/*
			Texture2D returns the result of the flattened layers.
			Warning: If layer operations have been performed on the complex texture, GetTexture2D() will not reflect them
			until FlattenTexturePixelSources() has been called. Check the IsDirty property to know if there are unflattened changes
		*/
		public Texture2D Texture2D
		{
			get { return mResultTexture; }
		}

		/// <summary>
		/// Create an empty complex texture with given dimensions
		/// </summary>
		public LayeredTexture(int width, int height, bool requireAlphaChannel)
		{
			mWidth = Mathf.ClosestPowerOfTwo(width);
			mHeight = Mathf.ClosestPowerOfTwo(height);

			if (mWidth < 16 || mHeight < 16 || mWidth > 1024 || mHeight > 1024)
			{
				throw new ArgumentOutOfRangeException("LayeredTexture Constructor: given width and height are out of acceptable bounds ( 16 to 1024 )");
			}

			mLayers = new List<PixelSourceLayer>();

			// Create a blank Texture
			mRequireAlphaChannel = requireAlphaChannel;
			if (mRequireAlphaChannel)
				mResultTexture = new Texture2D(width, height, TextureFormat.ARGB32, true);
			else
				mResultTexture = new Texture2D(width, height, TextureFormat.RGB24, true);

			// Pretend its dirty to have the LT create the first blank texture
			mDirtyPixels = new Rect(0, 0, 1, 1);
			mIsResultCompressed = false;
			FlattenLayers();
		}

		public virtual void Dispose()
		{
			Texture.Destroy(mResultTexture);
		}

		private string GetNameForLayer(PixelSourceLayer layer)
		{
			foreach (KeyValuePair<string, PixelSourceLayer> kvp in mLayerLookup)
			{
				if (kvp.Value == layer)
				{
					return kvp.Key;
				}
			}

			throw new KeyNotFoundException("layer");
		}
		public LayeredTexture(LayeredTexture copy)
			: this(copy.mWidth, copy.mHeight, copy.mRequireAlphaChannel)
		{
			foreach (PixelSourceLayer layer in copy.mLayers)
			{
				this.AddLayer(copy.GetNameForLayer(layer), layer.PixelBlendMode, layer.PixelSource);
			}

			FlattenLayers();
		}

		/*
			Predicates
		*/
		public bool HasLayer(string name)
		{
			return mLayerLookup.ContainsKey(name);
		}

		/// <summary>
		/// FlattenLayers is made explicit in order to let us perform multiple layer operations before calculating the final result.
		///	FlattenLayers is an expensive operation and should be used sparingly.
		///	This function is a coroutine
		/// </summary>
		public IEnumerator<IYieldInstruction> FlattenLayersOverFramesCoroutine(int interval)
		{
			// TODO: remove this when we fix coroutine bug of coroutines never yielding.
			if (IsDirty)
			{
				if (mIsResultCompressed)
				{
					RefreshTexture2D();
					mIsResultCompressed = false;
				}

				for (int i = 0; i < interval; ++i)
				{
					if (i != 0)
					{
						yield return new YieldUntilNextFrame();
					}

					FlattenDirtyPixels(interval, i);
				}

				mResultTexture.wrapMode = mWrapMode;
				mResultTexture.Apply();

				// Mark all Layers as clean
				MarkAsClean();
			}
			else
			{
				yield return new YieldUntilNextFrame();
			}
		}

		/// <summary>
		/// Works like FlattenLayers, but does the work over a number of frames.
		/// </summary>
		/// <param name="frames">Number of frames to take to build the texture</param>
		public void FlattenLayers()
		{
			// doesn't need to start in a scheduler, this happens in a single frame
			IEnumerator<IYieldInstruction> iterator = FlattenLayersOverFramesCoroutine(1);
			iterator.MoveNext();
		}

		public void AddLayer(string name, PixelSource newPixelSource)
		{
			AddLayer(name, Pixel.PixelBlendMode.Layer, newPixelSource, 0, 0);
		}

		public void AddLayer(string name, Pixel.PixelBlendMode blendMode, PixelSource newPixelSource)
		{
			AddLayer(name, blendMode, newPixelSource, 0, 0);
		}

		public void AddLayer(string name, Pixel.PixelBlendMode blendMode, PixelSource newPixelSource, int xOffset, int yOffset)
		{
			// Check if a layer with the same name already exists
			if (HasLayer(name))
				Console.LogError("LayeredTexture AddLayer: PixelSource with name " + name + " already exists.");

			// Add layer
			AddLayer(name, new PixelSourceLayer(this, newPixelSource, blendMode, xOffset, yOffset));
		}

		protected PixelSourceLayer AddLayer(string name, PixelSourceLayer newLayer)
		{
			mLayerLookup.Add(name, newLayer);
			mLayers.Add(newLayer);
			MarkAsDirty(newLayer);
			return newLayer;
		}

		public void ChangeLayerBlendMode(string name, Pixel.PixelBlendMode newBlendMode)
		{
			// Check if a layer exists
			if (HasLayer(name))
			{
				PixelSourceLayer layer = GetLayer(name);
				layer.PixelBlendMode = newBlendMode;
				MarkAsDirty(layer);
			}
			else
			{
				Console.LogError("LayeredTexture ChangeLayerBlendMode: TexturePixelSource with name " + name + " does not exist");
			}
		}

		public void MoveLayerRelative(string layerName, int x, int y)
		{

			if (HasLayer(layerName))
			{
				PixelSourceLayer moveLayer = GetLayer(layerName);
				MarkAsDirty(moveLayer);

				moveLayer.OffsetX += x;
				moveLayer.OffsetY += y;

				MarkAsDirty(moveLayer);
			}
		}

		public void SetLayerPixelSource(string layerName, PixelSource newPixelSource)
		{
			// Check if a layer exists
			if (HasLayer(layerName))
			{
				// Get relevant layer
				PixelSourceLayer layer = GetLayer(layerName);

				// Mark the old pixel source as dirty
				MarkAsDirty(layer);

				// Swap
				layer.PixelSource = newPixelSource;

				// Mark the new source as dirty
				MarkAsDirty(layer);
			}
			else
				Console.LogError("LayeredTexture SetLayerPixelSource: Layer with name " + layerName + " does not exist");
		}


		/// <summary>
		/// Change the PixelFilter used by the Layer
		/// </summary>
		public void SetLayerFilter(string layerName, PixelFilter newFilter)
		{
			// Check if a layer exists
			if (!HasLayer(layerName))
			{
				throw new Exception("LayeredTexture SetLayerFilter: Layer with name " + layerName + " does not exist");
			}

			// Get relevant layer
			PixelSourceLayer layer = GetLayer(layerName);

			// Mark the layer as dirty
			MarkAsDirty(layer);

			// Swap
			layer.Filter = newFilter;
			newFilter.Parent = layer;
		}

		/// <summary>
		/// Change the PixelFilter Color property
		/// </summary>
		public void SetLayerFilterColor(string layerName, Color color)
		{
			if (HasLayer(layerName))
			{
				PixelSourceLayer layer = GetLayer(layerName);
				layer.Filter.FilterColor = color;
			}
			else
				Console.LogError("LayeredTexture SetLayerFilterColor: Layer with name " + layerName + " does not exist");
		}

		public Color GetLayerFilterColor(string layerName)
		{
			if (HasLayer(layerName))
			{
				PixelSourceLayer layer = GetLayer(layerName);
				return layer.Filter.FilterColor;
			}
			else
				Console.LogError("LayeredTexture GetLayerFilterColor: Layer with name " + layerName + " does not exist");
			return Color.clear;
		}

		public void ClearLayer(string layerName)
		{
			// Check if a layer exists
			if (HasLayer(layerName))
			{
				// Get relevant layer
				PixelSourceLayer layer = GetLayer(layerName);

				// Mark the layer as dirty
				MarkAsDirty(layer);

				if (layer.PixelBlendMode == Pixel.PixelBlendMode.Multiply)
					layer.PixelSource = new SolidColorPixelSource(mWidth, mHeight, Color.white);
				else
					layer.PixelSource = SolidColorPixelSource.Clear(mWidth, mHeight);
			}
			else
				Console.LogError("LayeredTexture ClearLayer: Layer with name " + layerName + " does not exist in this Complex Texture");
		}

		public override string ToString()
		{
			string result = "Complex Texture:\n";
			ICollection<string> layerNames = mLayerLookup.Keys;

			foreach (string l in layerNames)
			{
				PixelSourceLayer printLayer = GetLayer(l);
				result += GetLayerIndex(l) + " " + l + " " + printLayer.ToString() + "\n";
			}

			result += "Is Dirty: " + HasDirtyPixels() + "\nDirty Pixels: " + mDirtyPixels;
			return result;
		}

		protected int GetLayerIndex(string name)
		{
			return mLayers.IndexOf(GetLayer(name));
		}

		protected PixelSourceLayer GetLayer(string name)
		{
			return mLayerLookup[name] as PixelSourceLayer;
		}

		/*
			We combine the layers using a Top down per pixel approach
			for each pixel we traverse down
			the layers but only if the lower layer's pixel is visible
		*/
		protected virtual void FlattenDirtyPixels(int interval, int offset)
		{
			int startX = (int)mDirtyPixels.x + offset;
			int startY = (int)mDirtyPixels.y;
			int endX = (int)mDirtyPixels.x + (int)mDirtyPixels.width;
			int endY = (int)mDirtyPixels.y + (int)mDirtyPixels.height;

			// For each Dirty Pixel
			for (int x = startX; x < endX; x += interval)
			{
				for (int y = startY; y < endY; ++y)
				{
					mResultTexture.SetPixel(x, y, HColor.ToUnityColor(FlattenPixel(x, y, mLayers.Count, mLayers)));
				}
			}
		}

		/// <summary>
		/// Flatten a Pixel of the Complex Texture. Recurse down the layers and combine pixels
		/// </summary>
		protected HColor FlattenPixel(int x, int y, int level, List<PixelSourceLayer> layers)
		{
			// Base Case 1
			switch (level)
			{
				case 0:
					return new HColor(0, 0, 0, 0);
				case 1:
					return layers[0][x, y];
				default:
					PixelSourceLayer currentLayer = layers[level - 1];

					// Optimization 1: Out of PixelSource Bounds
					// If we are out of bounds for this layer don't bother Blending Pixels, go to next layer
					if (!currentLayer.IndexInBounds(x, y))
					{
						return FlattenPixel(x, y, level - 1, layers);
					}

					HColor pixel = currentLayer[x, y];
					Pixel.PixelBlendMode blendMode = currentLayer.PixelBlendMode;

					// Optimization 2
					// If layer Blendmode == Layer, and alpha is 1, get the hell out of here
					if (blendMode == Pixel.PixelBlendMode.Layer)
					{
						// Quick Exit
						if (pixel.a == 255)
							return pixel;

						// If 100% transparent go down to next layer
						if (pixel.a == 0)
							return FlattenPixel(x, y, level - 1, layers);
					}

					// Else recurse down and combine pixel with the result of the lower pixel
					Pixel.BlendPixels blendFunction = Pixel.GetBlendFunction(blendMode);
					return blendFunction(FlattenPixel(x, y, level - 1, layers), pixel);
			}
		}

		private void RefreshTexture2D()
		{
			if (mRequireAlphaChannel)
				mResultTexture.Resize(mWidth, mHeight, TextureFormat.ARGB32, true);
			else
				mResultTexture.Resize(mWidth, mHeight, TextureFormat.RGB24, true);
		}

		public void MarkAsDirty(PixelSourceLayer dirtyLayer)
		{
			// If already Dirty combine dirty spaces
			if (HasDirtyPixels())
			{
				float minX = Mathf.Min(mDirtyPixels.x, dirtyLayer.OffsetX);
				float minY = Mathf.Min(mDirtyPixels.y, dirtyLayer.OffsetY);

				float maxX = Mathf.Max(mDirtyPixels.xMax, dirtyLayer.OffsetX + dirtyLayer.PixelSource.Width);
				float maxY = Mathf.Max(mDirtyPixels.yMax, dirtyLayer.OffsetY + dirtyLayer.PixelSource.Height);

				maxX = Mathf.Clamp(maxX, 0, mWidth);
				maxY = Mathf.Clamp(maxY, 0, mHeight);

				minX = Mathf.Clamp(minX, 0, mWidth);
				minY = Mathf.Clamp(minY, 0, mHeight);

				mDirtyPixels = new Rect(minX, minY, maxX - minX, maxY - minY);
			}
			else
			{
				mDirtyPixels = new Rect(dirtyLayer.OffsetX, dirtyLayer.OffsetY, dirtyLayer.PixelSource.Width, dirtyLayer.PixelSource.Height);
			}
		}

		private void MarkAsClean()
		{
			mDirtyPixels = new Rect(0, 0, 0, 0);
		}

		private bool HasDirtyPixels()
		{
			return (mDirtyPixels.width > 0 && mDirtyPixels.height > 0);
		}

		private bool IsLayerDirty(PixelSourceLayer pl)
		{
			return mDirtyLayers.Contains(pl);
		}
	}
}