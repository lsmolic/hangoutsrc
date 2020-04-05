using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor {
public enum PrefabType {
	None,
	Prefab,
	ModelPrefab,
	PrefabInstance,
	ModelPrefabInstance,
	MissingPrefabInstance,
	DisconnectedPrefabInstance,
	DisconnectedModelPrefabInstance
}
}
