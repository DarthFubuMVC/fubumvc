using System;
using System.Diagnostics;

namespace Bottles.Deployment
{
    public interface IProcessRunner
    {
        int Run(ProcessStartInfo info, TimeSpan waitDuration);
        int Run(ProcessStartInfo info);
    }
}