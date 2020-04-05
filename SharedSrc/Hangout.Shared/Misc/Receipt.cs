using System;

namespace Hangout.Shared
{
	public class Receipt : IReceipt
	{
		private readonly Hangout.Shared.Action mOnExit;

		/// <summary>
		/// A receipt that does nothing
		/// </summary>
		public Receipt()
		{
			mOnExit = null;
		}

		public Receipt(Hangout.Shared.Action onExit)
		{
			if (onExit == null)
			{
				throw new ArgumentNullException("onExit");
			}
			mOnExit = onExit;
		}

		public void Exit()
		{
			if (mOnExit != null)
			{
				mOnExit();
			}
		}
	}
}