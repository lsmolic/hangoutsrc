using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
    public static class ItemType
    {
        public const string TOPS = "Tops";
        public const string PANTS = "Pants";
        public const string SKIRT = "Skirt";
        public const string SHOES = "Shoes";
        public const string MAKEUP = "Makeup";
        public const string ROOM = "Room";
        public const string ROOM_BACKDROP = "Room Backdrop";
        public const string HAIR = "Hair";
        public const string BODY = "Body";
        public const string FACE = "Face";
        public const string EMOTE = "Emote";
		public const string MOOD = "Mood";
		public const string EMOTICON = "Emoticon";
        public const string BAGS = "Bags";
        public const string ENERGY_REFILL = "Energy Refill";
		public const string PUBLIC_ROOM_BACKGROUND = "Public Room Background";
		public const string PUBLIC_ROOM_BACKDROP = "Public Room Backdrop";
		public const string PRIVATE_ROOM_BACKGROUND = "Private Room Background";
		public const string PRIVATE_ROOM_BACKDROP = "Private Room Backdrop";

		public static bool ValidItemTypeString(string str)
		{
			bool result;

			// Make sure that when you add an ItemType at the top of this class, 
			//  you add a case in this switch.

			// TODO: This switch is duplicate data, combine this list with the above list
			switch(str)
			{
				case TOPS:
				case PANTS:
				case SKIRT:
				case SHOES:
				case MAKEUP:
				case ROOM:
				case ROOM_BACKDROP:
				case HAIR:
				case BODY:
				case FACE:
				case EMOTE:
				case MOOD:
				case EMOTICON:
				case BAGS:
                case ENERGY_REFILL:
				case PUBLIC_ROOM_BACKGROUND:
				case PUBLIC_ROOM_BACKDROP:
				case PRIVATE_ROOM_BACKGROUND:
				case PRIVATE_ROOM_BACKDROP:
					result = true;
					break;
				default:
					result = false;
					break;
			}

			return result;
		}
    }
}
