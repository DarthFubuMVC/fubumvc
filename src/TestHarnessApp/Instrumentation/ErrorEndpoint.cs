using System;

namespace TestHarnessApp.Instrumentation
{
    public class ErrorEndpoint
    {
        public ErrorViewModel get_exception()
        {
            throw new Exception("boom");
        }
    }

    public class ErrorViewModel
    {
    }
}