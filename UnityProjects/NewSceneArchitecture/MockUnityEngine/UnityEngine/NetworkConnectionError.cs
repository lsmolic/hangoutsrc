using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public enum NetworkConnectionError {
	NoError,
	RSAPublicKeyMismatch,
	InvalidPassword,
	ConnectionFailed,
	TooManyConnectedPlayers,
	ConnectionBanned,
	AlreadyConnectedToAnotherServer,
	CreateSocketOrThreadFailure,
	IncorrectParameters,
	EmptyConnectTarget,
	InternalDirectConnectFailed,
	NATTargetNotConnected,
	NATTargetConnectionLost
}
}
