/*   
	Created by Vilas Tewari.

	A LayeredPalette is LayeredTexture that maintains TextureZones
	
	TextureZones: A TextureZone is essentially a set of Layers.
		
		TextureZones define an area ( in pixels ) on the LayeredTexture.
		All layers that belong to the TextureZone are confined to the zone area.
		TextureZones can be added and removed.
		TextureZones cannot overlap
		A Layer can only belong to 1 or no zones
*/


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hangout.Client
{
	public class LayeredPalette : LayeredTexture
	{
		private class TextureZone
		{
			private string mName;
			private Rect mArea;

			// Layers that belong to this zone
			private List<PixelSourceLayer> mZoneLayers;

			/*
				Properties
			*/
			public string Name
			{
				get { return mName; }
			}
			public Rect Area
			{
				get { return mArea; }
			}
			public List<PixelSourceLayer> ZoneLayers
			{
				get { return mZoneLayers; }
			}
			/*
				Constructor
			*/
			public TextureZone(string name, Rect area)
			{
				mName = name;
				mArea = area;
				mZoneLayers = new List<PixelSourceLayer>();
			}
			public bool IndexInBounds(int x, int y)
			{
				return mArea.Contains(new Vector2(x, y));
			}

			public void AddLayer(PixelSourceLayer pl)
			{
				mZoneLayers.Add(pl);
			}
		}

		// TextureZone
		private List<LayeredPalette.TextureZone> mZones = new List<LayeredPalette.TextureZone>();

		// Zone lookup by name
		private IDictionary<string, TextureZone> mZoneLookup = new Dictionary<string, TextureZone>();

		// Layers that belong to all Zones
		private List<PixelSourceLayer> mUniversalLayers = new List<PixelSourceLayer>();
		/*
			Constructor: Create an empty LayeredPalette with given dimensions
		*/
		public LayeredPalette(int width, int height, bool requireAlphaChannel)
			: base(width, height, requireAlphaChannel)
		{
		}

		/*
			Predicates
		*/
		public bool HasZone(string name)
		{
			return mZoneLookup.ContainsKey(name);
		}

		/*
			Add a TextureZone to which we can add layers
		*/
		public void CreateTextureZone(string name, int startX, int startY, int width, int height)
		{
			if (HasZone(name))
			{
				Console.LogError("LayeredPalette CreateTextureZone: Zone with name " + name + " already exists in this LayeredPalette.");
				return;
			}

			// Clamp to be within the texture
			Vector2 zoneStart = new Vector2(Mathf.Clamp(startX, 0, Width - 1), Mathf.Clamp(startY, 0, Height - 1));
			Vector2 zoneEnd = new Vector2(Mathf.Clamp(startX + width - 1, 0, Width - 1), Mathf.Clamp(startY + height - 1, 0, Height - 1));

			// if the start and end points are inverted
			if (zoneEnd.magnitude - zoneStart.magnitude < 0)
			{
				Console.LogError("LayeredPalette CreateTextureZone(): Start Co-ordinates of " + name + " must be to lower left of End Co-ordinates");
				return;
			}

			// If this zone overlaps with an existing zone
			int corner1 = HitTestTextureZones((int)zoneStart.x, (int)zoneStart.y);
			int corner2 = HitTestTextureZones((int)zoneEnd.x, (int)zoneEnd.y);
			int corner3 = HitTestTextureZones((int)zoneStart.x, (int)zoneEnd.y);
			int corner4 = HitTestTextureZones((int)zoneEnd.x, (int)zoneStart.y);

			if (corner1 + corner2 + corner3 + corner4 != -4)
			{
				Console.LogError("LayeredPalette CreateTextureZone(): Given TextureZone Co-ordinates for " + name + " Overlap with an existing TextureZone");
				return;
			}
			TextureZone newTextureZone = new TextureZone(name, new Rect(zoneStart.x, zoneStart.y, zoneEnd.x - zoneStart.x + 1, zoneEnd.y - zoneStart.y + 1));
			mZones.Add(newTextureZone);
			mZoneLookup[name] = newTextureZone;

			/* Add All universal layers to this zone */
			for (int x = 0; x < mUniversalLayers.Count; ++x)
				newTextureZone.AddLayer(mUniversalLayers[x]);
		}

		/*
			Add an empty layer to a zone
		*/
		public void CreateLayerInZone(string layerName, string zoneName, Pixel.PixelBlendMode blendMode)
		{
			if (!HasZone(zoneName))
				Console.LogError("LayeredPalette AddLayer(): Zone " + zoneName + " does not exist");
			else
			{
				TextureZone zone = GetZone(zoneName);
				SolidColorPixelSource newPixelSource = SolidColorPixelSource.Clear((int)zone.Area.width, (int)zone.Area.height);
				PixelSourceLayer newLayer = new PixelSourceLayer(this, newPixelSource, blendMode, (int)zone.Area.x, (int)zone.Area.y);
				CreateLayerInZone(layerName, zoneName, newLayer);
			}
		}

		public void CreateLayerInAllZones(string layerName, PixelSource newPixelSource, Pixel.PixelBlendMode blendMode)
		{
			PixelSourceLayer newLayer = new PixelSourceLayer(this, newPixelSource, blendMode, 0, 0);

			mUniversalLayers.Add(newLayer);

			// Add this layer to all zones
			for (int x = 0; x < mZones.Count; ++x)
				mZones[x].AddLayer(newLayer);

			// Create Layer
			AddLayer(layerName, newLayer);
		}

		/*
		
		*/
		private PixelSourceLayer CreateLayerInZone(string layerName, string zoneName, PixelSourceLayer newLayer)
		{
			TextureZone zone = GetZone(zoneName);

			/* Create Layer */
			PixelSourceLayer addedLayer = AddLayer(layerName, newLayer);

			/* Does the Layer Clip the Zone? */
			if (addedLayer.PixelSource.Width > zone.Area.width + 1 || addedLayer.PixelSource.Height > zone.Area.height + 1)
				Console.LogError("LayeredPalette CreateLayerInZone(): Layer " + layerName + " will be clipped when placed in zone " + zoneName);

			/* Add Layer to Zone */
			zone.AddLayer(newLayer);

			return addedLayer;
		}

		/*
			Get Zone Area in Pixels
		*/
		public Rect GetTextureZoneArea(string zoneName)
		{
			if (HasZone(zoneName))
			{
				return GetZone(zoneName).Area;
			}
			Console.LogError("LayeredPalette GetZoneArea(): No TextureZone with name " + zoneName + " exists");
			return new Rect(0, 0, 0, 0);
		}

		/*
			Get Zone Uv Area
		*/
		public Rect GetTextureZoneUvArea(string zoneName)
		{
			Rect zoneArea = GetTextureZoneArea(zoneName);

			return new Rect(zoneArea.x / Width, zoneArea.y / Height, zoneArea.width / Width, zoneArea.height / Height);
		}

		/*
			We combine the layers using a Top down per pixel approach
			for each pixel we traverse down
			the layers but only if the lower layer's pixel is visible
		*/
		protected override void FlattenDirtyPixels(int interval, int offset)
		{
			// for each zone
			List<PixelSourceLayer> zoneLayers;
			Rect dirtyPixels;

			int startX;
			int startY;
			int endX;
			int endY;

			// For each Zone check if it has dirty pixels
			for (int z = 0; z < mZones.Count; ++z)
			{
				// If there is a intersection with Dirty Rect
				dirtyPixels = RectUtility.PixelIntersection(mDirtyPixels, mZones[z].Area);
				if (dirtyPixels.width > 0 && dirtyPixels.height > 0)
				{
					// Flatten only layers that belong to this zone in dirty rect
					zoneLayers = mZones[z].ZoneLayers;

					if (zoneLayers.Count > 0)
					{
						startX = (int)dirtyPixels.x + offset;
						startY = (int)dirtyPixels.y;
						endX = (int)dirtyPixels.x + (int)dirtyPixels.width;
						endY = (int)dirtyPixels.y + (int)dirtyPixels.height;

						// For each Dirty Pixel
						for (int x = startX; x < endX; x += interval)
						{
							for (int y = startY; y < endY; ++y)
							{
								// If the Dirty mask is set to true
								mResultTexture.SetPixel(x, y, HColor.ToUnityColor(FlattenPixel(x, y, zoneLayers.Count, zoneLayers)));
							}
						}
					}
				}
			}
		}

		/*
			Get Zone reference from Zone name
		*/
		private LayeredPalette.TextureZone GetZone(string name)
		{
			return mZoneLookup[name] as LayeredPalette.TextureZone;
		}

		/*
			HitTest Pixel Values. Returns the index of the TextureZone hit, or -1 is nothing is hit
		*/
		private int HitTestTextureZones(int x, int y)
		{
			int hit = -1;
			for (int z = 0; z < mZones.Count; ++z)
			{
				if (mZones[z].IndexInBounds(x, y))
				{
					hit = z;
					return hit;
				}
			}
			return hit;
		}
	}
}