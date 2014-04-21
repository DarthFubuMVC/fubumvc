using System;
using System.Net;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Http.Scenarios;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http.Hosting
{
    [TestFixture]
    public class InMemoryHostTester
    {
        private InMemoryHost host;

        [TestFixtureSetUp]
        public void SetUp()
        {
            host = newHost();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            host.SafeDispose();
        }

        [Test]
        public void invoke_a_simple_string_endpoint()
        {
            host.Send(r => r.RelativeUrl("memory/hello"))
                .Body.ReadAsText().ShouldEqual("hello from the in memory host");
        }

        private static InMemoryHost newHost()
        {
            return FubuApplication.DefaultPolicies().StructureMap().RunInMemory();
        }

        private ScenarioAssertionException fails(Action<Scenario> configuration)
        {
            return Exception<ScenarioAssertionException>.ShouldBeThrownBy(() => {
                host.Scenario(configuration);
            });
        }

        [Test]
        public void using_scenario_with_string_text_and_relative_url()
        {
            host.Scenario(x => { x.Get.Url("memory/hello"); })
                .Body.ReadAsText().ShouldEqual("hello from the in memory host");
            ;
        }

        [Test]
        public void using_scenario_with_controller_expression()
        {
            host.Scenario(x => { x.Get.Action<InMemoryEndpoint>(e => e.get_memory_hello()); })
                .Body.ReadAsText().ShouldEqual("hello from the in memory host");
            ;
        }

        [Test]
        public void using_scenario_with_input_model()
        {
            host.Scenario(x => { x.Get.Input(new InMemoryInput {Color = "Red"}); })
                .Body.ReadAsText().ShouldEqual("The color is Red");
            ;


            host.Scenario(x => { x.Get.Input(new InMemoryInput {Color = "Orange"}); })
                .Body.ReadAsText().ShouldEqual("The color is Orange");
            ;
        }


        [Test]
        public void using_scenario_with_input_model_as_marker()
        {
            host.Scenario(x => { x.Get.Input<MarkerInput>(); })
                .Body.ReadAsText().ShouldEqual("just the marker");
            ;
        }

        [Test]
        public void using_scenario_with_ContentShouldContain_declaration_happy_path()
        {
            host.Scenario(x => {
                x.Get.Input<MarkerInput>();
                x.ContentShouldContain("just the marker");
            });
        }


        [Test]
        public void using_scenario_with_ContentShouldContain_declaration_sad_path()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() => {
                host.Scenario(x => {
                    x.Get.Input<MarkerInput>();
                    x.ContentShouldContain("wrong text");
                });
            });

            ex.Message.ShouldContain("The response body does not contain expected text \"wrong text\"");
        }

        [Test]
        public void using_scenario_with_ContentShouldNotContain_declaration_happy_path()
        {
            host.Scenario(x => {
                x.Get.Input<MarkerInput>();
                x.ContentShouldNotContain("some random stuff");
            });
        }

        [Test]
        public void using_scenario_with_ContentShouldNotContain_declaration_sad_path()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() => {
                host.Scenario(x => {
                    x.Get.Input<MarkerInput>();
                    x.ContentShouldNotContain("just the marker");
                });
            });

            ex.Message.ShouldContain("The response body should not contain text \"just the marker\"");
        }

        [Test]
        public void using_scenario_with_StatusCodeShouldBe_happy_path()
        {
            host.Scenario(x => {
                x.Get.Input<MarkerInput>();
                x.StatusCodeShouldBe(HttpStatusCode.OK);
            });
        }

        [Test]
        public void using_scenario_with_StatusCodeShouldBe_sad_path()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() => {
                host.Scenario(x => {
                    x.Get.Input<MarkerInput>();
                    x.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
                });
            });

            ex.Message.ShouldContain("Expected status code 500 (InternalServerError), but was 200");
        }

        [Test]
        public void single_header_value_is_positive()
        {
            host.Scenario(x => {
                x.JsonData(new HeaderInput {Key = "Foo", Value1 = "Bar"});
                x.Header("Foo").ShouldHaveOneNonNullValue()
                    .SingleValueShouldEqual("Bar");
            });
        }

        [Test]
        public void single_header_value_is_negative_with_the_wrong_value()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() => {
                host.Scenario(x => {
                    x.JsonData(new HeaderInput {Key = "Foo", Value1 = "NotBar"});
                    x.Header("Foo").ShouldHaveOneNonNullValue()
                        .SingleValueShouldEqual("Bar");
                });
            });

            ex.Message.ShouldContain("Expected a single header value of 'Foo'='Bar', but the actual value was 'NotBar'");
        }

        [Test]
        public void single_header_value_is_negative_with_the_too_many_values()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() => {
                host.Scenario(x => {
                    x.JsonData(new HeaderInput {Key = "Foo", Value1 = "NotBar", Value2 = "AnotherBar"});
                    x.Header("Foo").ShouldHaveOneNonNullValue()
                        .SingleValueShouldEqual("Bar");
                });
            });

            ex.Message.ShouldContain(
                "Expected a single header value of 'Foo'='Bar', but the actual values were 'NotBar', 'AnotherBar'");
        }

        [Test]
        public void single_header_value_is_negative_because_there_are_no_values()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() => {
                host.Scenario(x => {
                    x.JsonData(new HeaderInput {Key = "Foo"});
                    x.Header("Foo")
                        .SingleValueShouldEqual("Bar");
                });
            });

            ex.Message.ShouldContain(
                "Expected a single header value of 'Foo'='Bar', but no values were found on the response");
        }

        [Test]
        public void should_have_on_non_null_header_value_happy_path()
        {
            host.Scenario(x => {
                x.JsonData(new HeaderInput {Key = "Foo", Value1 = "Anything"});
                x.Header("Foo").ShouldHaveOneNonNullValue();
            });
        }

        [Test]
        public void should_have_one_non_null_value_sad_path()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() => {
                host.Scenario(x => {
                    x.JsonData(new HeaderInput {Key = "Foo"});
                    x.Header("Foo").ShouldHaveOneNonNullValue();
                });
            });

            ex.Message.ShouldContain("Expected a single header value of 'Foo', but no values were found on the response");
        }

        [Test]
        public void should_have_on_non_null_value_sad_path_with_too_many_values()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() => {
                host.Scenario(x => {
                    x.JsonData(new HeaderInput {Key = "Foo", Value1 = "Bar1", Value2 = "Bar2"});
                    x.Header("Foo").ShouldHaveOneNonNullValue();
                });
            });

            ex.Message.ShouldContain(
                "Expected a single header value of 'Foo', but found multiple values on the response: 'Bar1', 'Bar2'");
        }

        [Test]
        public void header_should_not_be_written_happy_path()
        {
            host.Scenario(x => {
                x.JsonData(new HeaderInput {Key = "Foo"});
                x.Header("Foo").ShouldNotBeWritten();
            });
        }

        [Test]
        public void header_should_not_be_written_sad_path_with_values()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() => {
                host.Scenario(x => {
                    x.JsonData(new HeaderInput {Key = "Foo", Value1 = "Bar1", Value2 = "Bar2"});
                    x.Header("Foo").ShouldNotBeWritten();
                });
            });

            ex.Message.ShouldContain("Expected no value for header 'Foo', but found values 'Bar1', 'Bar2'");
        }

        [Test]
        public void exact_content_happy_path()
        {
            host.Scenario(x => {
                x.Get.Action<InMemoryEndpoint>(_ => _.get_memory_hello());

                x.ContentShouldBe("hello from the in memory host");
            });
        }

        [Test]
        public void exact_content_sad_path()
        {
            var e = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() => {
                host.Scenario(x =>
                {
                    x.Get.Action<InMemoryEndpoint>(_ => _.get_memory_hello());

                    x.ContentShouldBe("the wrong content");
                });
            });

            e.Message.ShouldContain("Expected the content to be 'the wrong content'");
        }

        [Test]
        public void content_type_should_be_happy_path()
        {
            host.Scenario(_ => {
                _.Get.Action<InMemoryEndpoint>(x => x.get_memory_hello());

                _.ContentTypeShouldBe(MimeType.Text);
            });
        }

        [Test]
        public void content_type_sad_path()
        {
            var ex = fails(_ => {
                _.Get.Action<InMemoryEndpoint>(x => x.get_memory_hello());

                _.ContentTypeShouldBe("text/json");
            });

            ex.Message.ShouldContain("Expected a single header value of 'Content-Type'='text/json', but the actual value was 'text/plain'");
        }

        [Test]
        public void happily_blows_up_on_an_unexpected_500()
        {
            var ex = fails(_ => {
                _.Get.Action<InMemoryEndpoint>(x => x.get_wrong_status_code());

            });

            ex.Message.ShouldContain("Expected status code 200 (Ok), but was 500");
            ex.Message.ShouldContain("the error text");
        }
    }

    public class InMemoryEndpoint
    {
        private readonly IOutputWriter _writer;

        public InMemoryEndpoint(IOutputWriter writer)
        {
            _writer = writer;
        }

        public string get_wrong_status_code()
        {
            _writer.WriteResponseCode(HttpStatusCode.InternalServerError);

            return "the error text";
        }

        public string post_header_values(HeaderInput input)
        {
            if (input.Value1.IsNotEmpty())
            {
                _writer.AppendHeader(input.Key, input.Value1);
            }

            if (input.Value2.IsNotEmpty())
            {
                _writer.AppendHeader(input.Key, input.Value2);
            }

            return "it's all good";
        }

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

    public class HeaderInput
    {
        public string Key { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
    }

    public class InMemoryInput
    {
        public string Color { get; set; }
    }

    public class MarkerInput
    {
    }
}