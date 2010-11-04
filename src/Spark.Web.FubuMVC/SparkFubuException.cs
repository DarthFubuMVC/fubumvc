using System;
using System.Runtime.Serialization;
using FubuMVC.Core;

namespace Spark.Web.FubuMVC
{
    public class SparkFubuException : FubuException
    {
        public SparkFubuException(int errorCode, string message) : base(errorCode, message)
        {
        }

        protected SparkFubuException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SparkFubuException(int errorCode, Exception inner, string template, params string[] substitutions) : base(errorCode, inner, template, substitutions)
        {
        }

        public SparkFubuException(int errorCode, string template, params string[] substitutions) : base(errorCode, template, substitutions)
        {
        }
    }
}