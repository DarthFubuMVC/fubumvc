using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;

namespace FubuMVC.Core.ServiceBus.Scheduling
{
    public class TaskScheduler : IScheduler
    {
        private readonly Task[] _tasks;
        private bool _stopped = true;

        public TaskScheduler(int taskCount)
        {
            _tasks = new Task[taskCount];
        }

        public int TaskCount {get { return _tasks.Length; }}
        public IEnumerable<Task> Tasks { get { return _tasks.Where(x => x != null).ToArray(); } } 

        public void Start(Action action)
        {
            _stopped = false;
            for (int i = 0; i < TaskCount; i++)
            {
                var task = Task.Factory.StartNew(action, TaskCreationOptions.LongRunning);
                _tasks[i] = task;
            }
        }

        public void Dispose()
        {
            if (_stopped)
                return;

            _stopped = true;
            Task.WaitAll(_tasks, 5.Seconds());
        }

        public static IScheduler Default()
        {
            return new TaskScheduler(1);
        }

        public override string ToString()
        {
            return "TaskScheduler Count: " + _tasks.Length;
        }
    }
}