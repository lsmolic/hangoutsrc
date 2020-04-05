
namespace Hangout.Shared
{
	public enum MessageSubType
	{
		// Connection
		Disconnect,
		RequestLogin,
	   	InvalidLogin,
       	SuccessfulLogin,	
		
		// Avatar
		Chat,
		Emoticon,
		Telemetry,
		Delete,
		Emote,
        SetPosition,
        UpdateDna,
		
		// Room
		DeleteRoom,
        RequestRooms,
        ReceiveRooms,
        SwitchRoom,
        CreateRoom,
        SearchRooms,
		LeaveRoom,
		JoinRoom,
		
		// GreenScreenRoom
        ChangeBackground,
		
		// Room Request
        ClientOwnedRooms,
        FriendsRooms,
        PublicRooms,
		
		// Room Loading
		LoadNewRoom,
		
		// Fashion Minigame
		RequestLoadingInfo,
		RequestNpcAssets,
		GetPlayerData,
		SetPlayerData,
		GetFriendsToHire,
		HireFriend,
        LevelComplete,
		GetAllHiredAvatars,
		UseEnergy,
		
		// Inventory 
        PaymentId,
		PlayerInventory,
        StoreInventory,
        EnergyStore,
        Animation,
        Purchase,
        GetBalance,
		
		// Friends
        ReceiveFriends,
		ReceiveEntourage,
		
		// Errors
		UserLoginGetOrCreateRoomError,
		UserLoginGetOrCreateAvatarError,
		UserLoginCannotGetAccountFromSessionManagerError,
		
		// Account
		SaveUserProperties,
		
		// Asset Repo
		GetItemsById,
        GetEmotes,
        GetMoods,
		
		// Escrow
		ProcessEscrowTransaction,
		
		// Admin
        HealthCheck,
    }
}