using FubuFastPack.NHibernate;
using IntegrationTesting.Domain;
using NUnit.Framework;
using StructureMap;
using FubuMVC.Tests;

namespace IntegrationTesting.FubuFastPack.NHibernate
{
    [TestFixture]
    public class find_type_by_persisted_class_name
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
        public void get_address_type()
        {
            source.Configuration().PersistedTypeByName("address").ShouldEqual(typeof (Address));
            source.Configuration().PersistedTypeByName("Address").ShouldEqual(typeof (Address));
        }

        [Test]
        public void get_site_type()
        {
            source.Configuration().PersistedTypeByName("Site").ShouldEqual(typeof(Site));
        }
    }
}