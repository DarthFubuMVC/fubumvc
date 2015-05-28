using FubuMVC.Core;
using FubuMVC.RavenDb;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Embedded;

namespace FubuPersistence.Tests.RavenDb.Integration
{
    [TestFixture]
    public class Replacing_database_settings_tester
    {
        [Test]
        public void can_replace_with_new_database()
        {
            using (var runtime = FubuApplication.DefaultPolicies().StructureMap().Bootstrap())
            {
                var original = runtime.Factory.Get<IDocumentStore>();

                runtime.Factory.UseInMemoryDatastore();

                var current = runtime.Factory.Get<IDocumentStore>();
                current
                    .ShouldBeOfType<EmbeddableDocumentStore>()
                    .RunInMemory.ShouldBeTrue();

                current.ShouldNotBeTheSameAs(original);
            }
        }
    }
}