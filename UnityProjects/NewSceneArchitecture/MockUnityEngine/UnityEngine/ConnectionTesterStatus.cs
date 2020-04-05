using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public enum ConnectionTesterStatus {
	Error,
	Undetermined,
	PrivateIPNoNATPunchthrough,
	PrivateIPHasNATPunchThrough,
	PublicIPIsConnectable,
	PublicIPPortBlocked,
	PublicIPNoServerStarted
}
}
