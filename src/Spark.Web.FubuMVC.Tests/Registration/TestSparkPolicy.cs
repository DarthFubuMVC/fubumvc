using System;
using FubuMVC.Core.Registration.Nodes;
using Spark.Web.FubuMVC.Registration;

namespace Spark.Web.FubuMVC.Tests.Registration
{
    public class TestSparkPolicy : ISparkPolicy
    {
        public bool Matches(ActionCall call)
        {
            throw new NotImplementedException();
        }

        public string BuildViewLocator(ActionCall call)
        {
            throw new NotImplementedException();
        }

        public string BuildViewName(ActionCall call)
        {
            throw new NotImplementedException();
        }
    }
}