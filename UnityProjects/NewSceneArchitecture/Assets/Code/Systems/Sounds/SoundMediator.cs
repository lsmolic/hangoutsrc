/* Pherg 10.16.09 */

using System;
using System.Collections.Generic;
using System.Text;

using PureMVC.Patterns;

namespace Hangout.Client
{
	public class SoundMediator : Mediator
	{
		private SoundProxy mSoundProxy;
		public override void OnRegister()
		{
			base.OnRegister();
			mSoundProxy = GameFacade.Instance.RetrieveProxy<SoundProxy>();
		}
		public override IList<string> ListNotificationInterests()
		{
			return new string[] 
            {
                GameFacade.PLAY_SOUND_APPLY_ROOM_BACKGROUND,
                GameFacade.PLAY_SOUND_BUTTON_PRESS,
                GameFacade.PLAY_SOUND_CLOSE,
                GameFacade.PLAY_SOUND_ERROR,
                GameFacade.PLAY_SOUND_LEVEL_UP,
                GameFacade.PLAY_SOUND_MAP_OPEN,
                GameFacade.PLAY_SOUND_POPUP_APPEAR_A,
                GameFacade.PLAY_SOUND_POPUP_APPEAR_B,
                GameFacade.PLAY_SOUND_SWAPPING_ITEMS,
            };
		}

		public override void HandleNotification(PureMVC.Interfaces.INotification notification)
		{
			base.HandleNotification(notification);
			switch (notification.Name)
			{
				case GameFacade.PLAY_SOUND_APPLY_ROOM_BACKGROUND:
					mSoundProxy.PlaySound(SoundName.ApplyRoomBackground);
					break;
				case GameFacade.PLAY_SOUND_BUTTON_PRESS:
					mSoundProxy.PlaySound(SoundName.ButtonPress);
					break;
                case GameFacade.PLAY_SOUND_CLOSE:
					mSoundProxy.PlaySound(SoundName.Close);
					break;
                case GameFacade.PLAY_SOUND_ERROR:
					mSoundProxy.PlaySound(SoundName.Error);
					break;
                case GameFacade.PLAY_SOUND_LEVEL_UP:
					mSoundProxy.PlaySound(SoundName.LevelUp);
					break;
                case GameFacade.PLAY_SOUND_MAP_OPEN:
					mSoundProxy.PlaySound(SoundName.MapOpen);
					break;
                case GameFacade.PLAY_SOUND_POPUP_APPEAR_A:
					mSoundProxy.PlaySound(SoundName.PopupAppearA);
					break;
                case GameFacade.PLAY_SOUND_POPUP_APPEAR_B:
					mSoundProxy.PlaySound(SoundName.PopupAppearB);
					break;
				case GameFacade.PLAY_SOUND_SWAPPING_ITEMS:
					mSoundProxy.PlaySound(SoundName.SwappingItems);
					break;
			}
		}

	}
}
