using System;
using System.Collections.Generic;

namespace FubuTransportation.Monitoring
{
    public interface IPersistentTaskSource
    {
        string Protocol { get; }
        IEnumerable<Uri> PermanentTasks();

        IPersistentTask CreateTask(Uri uri);
    }
}