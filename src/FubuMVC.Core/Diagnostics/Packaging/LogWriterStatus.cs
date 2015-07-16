using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics.Packaging
{
    /// <summary>
    /// Manages the log stack
    /// </summary>
    public class LogWriterStatus
    {
        private readonly Stack<IActivationLog> _logs = new Stack<IActivationLog>();

        public LogWriterStatus()
        {
            _logs.Push(new ActivationLog(new PerfTimer()));
        }

        public IActivationLog Current
        {
            get
            {
                return _logs.Peek();
            }
        }

        public void PushLog(IActivationLog log)
        {
            _logs.Push(log);
        }

        public void PopLog()
        {
            _logs.Pop();
        }
    }
}