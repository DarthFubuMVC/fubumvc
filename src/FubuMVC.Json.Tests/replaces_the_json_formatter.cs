using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Json.Tests
{
    [TestFixture]
    public class replaces_the_json_formatter
    {
        [Test]
        public void does_replace_the_built_in_json_formatter()
        {
            using (var runtime = FubuApplication.DefaultPolicies().StructureMap().Bootstrap())
            {
                var settings = runtime.Factory.Get<ConnegSettings>();
                settings.Formatters.First().ShouldBeOfType<NewtonsoftJsonFormatter>();
                settings.Formatters.OfType<JsonSerializer>().Any().ShouldBeFalse();
            }
        }
    }
}