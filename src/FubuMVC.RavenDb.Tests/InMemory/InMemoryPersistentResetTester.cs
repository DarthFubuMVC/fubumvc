using System.Linq;
using FubuMVC.RavenDb.InMemory;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.RavenDb.Tests.InMemory
{
    [TestFixture]
    public class InMemoryPersistentResetTester
    {
        [Test]
        public void clear_all_state()
        {
            var persister = new InMemoryPersistor();
            persister.Persist(new City());
            persister.Persist(new City());
            persister.Persist(new City());
            persister.Persist(new City());
            persister.Persist(new City());
            persister.Persist(new City());

            var reset = new InMemoryPersistenceReset(persister);

            persister.LoadAll<City>().Any().ShouldBeTrue();

            reset.ClearPersistedState();

            persister.LoadAll<City>().Any().ShouldBeFalse();

        }
    }

    public class City : Entity
    {
        
    }
}