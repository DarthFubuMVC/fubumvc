using System;
using FubuFastPack.NHibernate;
using FubuFastPack.Persistence;
using IntegrationTesting.Domain;
using NUnit.Framework;
using StructureMap;
using FubuMVC.Tests;
using System.Linq;

namespace IntegrationTesting.FubuFastPack.Persistence
{
    [TestFixture]
    public class RepositorySmokeTester
    {
        private IContainer container;
        private IRepository repository;

        [SetUp]
        public void SetUp()
        {
            DatabaseDriver.Bootstrap();

            container = DatabaseDriver.ContainerWithDatabase();
            container.Configure(x =>
            {
                x.For<Func<IRepository, string, Site>>()
                    .Use((r, text) => r.FindBy<Site>(s => s.Name == text));
            });
            
            repository = container.GetInstance<IRepository>();
        }

        [TearDown]
        public void TearDown()
        {
            container.Dispose();
        }

        [Test]
        public void save_and_find_by_id()
        {
            var site = new Site{
                Name = "Jeremy's Site"
            };

            repository.Save(site);

            // silly test because it would just pull from the identity map,
            // but still...
            repository.Find<Site>(site.Id).ShouldNotBeNull();
        }

        [Test]
        public void find_by_path()
        {
            var site = new Site
            {
                Name = "999"
            };


            repository.Save(site);

            var path = "Site/" + site.Id;

            repository.FindByPath(path).ShouldBeOfType<Site>().Name.ShouldEqual("999");
        }

        [Test]
        public void find_by_path_for_unknown_entity_should_return_null()
        {
            var site = new Site
            {
                Name = "999"
            };


            repository.Save(site);

            var path = "Site/" + Guid.NewGuid();

            repository.FindByPath(path).ShouldBeNull();
        }


        [Test]
        public void find_by_path_for_unrecognized_id_format_should_throw()
        {
            typeof(ArgumentException).ShouldBeThrownBy(() => repository.FindByPath("Site/1234"))
                .Message.ShouldContain("valid GUID");
        }

        [Test]
        public void find_by_path_for_unknown_type_should_throw()
        {
            typeof (ArgumentException).ShouldBeThrownBy(() => repository.FindByPath("Unkown/" + Guid.NewGuid()))
                .Message.ShouldContain("unknown entity type");
        }

        [Test]
        public void save_and_find_by_an_expression()
        {
            var site = new Site
            {
                Name = "001"
            };

            repository.Save(site);

            repository.FlushChanges();

            repository.FindBy<Site>(x => x.Name == "001").Name.ShouldEqual("001");
        }

        [Test]
        public void save_and_find_by_text()
        {
            var site = new Site
            {
                Name = "002"
            };

            repository.Save(site);
            repository.FlushChanges();

            // See the Func above where I register the "finder" for Site
            repository.Find<Site>("002").Name.ShouldEqual("002");
        }

        [Test]
        public void get_all_smoke_test()
        {
            repository.Save(new Site{Name = "003"});
            repository.Save(new Site{Name = "004"});
            repository.Save(new Site{Name = "005"});
            repository.FlushChanges();

            repository.GetAll<Site>().Count().ShouldBeGreaterThan(2);
        }

        [Test]
        public void smoke_test_query_methods()
        {
            repository.Save(new Site { Name = "006" });
            repository.Save(new Site { Name = "007" });
            repository.Save(new Site { Name = "008" });
            repository.FlushChanges();

            repository.Query<Site>().Where(x => x.Name.StartsWith("00")).Any().ShouldBeTrue();

            repository.Query<Site>(x => x.Name.StartsWith("00")).Any().ShouldBeTrue();

            repository.Query<Site>(new SiteQuery()).Count().ShouldEqual(1);
        }
    }

    public class SiteQuery : IQueryExpression<Site>
    {
        public IQueryable<Site> Apply(IQueryable<Site> input)
        {
            return input.Where(x => x.Name.EndsWith("7"));
        }
    }
}