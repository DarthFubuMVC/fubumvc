namespace FubuMVC.Core.Diagnostics.Runtime.Tracing
{
    public class NulloRequestLog : RequestLog
    {
        public override void AddLog(double requestTimeInMilliseconds, object log)
        {
            // Do nothing.  Nothing at all.
        }
    }
}