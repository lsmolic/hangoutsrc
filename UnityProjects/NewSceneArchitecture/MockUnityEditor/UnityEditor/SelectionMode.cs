using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor {
public enum SelectionMode {
	Unfiltered,
	TopLevel,
	Deep,
	ExcludePrefab,
	Editable,
	Assets,
	DeepAssets,
	OnlyUserModifiable
}
}
