using FubuMVC.Diagnostics.Features.Html.Preview;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Features.Html
{
    [TestFixture]
    public class PreviewModelActivatorTester
    {
        [Test]
        public void should_create_default_instance()
        {
            new PreviewModelActivator()
                .Activate(typeof (SampleContextModel))
                .ShouldNotBeNull();
        }
    }
}