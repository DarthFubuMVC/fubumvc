using System;
using System.Runtime.Serialization;

namespace FubuMVC.Core.Packaging.Environment
{
    public class EnvironmentRunnerException : ApplicationException
    {
        public EnvironmentRunnerException()
        {
        }

        public EnvironmentRunnerException(string message) : base(message)
        {
        }

        protected EnvironmentRunnerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}