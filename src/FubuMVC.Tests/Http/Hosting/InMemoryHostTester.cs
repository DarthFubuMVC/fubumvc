using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Http.Scenarios;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Hosting
{
    [TestFixture]
    public class InMemoryHostTester
    {
        [Test]
        public void invoke_a_simple_string_endpoint()
        {
            using (var host = FubuApplication.DefaultPolicies().StructureMap().RunInMemory())
            {
                host.Send(r => r.RelativeUrl("memory/hello"))
                    .Body.ReadAsText().ShouldEqual("hello from the in memory host");
            }

        }

        [Test]
        public void using_scenario_with_string_text_and_relative_url()
        {
            using (var host = FubuApplication.DefaultPolicies().StructureMap().RunInMemory())
            {
                host.Scenario(x => {
                    x.Get.Url("memory/hello");
                })
                .Body.ReadAsText().ShouldEqual("hello from the in memory host");;

            }
        }

        [Test]
        public void using_scenario_with_controller_expression()
        {
            using (var host = FubuApplication.DefaultPolicies().StructureMap().RunInMemory())
            {
                host.Scenario(x =>
                {
                    x.Get.Action<InMemoryEndpoint>(e => e.get_memory_hello());
                })
                .Body.ReadAsText().ShouldEqual("hello from the in memory host"); ;

            }
        }

        [Test]
        public void using_scenario_with_input_model()
        {
            using (var host = FubuApplication.DefaultPolicies().StructureMap().RunInMemory())
            {
                host.Scenario(x =>
                {
                    x.Get.Input(new InMemoryInput{Color = "Red"});
                })
                .Body.ReadAsText().ShouldEqual("The color is Red"); ;


                host.Scenario(x =>
                {
                    x.Get.Input(new InMemoryInput { Color = "Orange" });
                })
                .Body.ReadAsText().ShouldEqual("The color is Orange"); ;

            }


        }


        [Test]
        public void using_scenario_with_input_model_as_marker()
        {
            using (var host = FubuApplication.DefaultPolicies().StructureMap().RunInMemory())
            {
                host.Scenario(x =>
                {
                    x.Get.Input<MarkerInput>();
                })
                .Body.ReadAsText().ShouldEqual("just the marker"); ;
            }
        }

        [Test]
        public void using_scenario_with_ContentShouldContain_declaration_happy_path()
        {
            using (var host = FubuApplication.DefaultPolicies().StructureMap().RunInMemory())
            {
                host.Scenario(x => {
                    x.Get.Input<MarkerInput>();
                    x.ContentShouldContain("just the marker");
                });
            }
        }

        [Test]
        public void using_scenario_with_ContentShouldContain_declaration_sad_path()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() => {
                using (var host = FubuApplication.DefaultPolicies().StructureMap().RunInMemory())
                {
                    host.Scenario(x =>
                    {
                        x.Get.Input<MarkerInput>();
                        x.ContentShouldContain("wrong text");
                    });
                }
            });

            ex.Message.ShouldContain("The response body does not contain expected text \"wrong text\"");
        }
    }

    public class InMemoryEndpoint
    {
        public string get_memory_hello()
        {
            return "hello from the in memory host";
        }

        public string get_memory_color_Color(InMemoryInput input)
        {
            return "The color is " + input.Color;
        }

        public string get_memory_marker_text(MarkerInput input)
        {
            return "just the marker";
        }
    }

    public class InMemoryInput
    {
        public string Color { get; set; }
    }

    public class MarkerInput
    {
        
    }
}