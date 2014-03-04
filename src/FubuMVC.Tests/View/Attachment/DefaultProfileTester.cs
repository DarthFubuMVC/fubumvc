using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.View.Attachment
{
    [TestFixture]
    public class DefaultProfileTester
    {
        [Test]
        public void condition_type_is_Always()
        {
            new DefaultProfile().Condition.ShouldBeTheSameAs(Always.Flyweight);
        }

        [Test]
        public void filter_just_returns_the_same()
        {
            var views = new IViewToken[]{
                MockRepository.GenerateMock<IViewToken>(),
                MockRepository.GenerateMock<IViewToken>(),
                MockRepository.GenerateMock<IViewToken>(),
                MockRepository.GenerateMock<IViewToken>(),
                MockRepository.GenerateMock<IViewToken>(),
                MockRepository.GenerateMock<IViewToken>()
            };

            var original = new ViewBag(views);

            new DefaultProfile().Filter(original).ShouldBeTheSameAs(original);
        }
    }
}