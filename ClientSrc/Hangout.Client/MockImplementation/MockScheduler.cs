using System;
using System.Collections.Generic;
using Hangout.Shared;

namespace Hangout.Client
{
    public class Scheduler : IScheduler
    {
        private class SchedulerTask : ITask
        {
            private Scheduler mOwner;
            private IEnumerator<IYieldInstruction> mCoroutine;

            public SchedulerTask(Scheduler owner, IEnumerator<IYieldInstruction> coroutine)
            {
                if (owner == null)
                {
                    throw new ArgumentNullException("owner");
                }

                if (coroutine == null)
                {
                    throw new ArgumentNullException("coroutine");
                }

                mOwner = owner;
                mCoroutine = coroutine;
            }

            public bool RunInFixedUpdateLoop
            {
                get
                {
                    bool result = false;
                    if (mCoroutine != null)
                    {
                        result = mCoroutine.Current is IFixedYieldInstruction;
                    }
                    return result;
                }
            }

            /// Returns false when the coroutine has completed
            public bool Step()
            {
                bool result = mCoroutine.MoveNext();
                if (!result)
                {
                    mCoroutine = null;
                }

                return result;
            }

            public bool Ready
            {
                get
                {
                    bool result = false;
                    if (mCoroutine != null && !mExited)
                    {
                        result = mCoroutine.Current.Ready;
                    }
                    return result;
                }
            }

            private Hangout.Shared.Action mOnExit = null;
            public void AddOnExitAction(Hangout.Shared.Action callback)
            {
                mOnExit += callback;
            }

            public bool IsRunning
            {
                get { return mCoroutine != null; }
            }

            private bool mExited = false;
            public void Exit()
            {
                if (!mExited)
                {
                    if (mOnExit != null)
                    {
                        mOnExit();
                    }

                    mOwner.TaskExited(this);
                    mExited = true;
                }
            }
            
            public void ForceExit()
            {
				mExited = true;
            }

            public override string ToString()
            {
                return "SchedulerTask: " + mCoroutine.ToString();
            }
        }

        private List<SchedulerTask> mPendingTasks = new List<SchedulerTask>();
        private List<SchedulerTask> mPendingFixedUpdateTasks = new List<SchedulerTask>();

        private void TaskExited(SchedulerTask task)
        {
            mPendingTasks.Remove(task);
            mPendingFixedUpdateTasks.Remove(task);
        }

        public ITask StartCoroutine(IEnumerator<IYieldInstruction> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            ITask result;

            // IEnumerators start 'before the first location', so calling this once here gets the task ready to execute
            if (task.MoveNext())
            {
                SchedulerTask schedulerTask = new SchedulerTask(this, task);
                result = schedulerTask;
                if (task.Current is IFixedYieldInstruction)
                {
                    mPendingFixedUpdateTasks.Add(schedulerTask);
                }
                else
                {
                    mPendingTasks.Add(schedulerTask);
                }
            }
            else
            {
                // This coroutine never yielded, so it's execution is complete
                result = new EmptyTask();
            }

            return result;
        }

        private class EmptyTask : ITask
        {
            private Hangout.Shared.Action mOnExit = null;
            public bool IsRunning
            {
                get { return false; }
            }

            public void AddOnExitAction(Action onExit)
            {
                mOnExit += onExit;
            }

            public void Exit()
            {
                if (mOnExit != null)
                {
                    mOnExit();
                }
            }
            
            public void ForceExit()
            {
				
            }
        }

        private ILogger mLogger = null;
        public ILogger Logger
        {
            set { mLogger = value; }
        }

        public void FixedUpdate()
        {
            List<SchedulerTask> completedTasks = new List<SchedulerTask>();
            List<SchedulerTask> moveToNormalUpdate = new List<SchedulerTask>();
            List<SchedulerTask> tasks = new List<SchedulerTask>(mPendingFixedUpdateTasks);

            foreach (SchedulerTask coroutine in tasks)
            {
                try
                {
                    if (!coroutine.RunInFixedUpdateLoop)
                    {
                        throw new Exception("Attempting to run a non-FixedUpdate Yield Instruction during FixedUpdate.");
                    }

                    if (!coroutine.Step())
                    {
                        completedTasks.Add(coroutine);
                    }
                    else if (!coroutine.RunInFixedUpdateLoop)
                    {
                        moveToNormalUpdate.Add(coroutine);
                    }
                }
                catch (System.Exception e)
                {
                    mPendingFixedUpdateTasks.Remove(coroutine);
                    if (mLogger != null)
                    {
                        mLogger.Log(e.ToString(), LogLevel.Error);
                    }
                    else
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }

            try
            {
                foreach (SchedulerTask task in completedTasks)
                {
                    task.Exit();
                }

            }
            catch (System.Exception e)
            {
                if (mLogger != null)
                {
                    mLogger.Log(e.ToString(), LogLevel.Error);
                }
                else
                {
                    Console.WriteLine(e.ToString());
                }
            }

            foreach (SchedulerTask task in moveToNormalUpdate)
            {
                mPendingTasks.Add(task);
                mPendingFixedUpdateTasks.Remove(task);
            }
        }

        public void Update()
        {
            List<SchedulerTask> completedTasks = new List<SchedulerTask>();
            List<SchedulerTask> moveToFixedUpdate = new List<SchedulerTask>();

            // TODO: This list copy is unnecessary, we could use a datastructure
            //		  that you can remove from while iterating.
            List<SchedulerTask> tasks = new List<SchedulerTask>(mPendingTasks);

            foreach (SchedulerTask coroutine in tasks)
            {
                try
                {
                    if (coroutine.RunInFixedUpdateLoop)
                    {
                        throw new Exception("Attempting to run a FixedUpdate Yield Instruction during Update.");
                    }


                    if (coroutine.Ready)
                    {
                        if (!coroutine.Step())
                        {
                            completedTasks.Add(coroutine);
                        }
                        else if (coroutine.RunInFixedUpdateLoop)
                        {
                            moveToFixedUpdate.Add(coroutine);
                        }
                    }
                }
                catch (Exception e)
                {
                    mPendingTasks.Remove(coroutine);
                    if (mLogger != null)
                    {
                        mLogger.Log(e.ToString(), LogLevel.Error);
                    }
                    else
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }

            try
            {
                foreach (SchedulerTask task in completedTasks)
                {
                    task.Exit();
                }
            }
            catch (System.Exception e)
            {
                if (mLogger != null)
                {
                    mLogger.Log(e.ToString(), LogLevel.Error);
                }
                else
                {
                    Console.WriteLine(e.ToString());
                }
            }

            foreach (SchedulerTask task in moveToFixedUpdate)
            {
                mPendingFixedUpdateTasks.Add(task);
                mPendingTasks.Remove(task);
            }
        }

        public void ShowTasks(ICollection<string> args)
        {
            Console.WriteLine(String.Format("Tasks:{0}", LogLevel.Info));
            foreach (SchedulerTask task in mPendingTasks)
            {
                Console.WriteLine(String.Format("{0} {1}", task, LogLevel.Info));
            }

            foreach (SchedulerTask task in mPendingFixedUpdateTasks)
            {
                Console.WriteLine(String.Format("{0} {1}", task, LogLevel.Info));
            }
        }
    }

    public interface IFixedYieldInstruction : IYieldInstruction
    {
    }
}