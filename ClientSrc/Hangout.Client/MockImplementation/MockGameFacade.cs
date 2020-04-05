using System;
using System.Collections.Generic;
using System.Text;

using PureMVC.Patterns;
using PureMVC.Interfaces;

using Hangout.Shared;

namespace Hangout.Client
{
    public class GameFacade : Facade, IFacade, IDisposable
    {
        private Scheduler mScheduler = new Scheduler();

        public void SchedulerLoop()
        {
            mScheduler.Update();
        }

        protected override void InitializeController()
        {
            base.InitializeController();

            GameFacade.Instance.RegisterMediator(new SchedulerMediator(mScheduler));
        }


        public void Dispose()
        {
           // SendNotification(APPLICATION_EXIT);
        }

    }
}
