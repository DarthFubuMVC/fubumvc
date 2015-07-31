using FubuMVC.Core;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Embedded;
using Shouldly;

namespace FubuMVC.RavenDb.Tests.RavenDb.Integration
{
    [TestFixture]
    public class Replacing_database_settings_tester
    {
        [Test]
        public void can_replace_with_new_database()
        {
            using (var runtime = FubuRuntime.Basic())
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