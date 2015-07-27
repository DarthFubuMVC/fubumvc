using System;
using System.Linq;
using FubuMVC.RavenDb.Storage;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.RavenDb.Tests.Storage
{
    [TestFixture]
    public class GlobalEntityStorageTester : InteractionContext<EntityStorage<User>>
    {
        [Test]
        public void find_delegates()
        {
            var user = new User(){
                Id = Guid.NewGuid()
            };

            MockFor<IPersistor>().Stub(x => x.Find<User>(user.Id))
                .Return(user);

            ClassUnderTest.Find(user.Id).ShouldBeTheSameAs(user);
        }

        [Test]
        public void update_just_delegates()
        {
            var user = new User()
            {
                Id = Guid.NewGuid()
            };

            ClassUnderTest.Update(user);

            MockFor<IPersistor>().AssertWasCalled(x => x.Persist(user));
        }

        [Test]
        public void remove_just_delegates()
        {
            var user = new User()
            {
                Id = Guid.NewGuid()
            };

            ClassUnderTest.Remove(user);

            MockFor<IPersistor>().AssertWasCalled(x => x.Remove(user));
        }

        [Test]
        public void all_just_delegates()
        {
            IQueryable<User> queryable = new User[0].AsQueryable();

            MockFor<IPersistor>().Stub(x => x.LoadAll<User>()).Return(queryable);

            ClassUnderTest.All().ShouldBeTheSameAs(queryable);
        }

        [Test]
        public void delete_all_delegates()
        {
            ClassUnderTest.DeleteAll();

            MockFor<IPersistor>().AssertWasCalled(x => x.DeleteAll<User>());
        }
    }
}