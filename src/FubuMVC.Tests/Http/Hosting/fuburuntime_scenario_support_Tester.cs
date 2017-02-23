﻿using System;
using System.Net;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http.Scenarios;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.ServiceBus.Docs.Basics;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http.Hosting
{
    
    public class fuburuntime_scenario_support_Tester : IDisposable
    {
        private FubuRuntime host = FubuRuntime.Basic();

        public void Dispose()
        {
            host.SafeDispose();
        }


        [Fact]
        public void invoke_a_simple_string_endpoint()
        {
            host.Scenario(_ =>
            {
                _.Get.Url("memory/hello");
                _.ContentShouldBe("hello from the in memory host");
            });
        }

        private ScenarioAssertionException fails(Action<Scenario> configuration)
        {
            return Exception<ScenarioAssertionException>.ShouldBeThrownBy(() => { host.Scenario(configuration); });
        }

        public void examples()
        {
host.Scenario(_ =>
{
    // Specify a GET request to the Url that runs an endpoint method:
    _.Get.Action<InMemoryEndpoint>(e => e.get_memory_hello());

    // Or specify a POST to the Url that would handle an input message:
    _.Post

        // This call serializes the input object to Json using the 
        // application's configured JSON serializer and setting
        // the contents on the Request body
        .Json(new HeaderInput {Key = "Foo", Value1 = "Bar"});

    // Or specify a GET by an input object to get the route parameters
    _.Get.Input(new InMemoryInput { Color = "Red" });
});



host.Scenario(_ =>
{
    // set up a request here

    // Read the response body as text
    var bodyText = _.Response.Body.ReadAsText();

    // Read the response body by deserializing Json
    // into a .net type with the application's
    // configured Json serializer
    var output = _.Response.Body.ReadAsJson<MyResponse>();

    // If you absolutely have to work with Xml...
    var xml = _.Response.Body.ReadAsXml();
});
        }

        [Fact]
        public void using_scenario_with_string_text_and_relative_url()
        {
            host.Scenario(x => { x.Get.Url("memory/hello"); })
                .Body.ReadAsText().ShouldBe("hello from the in memory host");
            ;
        }

        [Fact]
        public void using_scenario_with_controller_expression()
        {
            host.Scenario(x =>
            {
                x.Get.Action<InMemoryEndpoint>(e => e.get_memory_hello());
            })
                .Body.ReadAsText().ShouldBe("hello from the in memory host");
            ;
        }

        [Fact]
        public void using_scenario_with_input_model()
        {
            host.Scenario(x => { x.Get.Input(new InMemoryInput {Color = "Red"}); })
                .Body.ReadAsText().ShouldBe("The color is Red");
            ;


            host.Scenario(x => { x.Get.Input(new InMemoryInput {Color = "Orange"}); })
                .Body.ReadAsText().ShouldBe("The color is Orange");
            ;
        }


        [Fact]
        public void using_scenario_with_input_model_as_marker()
        {
            host.Scenario(x => { x.Get.Input<MarkerInput>(); })
                .Body.ReadAsText().ShouldBe("just the marker");
            ;
        }

        [Fact]
        public void using_scenario_with_ContentShouldContain_declaration_happy_path()
        {
            host.Scenario(x =>
            {
                x.Get.Input<MarkerInput>();
                x.ContentShouldContain("just the marker");
            });
        }


        [Fact]
        public void using_scenario_with_ContentShouldContain_declaration_sad_path()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() =>
            {
                host.Scenario(x =>
                {
                    x.Get.Input<MarkerInput>();
                    x.ContentShouldContain("wrong text");
                });
            });

            ex.Message.ShouldContain("The response body does not contain expected text \"wrong text\"");
        }

        [Fact]
        public void using_scenario_with_ContentShouldNotContain_declaration_happy_path()
        {
            host.Scenario(x =>
            {
                x.Get.Input<MarkerInput>();
                x.ContentShouldNotContain("some random stuff");
            });
        }

        [Fact]
        public void using_scenario_with_ContentShouldNotContain_declaration_sad_path()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() =>
            {
                host.Scenario(x =>
                {
                    x.Get.Input<MarkerInput>();
                    x.ContentShouldNotContain("just the marker");
                });
            });

            ex.Message.ShouldContain("The response body should not contain text \"just the marker\"");
        }

        [Fact]
        public void using_scenario_with_StatusCodeShouldBe_happy_path()
        {
            host.Scenario(x =>
            {
                x.Get.Input<MarkerInput>();
                x.StatusCodeShouldBe(HttpStatusCode.OK);
            });
        }

        [Fact]
        public void using_scenario_with_StatusCodeShouldBe_sad_path()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() =>
            {
                host.Scenario(x =>
                {
                    x.Get.Input<MarkerInput>();
                    x.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
                });
            });

            ex.Message.ShouldContain("Expected status code 500 (InternalServerError), but was 200");
        }

        [Fact]
        public void single_header_value_is_positive()
        {
            host.Scenario(x =>
            {
                x.Post.Json(new HeaderInput {Key = "Foo", Value1 = "Bar"}).Accepts("text/plain");

                x.Header("Foo").ShouldHaveOneNonNullValue()
                    .SingleValueShouldEqual("Bar");
            });
        }

        [Fact]
        public void single_header_value_is_negative_with_the_wrong_value()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() =>
            {
                host.Scenario(x =>
                {
                    x.Post.Json(new HeaderInput {Key = "Foo", Value1 = "NotBar"});
                    x.Header("Foo").ShouldHaveOneNonNullValue()
                        .SingleValueShouldEqual("Bar");
                });
            });

            ex.Message.ShouldContain("Expected a single header value of 'Foo'='Bar', but the actual value was 'NotBar'");
        }

        [Fact]
        public void single_header_value_is_negative_with_the_too_many_values()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() =>
            {
                host.Scenario(x =>
                {
                    x.Post.Json(new HeaderInput {Key = "Foo", Value1 = "NotBar", Value2 = "AnotherBar"});
                    x.Header("Foo").ShouldHaveOneNonNullValue()
                        .SingleValueShouldEqual("Bar");
                });
            });

            ex.Message.ShouldContain(
                "Expected a single header value of 'Foo'='Bar', but the actual values were 'NotBar', 'AnotherBar'");
        }

        [Fact]
        public void single_header_value_is_negative_because_there_are_no_values()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() =>
            {
                host.Scenario(x =>
                {
                    x.Post.Json(new HeaderInput {Key = "Foo"});
                    x.Header("Foo")
                        .SingleValueShouldEqual("Bar");
                });
            });

            ex.Message.ShouldContain(
                "Expected a single header value of 'Foo'='Bar', but no values were found on the response");
        }

        [Fact]
        public void should_have_on_non_null_header_value_happy_path()
        {
            host.Scenario(x =>
            {
                x.Post.Json(new HeaderInput {Key = "Foo", Value1 = "Anything"}).Accepts("text/plain");
                x.Header("Foo").ShouldHaveOneNonNullValue();
            });
        }

        [Fact]
        public void should_have_one_non_null_value_sad_path()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() =>
            {
                host.Scenario(x =>
                {
                    x.Post.Json(new HeaderInput {Key = "Foo"}).Accepts("text/plain");
                    x.Header("Foo").ShouldHaveOneNonNullValue();
                });
            });

            ex.Message.ShouldContain("Expected a single header value of 'Foo', but no values were found on the response");
        }

        [Fact]
        public void should_have_on_non_null_value_sad_path_with_too_many_values()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() =>
            {
                host.Scenario(x =>
                {
                    x.Post.Json(new HeaderInput {Key = "Foo", Value1 = "Bar1", Value2 = "Bar2"});
                    x.Header("Foo").ShouldHaveOneNonNullValue();
                });
            });

            ex.Message.ShouldContain(
                "Expected a single header value of 'Foo', but found multiple values on the response: 'Bar1', 'Bar2'");
        }

        [Fact]
        public void header_should_not_be_written_happy_path()
        {
            host.Scenario(x =>
            {
                x.Post.Json(new HeaderInput {Key = "Foo"}).Accepts("text/plain");
                x.Header("Foo").ShouldNotBeWritten();
            });
        }

        [Fact]
        public void header_should_not_be_written_sad_path_with_values()
        {
            var ex = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() =>
            {
                host.Scenario(x =>
                {
                    x.Post.Json(new HeaderInput {Key = "Foo", Value1 = "Bar1", Value2 = "Bar2"}).Accepts("plain/text");
                    x.Header("Foo").ShouldNotBeWritten();
                });
            });

            ex.Message.ShouldContain("Expected no value for header 'Foo', but found values 'Bar1', 'Bar2'");
        }

        [Fact]
        public void exact_content_happy_path()
        {
            host.Scenario(x =>
            {
                x.Get.Action<InMemoryEndpoint>(_ => _.get_memory_hello());

                x.ContentShouldBe("hello from the in memory host");
            });
        }

        [Fact]
        public void exact_content_sad_path()
        {
            var e = Exception<ScenarioAssertionException>.ShouldBeThrownBy(() =>
            {
                host.Scenario(x =>
                {
                    x.Get.Action<InMemoryEndpoint>(_ => _.get_memory_hello());

                    x.ContentShouldBe("the wrong content");
                });
            });

            e.Message.ShouldContain("Expected the content to be 'the wrong content'");
        }

        [Fact]
        public void content_type_should_be_happy_path()
        {
            host.Scenario(_ =>
            {
                _.Get.Action<InMemoryEndpoint>(x => x.get_memory_hello());

                _.ContentTypeShouldBe(MimeType.Text);
            });
        }

        [Fact]
        public void content_type_sad_path()
        {
            var ex = fails(_ =>
            {
                _.Get.Action<InMemoryEndpoint>(x => x.get_memory_hello());

                _.ContentTypeShouldBe("text/json");
            });

            ex.Message.ShouldContain(
                "Expected a single header value of 'Content-Type'='text/json', but the actual value was 'text/plain'");
        }

        [Fact]
        public void happily_blows_up_on_an_unexpected_500()
        {
            var ex = fails(_ => { _.Get.Action<InMemoryEndpoint>(x => x.get_wrong_status_code()); });

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