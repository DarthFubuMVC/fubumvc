using FubuMVC.Core.Continuations;

namespace Spark.Web.FubuMVC.Tests.Registration
{
    public class SampleEndpoint
    {
        public SampleOutput Get(SampleInput input)
        {
            return new SampleOutput();
        }

        public FubuContinuation Post()
        {
            return FubuContinuation.RedirectTo(new SampleInput());
        }
    }
}