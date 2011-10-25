using System;

namespace FubuMVC.Spark.SparkModel.Sharing
{
    [Serializable]
    public class InvalidSyntaxException : Exception
    {
        public static readonly string USAGE =
            @"

  Valid usages:

  # comments

  export to [all|<provenance names>]
  import from <provenance names>
 
";

        public InvalidSyntaxException(string message) : base(message + USAGE) {}

        public InvalidSyntaxException(string message, Exception innerException)
            : base(message + USAGE, innerException) {}
    }
}