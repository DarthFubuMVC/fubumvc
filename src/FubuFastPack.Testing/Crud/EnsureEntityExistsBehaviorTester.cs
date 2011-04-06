using System;
using System.Linq;
using FubuCore;
using FubuFastPack.Crud;
using FubuFastPack.Persistence;
using FubuFastPack.Testing.Security;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuFastPack.Testing.Crud
{
    [TestFixture]
    public class when_ensuring_entity_exists_and_it_does : InteractionContext<EnsureEntityExistsBehavior<Site>>
    {
        private Site _site;
        private InMemoryFubuRequest _fubuRequest;

        protected override void beforeEach()
        {
            var repository = new InMemoryRepository(null);
            Services.Inject<IRepository>(repository);


            _site = new Site();
            repository.Save(_site);
            

            _fubuRequest = new InMemoryFubuRequest();
            _fubuRequest.Set(new ItemRequest { Id = _site.Id });
            Services.Inject<IFubuRequest>(_fubuRequest);

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_store_the_entity_in_the_request()
        {
            _fubuRequest.Find<Site>().FirstOrDefault().ShouldBeTheSameAs(_site);
        }
    }

    [TestFixture]
    public class when_ensuring_entity_exists_and_it_does_not : InteractionContext<EnsureEntityExistsBehavior<Site>>
    {
        private InMemoryFubuRequest _fubuRequest;
        private Guid _requestedId;

        protected override void beforeEach()
        {
            var repository = new InMemoryRepository(null);
            Services.Inject<IRepository>(repository);

            _fubuRequest = new InMemoryFubuRequest();
            _requestedId = Guid.NewGuid();
            _fubuRequest.Set(new ItemRequest { Id = _requestedId });
            Services.Inject<IFubuRequest>(_fubuRequest);
        }

        [Test]
        public void should_throw_exception_indicating_entity_does_not_exist()
        {
            var exception = typeof(NonExistentEntityException).ShouldBeThrownBy(() => ClassUnderTest.Invoke()).As<NonExistentEntityException>();
            exception.EntityType.ShouldEqual(typeof(Site));
            exception.EntityId.ShouldEqual(_requestedId);
        }
    }
}