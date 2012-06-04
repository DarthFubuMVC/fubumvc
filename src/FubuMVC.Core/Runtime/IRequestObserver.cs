using System;

namespace FubuMVC.Core.Runtime
{
    public interface IRequestObserver
    {
        void RecordLog(string message);
        void RecordLog(string message, params object[] args);
    }

    public class NulloRequestObserver : IRequestObserver
    {
        public void RecordLog(string message)
        {
            
        }

        public void RecordLog(string message, params object[] args)
        {
        }
    }
}