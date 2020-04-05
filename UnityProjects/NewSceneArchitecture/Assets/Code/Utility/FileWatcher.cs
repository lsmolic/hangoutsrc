/**  --------------------------------------------------------  *
 *   FileWatcher.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/08/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client
{
	public class FileWatcher
	{
		private readonly string mFilename;
		private const float SCAN_INTERVAL = 0.1f;
		private readonly ITask mScanTask;
		
		private Hangout.Shared.Action mOnChanged = null;
		public Hangout.Shared.Action Changed 
		{
			get { return mOnChanged; }
			set { mOnChanged += value; }
		}
		
		private IEnumerator<IYieldInstruction> ScanFileForChanges(DateTime initialLastAccess)
		{			
			while(Application.isEditor)
			{
				FileInfo fileInfo = new FileInfo(mFilename);
				if(fileInfo.LastAccessTime != initialLastAccess) 
				{
					if(mOnChanged != null)
					{
						mOnChanged();
					}
					initialLastAccess = fileInfo.LastAccessTime;
				}
				
				yield return new YieldForSeconds(SCAN_INTERVAL);
			}
		}
		
		public FileWatcher(FileInfo file) 
		{
			mFilename = file.FullName;
			
			DateTime lastAccess = file.LastAccessTime;
            IScheduler scheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			mScanTask = scheduler.StartCoroutine(ScanFileForChanges(lastAccess));
		}

		public FileWatcher(FileInfo file, Hangout.Shared.Action onChanged)
			: this(file)
		{
			mOnChanged = onChanged;
		}

		~FileWatcher()
		{
			mScanTask.Exit();
		}

		public override string ToString()
		{
			return "FileWatcher(" + mFilename + ")";
		}
	}
}
