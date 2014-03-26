using System;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Elements.Builders;
using FubuMVC.Core.UI.Scenarios;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Scenarios
{
    [TestFixture]
    public class HtmlElementScenarioTester
    {
        [Test]
        public void simplest_possible_case()
        {
            var generator = HtmlElementScenario<Elements.Address>.For(definition =>
            {
                definition.Configure(x =>
                {
                    x.Displays.Add(r => true, new SpanDisplayBuilder());
                });
                definition.Model = new Elements.Address{
                    Address1 = "22 Cherry Tree Lane"
                };
            });

            generator.DisplayFor(x => x.Address1).ToString()
                .ShouldEqual("<span id=\"Address1\">22 Cherry Tree Lane</span>");
        }

        [Test]
        public void customize_display_formatting()
        {
            var generator = HtmlElementScenario<Elements.Address>.For(definition =>
            {
                definition.Display.IfIsType<string>().ConvertBy(start => "*" + start + "*");

                definition.Configure(x =>
                {
                    x.Displays.Add(r => true, new SpanDisplayBuilder());
                });
                definition.Model = new Elements.Address
                {
                    Address1 = "22 Cherry Tree Lane"
                };
            });

            generator.DisplayFor(x => x.Address1).ToString()
                .ShouldEqual("<span id=\"Address1\">*22 Cherry Tree Lane*</span>");
        }

        public class FakeElementNamingConvention : IElementNamingConvention
        {
            public string GetName(Type modelType, Accessor accessor)
            {
                return "*" + accessor.Name + "*";
            }
        }

        [Test]
        public void using_a_different_naming_convention()
        {
            var generator = HtmlElementScenario<Elements.Address>.For(definition =>
            {
                definition.Configure(x =>
                {
                    x.Displays.Add(r => true, new SpanDisplayBuilder());
                });

                definition.Naming = new FakeElementNamingConvention();

                definition.Model = new Elements.Address
                {
                    Address1 = "22 Cherry Tree Lane"
                };
            });

            generator.DisplayFor(x => x.Address1).ToString()
                .ShouldEqual("<span id=\"*Address1*\">22 Cherry Tree Lane</span>");
        }

        public class FakeService
        {
            public string Name { get; set; }
        }

        public class FakeBuilder : IElementBuilder
        {
            public bool Matches(ElementRequest subject)
            {
                return true;
            }

            public HtmlTag Build(ElementRequest request)
            {
                return new HtmlTag("div").Text(request.Get<FakeService>().Name);
            }
        }

        [Test]
        public void register_a_service()
        {
            var generator = HtmlElementScenario<Elements.Address>.For(definition =>
            {
                definition.Configure(x =>
                {
                    x.Displays.Add(r => true, new FakeBuilder());
                });

                definition.Model = new Elements.Address();
                definition.Services.Add(new FakeService{Name = "Jeremy"});
            });

            generator.DisplayFor(x => x.Address1).Text().ShouldEqual("Jeremy");
        }
    }
}