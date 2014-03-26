using FubuMVC.Core;
using FubuMVC.Core.Registration;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests
{
    [TestFixture]
    public class DiagnosticsExtensionTester
    {
        [Test]
        public void should_ignore_types_with_null_namespace()
        {
            var registry = new FubuRegistry(x =>
                                                {
                                                    x.Actions.IncludeType<ControllerWithoutANamespace>();
                                                    x.Import<DiagnosticsRegistration>();
                                                });
            // basically just make sure nothing blows up
            BehaviorGraph.BuildFrom(registry);
        }
    }
}

public class ControllerWithoutANamespace
{
    public HtmlDocument Index()
    {
        return new HtmlDocument();
    }
}