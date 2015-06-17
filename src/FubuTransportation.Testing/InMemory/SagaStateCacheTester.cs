using System;
using FubuTransportation.InMemory;
using FubuTransportation.Testing.ScenarioSupport;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuTransportation.Testing.InMemory
{
    [TestFixture]
    public class SagaStateCacheTester
    {
        [Test]
        public void find_is_null_without_storing_anything()
        {
            new SagaStateCache<Message>().Find(Guid.NewGuid())
                .ShouldBeNull();
        }

        [Test]
        public void store_and_find()
        {
            var message1 = new Message();
            var message2 = new Message();
            var message3 = new Message();

            var cache = new SagaStateCache<Message>();
            cache.Store(message1.Id, message1);
            cache.Store(message2.Id, message2);
            cache.Store(message3.Id, message3);


            cache.Find(message1.Id).ShouldBeTheSameAs(message1);
            cache.Find(message2.Id).ShouldBeTheSameAs(message2);
            cache.Find(message3.Id).ShouldBeTheSameAs(message3);
        }

        [Test]
        public void delete_from_cache()
        {
            var message1 = new Message();
            var message2 = new Message();
            var message3 = new Message();

            var cache = new SagaStateCache<Message>();
            cache.Store(message1.Id, message1);
            cache.Store(message2.Id, message2);
            cache.Store(message3.Id, message3);

            cache.Delete(message1.Id);
            cache.Find(message1.Id).ShouldBeNull();

            cache.Find(message2.Id).ShouldBeTheSameAs(message2);
            cache.Find(message3.Id).ShouldBeTheSameAs(message3);
        }
    }
}