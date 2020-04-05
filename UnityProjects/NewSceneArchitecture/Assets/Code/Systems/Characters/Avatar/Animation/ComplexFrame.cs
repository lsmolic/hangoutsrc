using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Client
{
	public class ComplexFrame
	{
		private readonly int mRepeatFrame = 1;
		private readonly SimpleUvAnimation[] mUvAnimations;
		private readonly float mWaitAfterFinished = 0;

		public int RepeatFrame
		{
			get { return mRepeatFrame; }
		}
		public SimpleUvAnimation[] UvAnimations
		{
			get { return mUvAnimations; }
		}
		public float WaitAfterFinished
		{
			get { return mWaitAfterFinished; }
		}
		public ComplexFrame(int reapeatFrame, float waitAfterFinished, SimpleUvAnimation[] simpleAnimations)
		{
			mRepeatFrame = reapeatFrame;
			mWaitAfterFinished = waitAfterFinished;
			mUvAnimations = simpleAnimations;
		}
	}
}
