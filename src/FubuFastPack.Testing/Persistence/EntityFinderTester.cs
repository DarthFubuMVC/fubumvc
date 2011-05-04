using System;
using FubuFastPack.Crud;
using FubuFastPack.Persistence;
using FubuFastPack.Testing.Security;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuFastPack.Testing.Persistence
{
    [TestFixture]
    public class EntityFinderTester : InteractionContext<DomainEntityFinder<Case>>
    {
        private Guid _id = Guid.NewGuid();
        private Case _theCase;
        private EntityFindViewModel _output;
        private object theFlattenedObject;

        protected override void beforeEach()
        {

            _theCase = new Case().WithId();

            theFlattenedObject = new object();

            MockFor<IFlattener>().Stub(x => x.Flatten(_theCase)).Return(theFlattenedObject);
            MockFor<IRepository>().Stub(r => r.Find<Case>(_id)).Return(_theCase);
            _output = ClassUnderTest.Find(new FindItemRequest<Case> { Id = _id });
        }

        [Test]
        public void should_return_the_flattened_entity_for_given_id()
        {
            _output.Model.ShouldBeTheSameAs(theFlattenedObject);
        }
    }
}