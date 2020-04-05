/*

SpooledWWW.cs

Wrapper for UnityEngine.WWW that limits the number of simultaneous downloads!

SpooledWWW HASA WWW.  Can't inherit from WWW because we need to defer InitWWW, 
which is always called from WWW's constructor.	That leaves us with the choice of 
modifying UnityEngine.WWW or changing our behavior as customers.  Sigh.

Usage:

SpooledWWW REQUIRES that instance.isDone be polled until it returns true.  If this is 
not done, the download may never start!	 Alternatively, you may poll instance.result 
for the same effect--it will return null while the request is incomplete.

After isDone returns true or result becomes non-null, instance.result will return the 
underlying WWW request, which is complete (and can be used to pull texture, audio, 
etc, as normal).

YieldForSpooledWWW is an IYieldInstruction that's set up to poll instance.isDone as a 
coroutine.	Usage is identical to YieldForWWW.	

Example usage: See ClientAssetRepository.DownloadTextureAsset.

*/

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Hangout.Client
{

public class SpooledWWW : IDisposable
{
	// Limit on concurrent downloads.  Change this to do more or less at once.
	protected readonly static uint mMaxConcurrent = 20;
	private readonly static object mLockObject = new System.Object();
	protected static uint mNumConcurrent = 0;

	protected readonly string mURL;
	protected readonly WWWForm mForm;

	protected bool mIsDownloading = false;
	protected bool mIsFinished = false;

	protected WWW mReq = null;

	public SpooledWWW(string url, WWWForm form)
	{
		mURL = url;
		mForm = form;
		CheckInit();
	}

	public SpooledWWW(string url) : this(url, null)
	{
	}

	// Fire off the request ONLY if there aren't too many outstanding
	// Otherwise wait until there are less than mMaxConcurrent
	protected void CheckInit()
	{
		// See if we should start the download.	 Be thread-safe for kicks.
		lock(mLockObject)
		{
			if(mNumConcurrent < mMaxConcurrent)
			{
				mNumConcurrent++;
				mIsDownloading = true;
			}
		}

		if(mIsDownloading)
		{
			// We successfully acquired a download slot.
			// Actually start up the download.
			if(mForm != null)
			{
				mReq = new WWW(mURL,mForm);
			}
			else
			{
				mReq = new WWW(mURL);
			}
		}
	}

	// This should always be polled at an interval or the download may never start.
	public bool isDone
	{
		get
		{
			if(!mIsDownloading && !mIsFinished)
			{
				CheckInit();
				return false;
			}

			if(mReq.isDone && !mIsFinished)
			{
				mIsFinished = true;
				mNumConcurrent--;
			}
			
			return mIsFinished;
		}
	}
	
	// ...or you can poll this until it's non-null.
	public WWW result
	{
		get
		{
			if(this.isDone)
			{
				return mReq;
			}
			else
			{
				return null;
			}
		}
	}

	public virtual void Dispose()
	{
		// If we were mid-download, release our download slot so someone else can start.
		if(mIsDownloading && !mIsFinished)
		{
			mIsFinished = true;
			mNumConcurrent--;
		}
		if(mReq != null)
		{
			mReq.Dispose();
		}
	}

	~SpooledWWW()
	{
		Dispose();
	}
}

}
