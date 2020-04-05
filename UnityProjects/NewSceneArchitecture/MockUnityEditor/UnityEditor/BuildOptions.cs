using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor
{
	public enum BuildOptions
	{
		StripDebugSymbols,
		CompressTextures,
		AutoRunPlayer,
		ShowBuiltPlayer,
		BuildAdditionalStreamedScenes
	}
}
