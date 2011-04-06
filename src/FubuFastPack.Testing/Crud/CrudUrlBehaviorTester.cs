using System;
using FubuFastPack.Crud;
using FubuFastPack.Testing.Security;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuFastPack.Testing.Crud
{
    [TestFixture]
    public class when_the_entity_is_new : InteractionContext<CrudUrlBehavior>
    {
        private EditEntityModel theInput;
        private IUrlRegistry theUrls;

        protected override void beforeEach()
        {
            theInput = new EditEntityModel(new User());
            theInput.Target.IsNew().ShouldBeTrue();


            MockFor<IFubuRequest>().Stub(x => x.Find<EditEntityModel>()).Return(new EditEntityModel[] { theInput });
            theUrls = this.UseStubUrlRegistry();

            ClassUnderTest.Invoke();
        }

        [Test]
        public void the_submit_url_should_be_set()
        {
            theInput.SubmitAction.ShouldEqual(theUrls.UrlFor(theInput));
        }
    }

    [TestFixture]
    public class when_the_entity_is_existing : InteractionContext<CrudUrlBehavior>
    {
        private EditEntityModel theInput;
        private IUrlRegistry theUrls;

        protected override void beforeEach()
        {
            theInput = new EditEntityModel(new User() { Id = Guid.NewGuid() });

            theInput.Target.IsNew().ShouldBeFalse();


            MockFor<IFubuRequest>().Stub(x => x.Find<EditEntityModel>()).Return(new EditEntityModel[] { theInput });
            theUrls = this.UseStubUrlRegistry();

            ClassUnderTest.Invoke();
        }

        [Test]
        public void the_property_update_url_should_be_applied()
        {
            theInput.PropertyUpdateUrl.ShouldEqual(theUrls.UrlForPropertyUpdate(typeof(User)));
        }
    }


}