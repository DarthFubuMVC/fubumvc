namespace FubuMVC.Core.Diagnostics
{
    public interface IRequestObserver
    {
        void RecordLog(string message);
        void RecordLog(string message, params object[] args);
    }
}