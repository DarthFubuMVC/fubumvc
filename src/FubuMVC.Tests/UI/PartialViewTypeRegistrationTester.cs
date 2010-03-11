using FubuMVC.Core.View;
using FubuMVC.UI;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class PartialViewTypeRegistrationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _registry = new PartialViewTypeRegistry();
            _registry.For<FakePartialModel>().Use<FakePartialView>();
        }

        #endregion

        private PartialViewTypeRegistry _registry;

        [Test]
        public void should_contain_registered_partial_view_type()
        {
            Assert.That(_registry.HasPartialViewTypeFor<FakePartialModel>());
        }

        [Test]
        public void should_render_registered_partial_view_type()
        {
            Assert.That(_registry.GetPartialViewTypeFor<FakePartialModel>(), Is.EqualTo(typeof (FakePartialView)));
        }
    }

    public class FakePartialView : FubuPage
    {
    }

    public class FakePartialModel
    {
    }
}