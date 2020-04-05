/**  --------------------------------------------------------  *
 *   GuiAnimation.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 12/18/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public class GuiAnimation : Widget
	{
		public class Frame
		{
			private readonly Texture mImage;
			private readonly float mTime;

			/// <summary>
			/// A single frame of the animation
			/// </summary>
			/// <param name="time">The time (in seconds) that this frame will be displayed.</param>
			public Frame(Texture image, float time)
			{
				if(image == null)
				{
					throw new ArgumentNullException("image");
				}
				if( time < 0.0f )
				{
					throw new ArgumentOutOfRangeException("time", "time must be a positive value");
				}

				mImage = image;
				mTime = time;
			}

			public Texture Image
			{
				get { return mImage; }
			}

			public float Time
			{
				get { return mTime; }
			}
		}

		private readonly List<Frame> mFrames = new List<Frame>();
		private float? mAnimationStartTime = null;
		private float mAnimationLength = 0.0f;
		
		public override object Clone()
		{
			return new GuiAnimation(this);
		}

		public GuiAnimation(GuiAnimation copy)
		: base(copy.Name, copy.GuiSize, copy.Style)
		{
			SetFrames(copy.mFrames);
		}

		public GuiAnimation(string name, IGuiSize size, IGuiStyle style, IEnumerable<Frame> frames)
			: base(name, size, style)
		{
			SetFrames(frames);
		}

		private void SetFrames(IEnumerable<Frame> frames)
		{
			mFrames.Clear();
			mAnimationLength = 0.0f;
			foreach(Frame frame in frames)
			{
				mFrames.Add(frame);
				mAnimationLength += frame.Time;
			}

			if( mAnimationLength == 0.0f )
			{
				// this exception will also catch empty frame lists
				throw new Exception("Cannot create an animation with a length of 0 seconds");
			}
		}
		
		public override void Draw(IGuiContainer container, Vector2 position) 
		{
			Rect coords = new Rect(position.x, position.y, this.ExternalSize.x, this.ExternalSize.y);

			if( mAnimationStartTime == null )
			{
				mAnimationStartTime = Time.time;
			}

			float timesPlayed = Time.time - (float)mAnimationStartTime / mAnimationLength;
			float animationTime = mAnimationLength - ((timesPlayed - Mathf.Floor(timesPlayed)) * mAnimationLength);

			Frame currentFrame = null;
			float totalFrameTime = 0.0f;
			foreach(Frame frame in mFrames)
			{
				totalFrameTime += frame.Time;
				if( totalFrameTime >= animationTime )
				{
					currentFrame = frame;
					break;
				}
			}
			if( this.Style != null )
			{
				GUI.Label(coords, currentFrame.Image, this.Style.GenerateUnityGuiStyle());
			}
			else 
			{
				GUI.Label(coords, currentFrame.Image);
			}
		}
	}	
}

