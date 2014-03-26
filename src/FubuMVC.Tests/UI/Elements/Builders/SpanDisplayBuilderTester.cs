using FubuCore;
using FubuCore.Formatting;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Elements.Builders;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Elements.Builders
{
    [TestFixture]
    public class SpanDisplayBuilderTester
    {
        [Test]
        public void displays_string_value_inside_of_a_span()
        {
            var stringifier = new Stringifier();

            stringifier.AddStrategy(new StringifierStrategy
            {
                Matches = r => true,
                StringFunction = r => "*" + r.RawValue + "*"
            });

            var services = new InMemoryServiceLocator();
            services.Add(stringifier);
            services.Add<IDisplayFormatter>(new DisplayFormatter(services, stringifier));

            var request = ElementRequest.For<ElementRequestTester.Model1>(new ElementRequestTester.Model1 { Child = new ElementRequestTester.Model2 { Name = "Little Lindsey" } }, x => x.Child.Name);
            request.Attach(services);
            request.ElementId = "something";

            var tag = new SpanDisplayBuilder().Build(request);
            tag.ToString().ShouldEqual("<span id=\"something\">*Little Lindsey*</span>");
        }
    }
}