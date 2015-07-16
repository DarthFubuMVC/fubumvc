using System;

namespace FubuMVC.Core.Diagnostics.Packaging
{
    /// <summary>
    /// Provides static access to the 'current' IActivationLog
    /// </summary>
    public static class LogWriter
    {
        private static readonly LogWriterStatus _status = new LogWriterStatus();

        public static void WithLog(IActivationLog log, Action action)
        {
            _status.PushLog(log);
            try
            {
                log.Execute(action);
            }
            catch (Exception ex)
            {
                log.MarkFailure(ex);
            }
            finally
            {
                _status.PopLog();
            }
        }

        public static IActivationLog Current
        {
            get { return _status.Current; }
        }
    }
}