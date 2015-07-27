using FubuMVC.RavenDb.RavenDb;
using NUnit.Framework;
using Raven.Client.Embedded;
using Raven.Imports.Newtonsoft.Json;
using Shouldly;

namespace FubuMVC.RavenDb.Tests.RavenDb
{
    [TestFixture]
    public class DocumentStoreBuilderTester
    {
        [Test]
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