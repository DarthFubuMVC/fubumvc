using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Hosting
{
    [TestFixture]
    public class InMemoryHostTester
    {
        [Test]
        public void invoke_a_simple_string_endpoint()
        {
            using (var host = FubuApplication.DefaultPolicies().StructureMap().RunInMemory())
            {
                host.Send(r => r.RelativeUrl("memory/hello"))
                    .ReadAsText().ShouldEqual("hello from the in memory host");
            }
        }
    }

    public class InMemoryEndpoint
    {
        public string get_memory_hello()
        {
            return "hello from the in memory host";
        }
    }
}