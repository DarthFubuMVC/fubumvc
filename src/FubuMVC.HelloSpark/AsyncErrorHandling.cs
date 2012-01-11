using System;
using FubuMVC.Core.Runtime;

namespace FubuMVC.HelloSpark
{
    public class AsyncExceptionHandler : IExceptionHandler
    {
        private readonly IOutputWriter _output;

        public AsyncExceptionHandler(IOutputWriter output)
        {
            _output = output;
        }

        public bool ShouldHandle(Exception exception)
        {
            return !(exception is DontHandleException);
        }

        public void Handle(Exception exception)
        {
            _output.WriteHtml("Handled Exception");
        }
    }

    public class DontHandleException : Exception
    {
        
    }
}