using System;
using System.Diagnostics;

namespace Fubu
{
    public interface IProcessFactory
    {
        IProcess Create(Action<ProcessStartInfo> configure);
    }
}