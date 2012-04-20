using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.WebForms.Testing
{
    [TestFixture]
    public class when_registered_partial_view_type
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _registry = new PartialViewTypeRegistry();
            var expression = new PartialViewTypeRegistrationExpression(_registry);
            expression.For<FakePartialModel>().Use<FakePartialView>();
        }

        #endregion

        private IPartialViewTypeRegistry _registry;

        [Test]
        public void should_contain_registered_partial_view_type()
        {
            _registry.HasPartialViewTypeFor<FakePartialModel>().ShouldBeTrue();
        }

        [Test]
        public void should_render_registered_partial_view_type()
        {
            var partialViewType = _registry.GetPartialViewTypeFor<FakePartialModel>().ShouldNotBeNull();
            partialViewType.ShouldEqual(typeof (FakePartialView));
        }
    }

    public class FakePartialView : FubuPage
    {
    }

    public class FakePartialModel
    {
    }
}