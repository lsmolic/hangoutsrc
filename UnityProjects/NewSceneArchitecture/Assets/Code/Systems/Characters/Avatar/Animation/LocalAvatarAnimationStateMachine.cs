/**  --------------------------------------------------------  *
 *   LocalAvatarAnimationStateMachine.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/24/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;
using PureMVC.Interfaces;
using Hangout.Shared;

namespace Hangout.Client
{
	/// <summary>
	/// LocalAvatarAnimationStateMachine handles animation for the local avatar, including emotes.
	/// </summary>
	class LocalAvatarAnimationStateMachine : DefaultAvatarAnimationStateMachine
	{
		public LocalAvatarAnimationStateMachine(LocalAvatarEntity avatarEntity)
			: base(avatarEntity)
		{
		}

        public override IList<string> ListNotificationInterests()
        {
            List<string> interestList = new List<string>();
            interestList.Add(GameFacade.PLAY_EMOTE);
            return interestList;
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case GameFacade.PLAY_EMOTE:
					string emoteName = notification.Body as string;
                    if (emoteName == null)
                    {
						Console.LogError("Unable to parse notification.Body into emoteName");
						return;
                    }
                    RigAnimationName animationName = (RigAnimationName)Enum.Parse(typeof(RigAnimationName), emoteName);
                    GameFacade.Instance.RetrieveProxy<AnimationProxy>().GetRigAnimation(animationName, delegate(AnimationClip animationClip)
                    {	
						AnimationSequence emoteSequence = new AnimationSequence(animationClip.name, true);
						emoteSequence.AddListing(new AnimationSequence.Listing(animationClip, new AnimationLoopCount(1, 1)));
						Emote emote = new Emote(animationName, emoteSequence);
						
						// Send emote message so other clients see us emoting
						GameFacade.Instance.RetrieveProxy<LocalAvatarProxy>().LocalAvatarDistributedObject.SendEmoteUpdate(emote.Name.ToString());
						PlayEmote(emote);
                    });
                    break;
            }
        }

		public override float CurrentAvatarSpeed
		{
			// This implementation is interfacing with prototype code, it should be replaced at some point
			get
			{
				return this.AnimationComponent.rigidbody.velocity.magnitude;
			}
		}
		
		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
