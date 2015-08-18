using System;
using System.Net;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using Serenity.Fixtures;
using StoryTeller;
using StoryTeller.Grammars.Tables;

namespace FubuMVC.IntegrationTesting.Conneg
{
    public class ConnegFixture : SerenityFixture
    {
        public ConnegFixture()
        {
            Title = "Content Negotiation";
        }

        [ExposeAsTable("Conneg Behavior for an Endpoint with the Default Configuration")]
        public void DefaultConnegRules(
            [Default("NULL")] string QueryString, 
            [Header("Send As"), SelectionValues("Json", "Xml", "Form Post")] string format,
            [Default("*/*")] string Accepts,
            out string ContentType,
            [Default("200")] out int ResponseCode
            )
        {
            DisableAllSecurity();

            var input = new XmlJsonHtmlMessage
            {
                Id = Guid.NewGuid()
            };

            var response = Runtime.Scenario(_ =>
            {
                var url = "send/mixed";
                if (QueryString.IsNotEmpty())
                {
                    url = url.AppendUrl(QueryString);
                }

                _.Security.Disable();
                _.IgnoreStatusCode();
                _.Post.Url(url);

                _.Request.Accepts(Accepts);

                switch (format)
                {
                    case "Json":
                        _.Request.Body.JsonInputIs(input);
                        _.Request.ContentType("application/json");
                        break;

                    case "Xml":
                        _.Request.Body.XmlInputIs(input);
                        _.Request.ContentType("application/xml");
                        break;

                    case "Form Post":
                        _.Request.Body.WriteFormData(input);
                        _.Request.ContentType(MimeType.HttpFormMimetype);
                        break;

                    default:
                        throw new ArgumentException("format");
                }
            });


            StoryTellerAssert.Fail(response.StatusCode == 500, () => response.Body.ReadAsText());

            ContentType = response.ContentType();
            ResponseCode = response.StatusCode;
        }
    }
}