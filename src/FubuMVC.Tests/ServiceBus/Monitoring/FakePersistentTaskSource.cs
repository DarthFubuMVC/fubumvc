using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Monitoring;

namespace FubuMVC.Tests.ServiceBus.Monitoring
{
    public class FakePersistentTaskSource : IPersistentTaskSource
    {
        private readonly Cache<string, FakePersistentTask> _tasks
            = new Cache<string, FakePersistentTask>();

        public FakePersistentTaskSource(string protocol)
        {
            Protocol = protocol;
            _tasks.OnMissing = name => {
                var uri = "{0}://{1}".ToFormat(Protocol, name).ToUri();
                return new FakePersistentTask(uri);
            };
        }

        public FakePersistentTask this[string name]
        {
            get { return _tasks[name]; }
        }

        public string Protocol { get; private set; }

        public IEnumerable<FakePersistentTask> FakeTasks()
        {
            return _tasks;
        } 

        public IEnumerable<Uri> PermanentTasks()
        {
            return _tasks.Select(x => x.Subject);
        }

        public IPersistentTask CreateTask(Uri uri)
        {
            var name = uri.Host;

            return _tasks.Has(name) ? _tasks[name] : null;
        }

        public FakePersistentTask AddTask(string key)
        {
            return _tasks[key];
        }
    }
}