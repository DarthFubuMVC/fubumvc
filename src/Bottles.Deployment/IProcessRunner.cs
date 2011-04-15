using System.Diagnostics;

namespace Bottles.Deployment
{
    public interface IProcessRunner
    {
        void Run(ProcessStartInfo info);
    }
}