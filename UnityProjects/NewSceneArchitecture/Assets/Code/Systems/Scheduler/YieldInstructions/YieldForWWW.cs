using UnityEngine;
using Hangout.Shared;

namespace Hangout.Client
{
    public class YieldForWWW : IYieldInstruction
    {
		private readonly WWW mWWW;
		public YieldForWWW(WWW www)
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
