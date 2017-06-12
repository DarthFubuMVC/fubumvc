using System;
using System.Linq;
using FubuMVC.RavenDb.Storage;
using FubuMVC.Tests.TestSupport;
using Rhino.Mocks;
using Shouldly;
using Xunit;

namespace FubuMVC.RavenDb.Tests.Storage
{
    public class GlobalEntityStorageTester : InteractionContext<EntityStorage<User>>
    {
        [Fact]
        public void find_delegates()
        {
            var user = new User(){
                Id = Guid.NewGuid()
            };

            MockFor<IPersistor>().Stub(x => x.Find<User>(user.Id))
                .Return(user);

            ClassUnderTest.Find(user.Id).ShouldBeTheSameAs(user);
        }

        [Fact]
        public void update_just_delegates()
        {
            var user = new User()
            {
                Id = Guid.NewGuid()
            };

            ClassUnderTest.Update(user);

            MockFor<IPersistor>().AssertWasCalled(x => x.Persist(user));
        }

        [Fact]
        public void remove_just_delegates()
        {
            var user = new User()
            {
                Id = Guid.NewGuid()
            };

            ClassUnderTest.Remove(user);

            MockFor<IPersistor>().AssertWasCalled(x => x.Remove(user));
        }

        [Fact]
        public void all_just_delegates()
        {
            IQueryable<User> queryable = new User[0].AsQueryable();

            MockFor<IPersistor>().Stub(x => x.LoadAll<User>()).Return(queryable);

            ClassUnderTest.All().ShouldBeTheSameAs(queryable);
        }

        [Fact]
        public void delete_all_delegates()
        {
            ClassUnderTest.DeleteAll();

            MockFor<IPersistor>().AssertWasCalled(x => x.DeleteAll<User>());
        }
    }
}
