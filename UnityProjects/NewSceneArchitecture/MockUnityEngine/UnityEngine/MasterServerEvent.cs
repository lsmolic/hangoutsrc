using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public enum MasterServerEvent {
	RegistrationFailedGameName,
	RegistrationFailedGameType,
	RegistrationFailedNoServer,
	RegistrationSucceeded,
	HostListReceived
}
}
