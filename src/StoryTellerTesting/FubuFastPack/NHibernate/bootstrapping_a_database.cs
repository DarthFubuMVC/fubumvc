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

            using (var container = DatabaseDriver.TransactionalContainer())
            {
                container.GetInstance<ISession>().Save(address);
            }

            using (var container = DatabaseDriver.TransactionalContainer())
            {
                var address2 = container.GetInstance<ISession>().Load<Address>(address.Id);

                address.Line1.ShouldEqual(address2.Line1);
                address.Line2.ShouldEqual(address2.Line2);
            }
        }
    }
}