using System.Diagnostics;
using System.IO;

namespace Fubu
{
    public class ProcessWrapper : IProcess
    {
        public ProcessStartInfo ProcessStartInfo { get; private set; }
        public Process WrappedProcess { get; private set; }

        public ProcessWrapper(ProcessStartInfo processStartInfo)
        {
            ProcessStartInfo = processStartInfo;
            WrappedProcess = new Process { StartInfo = processStartInfo };
        }

        public bool Start()
        {
            return WrappedProcess.Start();
        }

        public void WaitForExit()
        {
            WrappedProcess.WaitForExit();
        }

        public int ExitCode
        {
            get { return WrappedProcess.ExitCode; }
        }

        public string GetErrors()
        {
            return WrappedProcess
                .StandardError
                .ReadToEnd();
        }
    }
}