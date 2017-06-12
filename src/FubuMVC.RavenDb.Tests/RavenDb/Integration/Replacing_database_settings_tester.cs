using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using Raven.Client;
using Raven.Client.Embedded;
using Shouldly;
using Xunit;

namespace FubuMVC.RavenDb.Tests.RavenDb.Integration
{
    public class Replacing_database_settings_tester
    {
        [Fact]
        public void can_replace_with_new_database()
        {
            using (var runtime = FubuRuntime.Basic())
            {
                var original = runtime.Get<IDocumentStore>();

                runtime.Get<IServiceFactory>().UseInMemoryDatastore();

                var current = runtime.Get<IDocumentStore>();
                current
                    .ShouldBeOfType<EmbeddableDocumentStore>()
                    .RunInMemory.ShouldBeTrue();

                current.ShouldNotBeTheSameAs(original);
            }
        }
    }
}
