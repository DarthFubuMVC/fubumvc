using FubuMVC.Diagnostics.Features.Html.Preview;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Features.Html
{
    [TestFixture]
    public class PreviewModelTypeResolverTester : InteractionContext<PreviewModelTypeResolver>
    {
        protected override void beforeEach()
        {
            Container.Inject(ObjectMother.DiagnosticsGraph());
        }

        [Test]
        public void should_try_gettype_first()
        {
            var type = typeof (SampleContextModel);
            ClassUnderTest
                .TypeFor(type.AssemblyQualifiedName)
                .ShouldEqual(type);
        }

        [Test]
        public void should_fallback_to_behavior_graph()
        {
            var type = typeof (Diagnostics.Features.Dashboard.DashboardModel);
            ClassUnderTest
                .TypeFor(type.FullName)
                .ShouldEqual(type);
        }
    }
}