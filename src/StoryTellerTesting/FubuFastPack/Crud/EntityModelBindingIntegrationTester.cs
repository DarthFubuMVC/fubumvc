using System;
using FubuCore;
using FubuCore.Binding;
using FubuFastPack.Crud;
using FubuFastPack.Persistence;
using FubuTestApplication;
using FubuTestApplication.Domain;
using NUnit.Framework;
using StructureMap;
using InMemoryRequestData = FubuMVC.Core.Runtime.InMemoryRequestData;
using FubuMVC.Tests;
using FubuFastPack.StructureMap;

namespace IntegrationTesting.FubuFastPack.Crud
{
    public class ModelWithEntity
    {
        public Site Site { get; set; }
    }

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


    [TestFixture]
    public class EntityModelBinderIntegrationTester
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
        public void bind_model_should_be_null_when_the_id_is_null()
        {
            
            using (var nested = container.GetNestedContainer())
            {
                var context = nested.GetInstance<IBindingContext>();
                var binder = new EntityModelBinder();

                binder.Bind(typeof (Site), context).ShouldBeOfType<Site>()
                    .Id.ShouldEqual(Guid.Empty);
            }
        }

        [Test]
        public void bind_model_should_return_the_correct_entity_if_the_Id_is_a_guid()
        {
            using (var nested = container.GetNestedContainer())
            {
                var site = new Site();
                nested.GetInstance<IRepository>().Save(site);
                requestData["Id"] = site.Id.ToString();

                var context = nested.GetInstance<IBindingContext>();
                var binder = new EntityModelBinder();

                binder.Bind(typeof (Site), context).ShouldBeOfType<Site>().Id.ShouldEqual(site.Id);
            }
        }
    }

}