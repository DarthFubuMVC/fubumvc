using FubuMVC.RavenDb.RavenDb;
using Raven.Client.Embedded;
using Raven.Imports.Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace FubuMVC.RavenDb.Tests.RavenDb
{
    public class DocumentStoreBuilderTester
    {
        [Fact]
        public void simple_construction()
        {
            var settings = new RavenDbSettings
            {
                RunInMemory = true
            };

            var actions = new IDocumentStoreConfigurationAction[]
            {new CustomizeRavenJsonSerializer(new JsonConverter[0]),};

            var builder = new DocumentStoreBuilder(settings, actions);
            var store = builder.Build().ShouldBeOfType<EmbeddableDocumentStore>();
            store.RunInMemory.ShouldBeTrue();
            store.Conventions.CustomizeJsonSerializer.ShouldNotBeNull();

            store.Dispose();
        }
    }
}
