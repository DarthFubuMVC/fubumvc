using System;
using FubuCore.Binding;
using FubuFastPack.Crud;
using FubuFastPack.Domain;
using FubuFastPack.Persistence;
using FubuFastPack.Testing.Security;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuFastPack.Testing.Crud
{

    [IgnoreEntityInBinding]
    public class Address : DomainEntity
    {

    }

    public class NotesLog { }

    [TestFixture]
    public class EntityModelBinderTester : InteractionContext<EntityModelBinder>
    {
        [Test]
        public void should_match_entities()
        {
            ClassUnderTest.Matches(typeof(Case)).ShouldBeTrue();
            ClassUnderTest.Matches(typeof(Site)).ShouldBeTrue();
        }

        [Test]
        public void should_not_match_on_types_that_are_not_entities()
        {
            ClassUnderTest.Matches(GetType()).ShouldBeFalse();
        }

        [Test]
        public void should_not_match_on_address()
        {
            ClassUnderTest.Matches(typeof(Address)).ShouldBeFalse();
        }


        [Test]
        public void try_to_return_a_new_object_of_that_type_if_it_cannot_be_found_in_the_repository_for_child_object_binding_scenarios()
        {
            var theId = Guid.NewGuid();

            MockFor<IBindingContext>().Stub(x => x.Service<IRepository>()).Return(MockFor<IRepository>());
            MockFor<IBindingContext>().Stub(x => x.Service<IRequestData>()).Return(MockFor<IRequestData>());
            MockFor<IRequestData>().Stub(x => x.Value("Id")).Return(theId.ToString());

            MockFor<IRepository>().Stub(x => x.Find<User>(theId)).Return(null);

            ClassUnderTest.Bind(typeof(User), MockFor<IBindingContext>()).ShouldBeOfType<User>().ShouldNotBeNull();
        }
    }
}