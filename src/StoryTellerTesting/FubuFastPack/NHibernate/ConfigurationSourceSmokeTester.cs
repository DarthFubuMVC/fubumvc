using FubuFastPack.NHibernate;
using NUnit.Framework;
using StructureMap;
using FubuMVC.Tests;

namespace IntegrationTesting.FubuFastPack.NHibernate
{
    [TestFixture]
    public class ConfigurationSourceSmokeTester
    {
        private IContainer container;
        private IConfigurationSource source;

        [SetUp]
        public void SetUp()
        {
            DatabaseDriver.Bootstrap();

            container = DatabaseDriver.ContainerWithDatabase();
            source = container.GetInstance<IConfigurationSource>();
        }

        [TearDown]
        public void TearDown()
        {
            container.Dispose();
        }

        [Test]
        public void build_the_persistence_model()
        {
            source.Model().ShouldNotBeNull();
        }

        [Test]
        public void build_the_configuration()
        {
            source.Configuration().ShouldNotBeNull();
        }
    }
}