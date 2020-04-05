/**  --------------------------------------------------------  *
 *   AnimationSequence.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/20/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client
{
	public class AnimationSequence : AnimationStateBase
	{
		public class Listing
		{
			private readonly UnityEngine.AnimationClip mClip;
			public UnityEngine.AnimationClip Clip
			{
				get { return mClip; }
			}
			
			private readonly AnimationLoopCount mLoops;
			public AnimationLoopCount Loops
			{
				get { return mLoops; }
			}

			// Make clip.
			public Listing(UnityEngine.AnimationClip clip, AnimationLoopCount loops)
			{
				if (clip == null)
				{
					throw new ArgumentNullException("clip");
				}
				mClip = clip;
				
				if( loops == null )
				{
					throw new ArgumentNullException("loops");
				}
				mLoops = loops;
			}
		}

		private readonly System.Random mRandom = new System.Random((int)DateTime.Now.Ticks * 4);

		private ITask mExecutingAnimations = null;
		
		private readonly string mName;
		public override string Name
		{
	 		get { return mName; }
		}
		
		private readonly List<Listing> mAnimations = new List<Listing>();
		public List<Listing> Animations
		{
			get { return mAnimations; }
		}
		
		private bool mOneShot;
		public bool OneShot
		{
			get { return mOneShot; }
			set { mOneShot = value; }
		}
		
		public AnimationSequence(string name)
		: this( name, true )
		{
		}
		
		public AnimationSequence(string name, bool oneShot)
		{
			if( name == null )
			{
				throw new ArgumentNullException("name");
			}
			mName = name;
			mOneShot = oneShot;
		}

		public void AddFirstListing(Listing newListing)
		{
			mAnimations.Insert(0, newListing);
		}
		
		public void AddLastListing(Listing newListing)
		{
			mAnimations.Insert(mAnimations.Count - 1, newListing);
		}
		
		public void AddListing(Listing newListing)
		{
			mAnimations.Add(newListing);
		}
						
		public override void EnterState()
		{
			if (!(this.StateMachine is AvatarAnimationStateMachine))
			{
				throw new Exception("AnimationSequence expects to be used by AvatarAnimationStateMachine, not " + this.StateMachine.GetType().Name);
			}

			AvatarAnimationStateMachine stateMachine = (AvatarAnimationStateMachine)this.StateMachine;
			
			if( mExecutingAnimations != null )
			{
				mExecutingAnimations.Exit();
			}

            IScheduler scheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
            mExecutingAnimations = scheduler.StartCoroutine(SequenceAnimations(stateMachine));
		}
		
		public override void ExitState()
		{
			if( mExecutingAnimations != null )
			{
				mExecutingAnimations.Exit();
				mExecutingAnimations = null;
			}
		}
		
		private AnimationClip mCurrentPlayingClip = null;
		private IEnumerator<IYieldInstruction> SequenceAnimations(AvatarAnimationStateMachine stateMachine)
		{
			YieldWhile yieldWhileStateMachineIsAnimating = new YieldWhile(delegate()
			{
				return stateMachine.IsAnimating;
			});
			// Iterate through each of the animations in the sequence
			foreach( Listing animation in mAnimations )
			{
                Hangout.Shared.Action queueNextAnimation = delegate()
				{
					mCurrentPlayingClip = animation.Clip;
					stateMachine.PlayAnimation(animation.Clip);
					//Debug.Log(Time.time.ToString("f3") + ": Waiting for animation (" + animation.State.name + ") to complete in " + animation.State.length.ToString("f3") + " seconds");
				};
				
				// If this is an infinitely looping animation, play it forever
				while(animation.Loops.Infinite)
				{
					queueNextAnimation();
					yield return yieldWhileStateMachineIsAnimating;
				}
				
				// otherwise, play the animation for the specified number of loops
				uint loopCount = (uint)mRandom.Next((int)animation.Loops.Min, (int)animation.Loops.Max);
				for(uint i = 0; i < loopCount; ++i)
				{
					queueNextAnimation();
					yield return new YieldForSeconds(animation.Clip.length);
				}
			}

			while( !mOneShot )
			{
				stateMachine.PlayAnimation(mCurrentPlayingClip);
				yield return yieldWhileStateMachineIsAnimating;
			}
			mCurrentPlayingClip = null;
		}
		
		public override bool DoneAnimating
		{
			get
			{
				return mCurrentPlayingClip == null;
			}
		}
		
		public override void Dispose()
		{
			if ( mExecutingAnimations != null )
			{
				mExecutingAnimations.Exit();
			}
		}
	}	
}
