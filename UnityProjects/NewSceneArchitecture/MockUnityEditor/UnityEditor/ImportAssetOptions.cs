using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor {
public enum ImportAssetOptions {
	Default,
	ForceUpdate,
	ForceSynchronousImport,
	TryFastReimportFromMetaData,
	ImportRecursive
}
}
