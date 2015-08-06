using System;
using System.IO;
using System.Linq;
using System.Net;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security.Authentication.Cookies;
using FubuMVC.Core.Security.Authentication.Tickets;
using Shouldly;
using NUnit.Framework;
using Cookie = FubuMVC.Core.Http.Cookies.Cookie;

namespace FubuMVC.IntegrationTesting.Security.Authentication
{
    [TestFixture]
    public class authenticated_request_against_an_authenticated_route : AuthenticationHarness
    {


        [Test]
        public void login_with_default_credentials_and_retrieve_a_resource()
        {
            // create the auth ticket
            var now = DateTime.Now;
            var ticket = new AuthenticationTicket
            {
                Expiration = now.AddDays(1),
                LastAccessed = now,
                UserName = "fubu"
            };

            var writer = new CookieWriter();
            Container
                .With(typeof (IOutputWriter), writer)
                .GetInstance<CookieTicketSource>()
                .Persist(ticket);

            var cookie = writer.Cookie;

            Scenario(_ =>
            {
                _.Get.Input<TargetModel>();
                _.Request.Accepts("text/json").ContentType(MimeType.HttpFormMimetype);

                _.Request.AppendCookie(cookie);

                _.StatusCodeShouldBeOk();
            });
/*
            var response = endpoints.GetByInput(new TargetModel(), acceptType: "text/json", configure: r => {
                var cookies = new CookieContainer();
                cookies.Add(new System.Net.Cookie
                {
                    Domain = "localhost",
                    Path = cookie.Path,
                    Expires = now.AddDays(1),
                    Name = cookie.States.Single().Name,
                    Value = cookie.Value
                });

                r.CookieContainer = cookies;
                r.AllowAutoRedirect = false;
            });

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            */
             }

        #region Nested Type: CookieWriter

        public class CookieWriter : IOutputWriter
        {
            public Cookie Cookie { get; set; }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public void WriteFile(string contentType, string localFilePath, string displayName)
            {
                throw new NotImplementedException();
            }

            public void Write(string contentType, string renderedOutput)
            {
                throw new NotImplementedException();
            }

            public void Write(string renderedOutput)
            {
                throw new NotImplementedException();
            }

            public void RedirectToUrl(string url)
            {
                throw new NotImplementedException();
            }

            public void AppendCookie(Cookie cookie)
            {
                Cookie = cookie;
            }

            public void AppendHeader(string key, string value)
            {
                throw new NotImplementedException();
            }

            public void Write(string contentType, Action<Stream> output)
            {
                throw new NotImplementedException();
            }

            public void WriteResponseCode(HttpStatusCode status, string description = null)
            {
                throw new NotImplementedException();
            }

            public IRecordedOutput Record(Action action)
            {
                throw new NotImplementedException();
            }

            public void Replay(IRecordedOutput output)
            {
                throw new NotImplementedException();
            }

            public void Flush()
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}