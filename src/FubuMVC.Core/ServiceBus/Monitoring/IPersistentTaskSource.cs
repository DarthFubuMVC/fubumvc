using System;
using System.Collections.Generic;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public interface IPersistentTaskSource
    {
        string Protocol { get; }
        IEnumerable<Uri> PermanentTasks();

        IPersistentTask CreateTask(Uri uri);
    }
}