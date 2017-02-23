﻿using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Json;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http
{
    
    public class ConnegSettingsTester
    {
        private const string theOriginalMimetype = "random/custom";

        [Fact]
        public void no_correction_with_no_querystring()
        {
            var request = OwinHttpRequest.ForTesting();
            var mimeType = new CurrentMimeType("text/json", theOriginalMimetype);

            new ConnegSettings().InterpretQuerystring(mimeType, request);

            mimeType.AcceptTypes.Single().ShouldBe(theOriginalMimetype);
        }

        [Fact]
        public void no_correction_with_wrong_querystring()
        {
            var request = OwinHttpRequest.ForTesting();
            request.QueryString["Key"] = "Json";

            var mimeType = new CurrentMimeType("text/json", theOriginalMimetype);

            new ConnegSettings().InterpretQuerystring(mimeType, request);

            mimeType.AcceptTypes.Single().ShouldBe(theOriginalMimetype);
        }

        [Fact]
        public void correct_to_json()
        {
            var request = OwinHttpRequest.ForTesting();
            request.QueryString["Format"] = "Json";

            var mimeType = new CurrentMimeType("text/json", theOriginalMimetype);

            new ConnegSettings().InterpretQuerystring(mimeType, request);

            mimeType.AcceptTypes.Single().ShouldBe(MimeType.Json.Value);
        }

        [Fact]
        public void correct_to_xml()
        {
            var request = OwinHttpRequest.ForTesting();
            request.QueryString["Format"] = "XML";

            var mimeType = new CurrentMimeType("text/json", theOriginalMimetype);

            new ConnegSettings().InterpretQuerystring(mimeType, request);

            mimeType.AcceptTypes.Single().ShouldBe(MimeType.Xml.Value);
        }

        [Fact]
        public void use_a_custom_querystring_parameter()
        {
            var request = OwinHttpRequest.ForTesting();
            request.QueryString["Format"] = "Text";

            var settings = new ConnegSettings();
            settings.QuerystringParameters.Add(new ConnegQuerystring("Format", "Text", MimeType.Text));

            var mimeType = new CurrentMimeType("text/json", theOriginalMimetype);

            settings.InterpretQuerystring(mimeType, request);

            mimeType.AcceptTypes.Single().ShouldBe(MimeType.Text.Value);
        }

        [Fact]
        public void the_default_formatters_are_json_then_xml()
        {
            new ConnegSettings().Formatters.Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(NewtonsoftJsonFormatter), typeof(XmlFormatter));
        }

        [Fact]
        public void find_formatter_by_mimetype()
        {
            new ConnegSettings().FormatterFor(MimeType.Json)
                .ShouldBeOfType<NewtonsoftJsonFormatter>();

            new ConnegSettings().FormatterFor(MimeType.Xml)
                .ShouldBeOfType<XmlFormatter>();
        }

        [Fact]
        public void add_formatter_places_it_first()
        {
            var settings = new ConnegSettings();
            settings.AddFormatter(new AjaxAwareJsonSerializer());

            settings.FormatterFor(MimeType.Json)
                .ShouldBeOfType<AjaxAwareJsonSerializer>();
        }
    }
}