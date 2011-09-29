using System;
using System.Diagnostics;

namespace Fubu
{
    public class ProcessFactory : IProcessFactory
    {
        public IProcess Create(Action<ProcessStartInfo> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException("configure");
            }

            var processInfo = new ProcessStartInfo();
            configure(processInfo);

            return new ProcessWrapper(processInfo);
        }
    }
}