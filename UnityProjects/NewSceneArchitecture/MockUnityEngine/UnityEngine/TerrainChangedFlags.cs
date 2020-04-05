using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public enum TerrainChangedFlags {
	Heightmap,
	DetailPrototypes,
	DetailData,
	SplatPrototypes,
	SplatData,
	HeightmapDelayedUpdate,
	FlushImmediately
}
}
