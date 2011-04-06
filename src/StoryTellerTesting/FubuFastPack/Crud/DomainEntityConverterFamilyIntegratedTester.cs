using FubuCore;
using FubuFastPack.Persistence;
using FubuFastPack.StructureMap;
using FubuMVC.Tests;
using FubuTestApplication;
using FubuTestApplication.Domain;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace IntegrationTesting.FubuFastPack.Crud
{
    [TestFixture]
    public class DomainEntityConverterFamilyIntegratedTester
    {
        private IContainer container;

        [SetUp]
        public void SetUp()
        {
            container = DatabaseDriver.GetFullFastPackContainer();
            container.Configure(x =>
            {
                x.UseOnDemandNHibernateTransactionBoundary();
            });
        }

        [Test]
        public void convert_null_to_null()
        {
            var converter = container.GetInstance<IObjectConverter>();
            converter.FromString<Site>(null).ShouldBeNull();
        }

        [Test]
        public void convert_from_guid()
        {
            using (var nested = container.GetNestedContainer())
            {
                var site = new Site();
                nested.GetInstance<IRepository>().Save(site);

                var converter = nested.GetInstance<IObjectConverter>();
                converter
                    .FromString<Site>(site.Id.ToString())
                    .ShouldBeOfType<Site>()
                    .Id.ShouldEqual(site.Id);
            }
        }
    }
}