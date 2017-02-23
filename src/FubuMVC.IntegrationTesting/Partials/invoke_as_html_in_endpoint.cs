﻿using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using HtmlTags;
using Xunit;

namespace FubuMVC.IntegrationTesting.Partials
{
    
    public class invoke_as_html_in_endpoint
    {
        [Fact]
        public void get_the_partial_as_html_if_requested()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Input(new InputModel2 {Name = "Jeremy"});

                _.Request.Accepts("text/json");

                // Heinous
                _.ContentShouldContain("div");
                _.ContentShouldContain("/div");
                _.ContentShouldContain("Jeremy");
            });
        }
    }

    // "{"Text":"{"Name":"Jeremy"}"}
    //  {"Text":"{\"Name\":\"Jeremy\"}"}

    public class InvokerEndpoint
    {
        private readonly IPartialInvoker _invoker;

        public InvokerEndpoint(IPartialInvoker invoker)
        {
            _invoker = invoker;
        }

        public OutputModel get_inner_json_Name(InputModel model)
        {
            return new OutputModel
            {
                Text = _invoker.InvokeObject(new PartialModel {Name = model.Name})
            };
        }

        public OutputModel get_inner_html_Name(InputModel2 model)
        {
            return new OutputModel
            {
                Text = _invoker.InvokeAsHtml(new PartialModel {Name = model.Name})
            };
        }

        public PartialModel partial(PartialModel model)
        {
            return model;
        }
    }

    public class OutputModel
    {
        public string Text { get; set; }
    }

    public class InputModel
    {
        public string Name { get; set; }
    }

    public class InputModel2 : InputModel
    {
    }

    public class PartialModel
    {
        public string Name { get; set; }
    }

    public class PartialModelWriter : IMediaWriter<PartialModel>
    {
        public void Write(string mimeType, IFubuRequestContext context, PartialModel resource)
        {
            var html = new DivTag().Text(resource.Name).ToString();
            context.Writer.Write(MimeType.Html.Value, html);
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return MimeType.Html.Value; }
        }
    }
}