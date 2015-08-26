using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Http.Owin
{
    [TestFixture]
    public class OwinServiceArgumentsTester
    {
        [Test]
        public void registers_the_environment_dictionary_itself()
        {
            var environment = new Dictionary<string, object>();

            var routeData = new RouteData();

            var arguments = new OwinServiceArguments(routeData, environment);

            arguments.Get<IDictionary<string, object>>()
                .ShouldBeSameAs(environment);
        }

        [Test]
        public void register_the_execution_log_if_it_exists()
        {
            var log = new ChainExecutionLog();
            var environment = new Dictionary<string, object>();
            environment.Log(log);

            var routeData = new RouteData();

            var arguments = new OwinServiceArguments(routeData, environment);
        
            arguments.Get<IChainExecutionLog>()
                .ShouldBeSameAs(log);
        }
    }
}