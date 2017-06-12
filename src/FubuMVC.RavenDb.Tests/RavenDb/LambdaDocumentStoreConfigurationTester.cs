using FubuMVC.RavenDb.RavenDb;
using Raven.Client;
using Raven.Client.Document;
using Shouldly;
using StructureMap;
using Xunit;

namespace FubuMVC.RavenDb.Tests.RavenDb
{
    public class LambdaDocumentStoreConfigurationTester
    {
        [Fact]
        public void registers_and_uses_a_lambda_configuration_action()
        {
            var registry = new Registry();
            registry.RavenDbConfiguration(store => {
                store.Conventions.DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite;
            });

            var container = new Container(x => {
                x.IncludeRegistry<RavenDbRegistry>();
                x.IncludeRegistry(registry);
                x.For<RavenDbSettings>().Use(() => RavenDbSettings.InMemory());
            });

            container.GetInstance<IDocumentStore>().Conventions
                     .DefaultQueryingConsistency.ShouldBe(ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite);

            container.Dispose();
        }

        [Fact]
        public void registers_and_uses_a_lambda_configuration_action_2()
        {
            var registry = new Registry();
            registry.RavenDbConfiguration(store =>
            {
                store.Conventions.DefaultQueryingConsistency = ConsistencyOptions.None;
            });

            var container = new Container(x =>
            {
                x.IncludeRegistry<RavenDbRegistry>();
                x.IncludeRegistry(registry);
                x.For<RavenDbSettings>().Use(() => RavenDbSettings.InMemory());
            });

            container.GetInstance<IDocumentStore>().Conventions
                     .DefaultQueryingConsistency.ShouldBe(ConsistencyOptions.None);

            container.Dispose();
        }
    }
}
