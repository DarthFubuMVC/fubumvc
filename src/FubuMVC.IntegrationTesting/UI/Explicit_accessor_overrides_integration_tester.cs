using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using HtmlTags;
using HtmlTags.Conventions;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.IntegrationTesting.UI
{
    [TestFixture]
    public class Explicit_accessor_overrides_integration_tester
    {
        public class TestRegistry : FubuRegistry
        {
            public TestRegistry()
            {
                Actions.IncludeType<OverrideEndpoint>();

                // The profile has to exist before accessors can work
                Import<HtmlConventionRegistry>(x =>
                {
                    x.Profile("Green", profile =>
                    {
                        profile.Displays.Always.BuildBy<ComplexBuilder>();
                    });
                });
            }
        }


        [Test]
        public void override_the_display_for_the_default()
        {
            TestHost.Scenario<TestRegistry>(_ => {
                _.Get.Input(new OverrideRequest { Category = ElementConstants.Display });

                _.ContentShouldBe(new BlueBuilder().Build(null).ToString());
            });
        }

        [Test]
        public void override_the_display_for_the_label()
        {
            TestHost.Scenario<TestRegistry>(_ =>
            {
                _.Get.Input(new OverrideRequest { Category = ElementConstants.Label });

                _.ContentShouldBe(new SimpleBuilder().Build(null).ToString());
            });
        }

        [Test]
        public void override_the_display_for_the_input()
        {
            TestHost.Scenario<TestRegistry>(_ =>
            {
                _.Get.Input(new OverrideRequest { Category = ElementConstants.Editor });

                _.ContentShouldBe(new ComplexBuilder().Build(null).ToString());
            });
        }

        [Test]
        public void override_display_by_profile()
        {
            TestHost.Scenario<TestRegistry>(_ => {
                _.Get.Input(new OverrideRequest { Category = ElementConstants.Display, Profile = "Green" });

                _.ContentShouldBe(new GreenBuilder().Build(null).ToString());
            });
        }
    }

    public class TestAccessorOverrides : OverridesFor<OverrideTarget>
    {
        public TestAccessorOverrides()
        {
            Property(x => x.Name)
                .DisplayBuilder<BlueBuilder>()
                .LabelBuilder<SimpleBuilder>()
                .InputBuilder<ComplexBuilder>()
                .DisplayBuilder<GreenBuilder>(profile:"Green");
        }
    }

    public class OverrideEndpoint
    {
        private readonly IElementGenerator<OverrideTarget> _generator;

        public OverrideEndpoint(IElementGenerator<OverrideTarget> generator)
        {
            _generator = generator;
        }

        public HtmlTag get_target_Category_Profile(OverrideRequest request)
        {
            var elemRequest = ElementRequest.For<OverrideTarget>(x => x.Name);
            if (request.Category == ElementConstants.Display)
            {
                return _generator.DisplayFor(elemRequest, profile: request.Profile);
            }

            if (request.Category == ElementConstants.Label)
            {
                return _generator.LabelFor(elemRequest, profile: request.Profile);
            }

            return _generator.InputFor(elemRequest, profile: request.Profile);
        }
    }

    public class OverrideRequest
    {
        public OverrideRequest()
        {
            Profile = TagConstants.Default;
        }

        public string Category { get; set; }
        public string Profile { get; set; }
    }



    public class OverrideTarget
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class SimpleBuilder : IElementBuilder
    {
        public HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("div").AddClass("simple");
        }
    }

    public class ComplexBuilder : IElementBuilder
    {
        public HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("div").AddClass("complex");
        }
    }

    public class DifferentBuilder : IElementBuilder
    {
        public HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("div").AddClass("different");
        }
    }

    public class GreenBuilder : IElementBuilder
    {
        public HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("div").AddClass("green");
        }
    }

    public class BlueBuilder : IElementBuilder
    {
        public HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("div").AddClass("blue");
        }
    }
}