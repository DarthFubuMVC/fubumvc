using System.Linq;
using FubuMVC.RavenDb.InMemory;
using Shouldly;
using Xunit;

namespace FubuMVC.RavenDb.Tests.InMemory
{
    public class InMemoryPersistentResetTester
    {
        [Fact]
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