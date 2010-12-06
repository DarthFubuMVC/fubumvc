using System.IO;
using System.Linq;
using FubuFastPack.NHibernate;
using IntegrationTesting.Domain;
using NHibernate;
using NUnit.Framework;
using FubuMVC.Tests;

namespace IntegrationTesting.FubuFastPack.NHibernate
{
    [TestFixture]
    public class bootstrapping_a_database
    {
        [SetUp]
        public void SetUp()
        {
            DatabaseDriver.Bootstrap();
        }

        [Test]
        public void save_and_load_a_simple_entity()
        {
            var address = new Address(){
                Line1 = "a",
                Line2 = "b"
            };

            using (var container = DatabaseDriver.ContainerWithDatabase())
            {
                var session = container.GetInstance<ISession>();
                session.FlushMode = FlushMode.Always;
                
                session.SaveOrUpdate(address);
                session.Flush();


                session.CreateCriteria(typeof(Address)).List<Address>().Any()
                    .ShouldBeTrue();

                var address2 = session.Get<Address>(address.Id);

                address.Line1.ShouldEqual(address2.Line1);
                address.Line2.ShouldEqual(address2.Line2);
            }
        }

        [Test]
        public void smoke_test_writing_mappings()
        {
            if (Directory.Exists("mapping"))
            {
                Directory.Delete("mapping", true);
            }
            
            using (var container = DatabaseDriver.ContainerWithDatabase())
            {
                container.GetInstance<ISchemaWriter>().ExportMappingsTo("mapping");
            }

            Directory.Exists("mapping").ShouldBeTrue();
            Directory.GetFiles("mapping", "*.xml").Any().ShouldBeTrue();
        }
    }
}