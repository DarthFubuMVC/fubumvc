using FubuMVC.Core;
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
                                                    x.Applies.ToThisAssembly();
                                                    x.Actions.IncludeType<ControllerWithoutANamespace>();
                                                    x.Import<AdvancedDiagnosticsRegistry>();
                                                });
            // basically just make sure nothing blows up
            registry
                .BuildGraph();
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