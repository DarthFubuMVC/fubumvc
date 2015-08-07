using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using FubuCore;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Runtime;
using Shouldly;

namespace FubuMVC.IntegrationTesting
{
    public static class HttpResponseExtensions
    {
        public static HttpResponse ShouldHaveHeader(this HttpResponse response, HttpResponseHeader header)
        {
            response.ResponseHeaderFor(header).ShouldNotBeEmpty();
            return response;
        }

        public static HttpResponse ContentShouldBe(this HttpResponse response, MimeType mimeType, string content)
        {
            response.ContentType.ShouldBe(mimeType.Value);
            response.ReadAsText().ShouldBe(content);

            return response;
        }

        public static HttpResponse ContentTypeShouldBe(this HttpResponse response, MimeType mimeType)
        {
            response.ContentType.ShouldBe(mimeType.Value);

            return response;
        }

        public static HttpResponse LengthShouldBe(this HttpResponse response, int length)
        {
            response.ContentLength().ShouldBe(length);

            return response;
        }

        public static HttpResponse ContentShouldBe(this HttpResponse response, string mimeType, string content)
        {
            response.ContentType.ShouldBe(mimeType);
            response.ReadAsText().ShouldBe(content);

            return response;
        }


        public static HttpResponse StatusCodeShouldBe(this HttpResponse response, HttpStatusCode code)
        {
            response.StatusCode.ShouldBe(code);

            return response;
        }

        public static string FileEscape(this string file)
        {
            return "\"{0}\"".ToFormat(file);
        }

        public static IEnumerable<string> ScriptNames(this HttpResponse response)
        {
            var document = response.ReadAsXml();
            var tags = document.DocumentElement.SelectNodes("//script");

            foreach (XmlElement tag in tags)
            {
                var name = tag.GetAttribute("src");
                yield return name.Substring(name.IndexOf('_'));
            }
        }


        public static HttpResponse EtagShouldBe(this HttpResponse response, string etag)
        {
            etag.Trim('"').ShouldBe(etag);
            return response;
        }

        public static DateTime? LastModified(this HttpResponse response)
        {
            var lastModifiedString = response.ResponseHeaderFor(HttpResponseHeader.LastModified);
            return lastModifiedString.IsEmpty() ? (DateTime?) null : DateTime.ParseExact(lastModifiedString, "r", null);
        }

        public static HttpResponse LastModifiedShouldBe(this HttpResponse response, DateTime expected)
        {
            var lastModified = response.LastModified();
            lastModified.HasValue.ShouldBeTrue();
            lastModified.ShouldBe(expected);

            return response;
        }
    }
}