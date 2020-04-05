using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using Hangout.Shared;

namespace Hangout.Client
{
    public class SchedulerMediator : Mediator
    {
        private readonly IScheduler mScheduler;
        public IScheduler Scheduler
        {
            get { return mScheduler; }
        }

        public SchedulerMediator(IScheduler scheduler)
        {
            mScheduler = scheduler;
        }

    }
}
