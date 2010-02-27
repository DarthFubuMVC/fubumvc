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
            _renderer = new PartialViewTypeRenderer();
            _renderer.For<FakePartialModel>().Use<FakePartialView>();
        }

        #endregion

        private PartialViewTypeRenderer _renderer;

        [Test]
        public void should_contain_registered_partial_view_type()
        {
            Assert.That(_renderer.HasPartialViewTypeFor<FakePartialModel>());
        }

        [Test]
        public void should_render_registered_partial_view_type()
        {
            Assert.That(_renderer.GetPartialViewTypeFor<FakePartialModel>(), Is.EqualTo(typeof (FakePartialView)));
        }
    }

    public class FakePartialView : FubuPage
    {
    }

    public class FakePartialModel
    {
    }
}