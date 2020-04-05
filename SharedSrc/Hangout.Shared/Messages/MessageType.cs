using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
	public enum MessageType
	{
		Undefined,
		Connect,
		Disconnect,
		Create,
		Delete,
		Update,
		Loading,
		PaymentItems,
        Room,
        Inventory,
		FashionMinigame,
        Admin,
        Friends,
		Error,
		Account,
        Event,
        AssetRepository,
		Heartbeat,
		Escrow,
	}
}
