using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
	public enum UserAccountProperties
	{
		//general stuff
		FirstTimeUser, //bool

		//tutorial
		HasCompletedShoppingTutorial, //bool
		HasPlayedFashionMiniGame, //bool
		HasCompletedOpenMapTutorial, //bool
		HasCompletedDecorateTutorial, //bool
		HasCompletedGetCashTutorial, //bool
		HasCompleteMoveTutorial, //bool

        //last room entered
        LastRoomId, // RoomId
	}
}
