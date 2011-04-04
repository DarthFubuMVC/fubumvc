using FubuCore.Binding;
using FubuFastPack.Persistence;
using FubuFastPack.StructureMap;
using FubuMVC.Tests;
using FubuTestApplication;
using FubuTestApplication.Domain;
using NUnit.Framework;
using StructureMap;
using InMemoryRequestData = FubuMVC.Core.Runtime.InMemoryRequestData;

namespace IntegrationTesting.FubuFastPack.Crud
{
    [TestFixture]
    public class EntityConversionFamilyIntegrationTester
    {
        private IContainer container;
        private InMemoryRequestData requestData;

        [SetUp]
        public void SetUp()
        {
            requestData = new InMemoryRequestData();

            container = DatabaseDriver.GetFullFastPackContainer();
            container.Configure(x =>
            {
                x.UseOnDemandNHibernateTransactionBoundary();
                x.For<IRequestData>().Use(requestData);
            });


        }

        [Test]
        public void convert_should_be_null_when_the_id_is_null()
        {
            using (var nested = container.GetNestedContainer())
            {
                var context = nested.GetInstance<IBindingContext>();
                var binder = nested.GetInstance<StandardModelBinder>();

                binder.Bind(typeof(ModelWithEntity), context).ShouldBeOfType<ModelWithEntity>()
                    .Site.ShouldBeNull();
            }
        }

        [Test]
        public void bind_model_should_return_the_correct_entity_if_the_Id_is_a_guid()
        {
            using (var nested = container.GetNestedContainer())
            {
                var site = new Site();
                nested.GetInstance<IRepository>().Save(site);
                requestData["Site"] = site.Id.ToString();

                var context = nested.GetInstance<IBindingContext>();
                var binder = nested.GetInstance<StandardModelBinder>();

                binder.Bind(typeof(ModelWithEntity), context)
                    .ShouldBeOfType<ModelWithEntity>()
                    .Site.Id.ShouldEqual(site.Id);
            }
        }
    }
}