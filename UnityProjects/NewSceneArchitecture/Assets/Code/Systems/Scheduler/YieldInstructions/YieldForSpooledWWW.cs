using UnityEngine;
using Hangout.Shared;

namespace Hangout.Client
{
	public class YieldForSpooledWWW : IYieldInstruction
	{
		private readonly SpooledWWW mWWW;
		public YieldForSpooledWWW(SpooledWWW www)
		{
			mWWW = www;
		}
		
		public bool Ready
		{
			get 
			{
				return mWWW.isDone;
			}
		}
	}
}
