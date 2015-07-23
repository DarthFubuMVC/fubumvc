using System;
using System.IO;
using System.Net;
using FubuCore.Dates;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Cookie = FubuMVC.Core.Http.Cookies.Cookie;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http.Cookies
{
    [TestFixture]
    public class CookieValueTester : InteractionContext<CookieValue>
    {
        private ISystemTime time;
        private ICookies cookies;
        private RecordingOutputWriter writer;
        private string theName;
        private CookieValue theCookieValue;

        protected override void beforeEach()
        {
            time = MockRepository.GenerateMock<ISystemTime>();
            cookies = MockFor<ICookies>();
            writer = new RecordingOutputWriter();

            theName = "some name";

            theCookieValue = new CookieValue(theName, time, cookies, writer);
        }

        [Test]
        public void erase()
        {
            var now = DateTime.Today.AddHours(8).ToUniversalTime();
            time.Stub(x => x.UtcNow()).Return(now);

            theCookieValue.Erase();

            writer.LastCookie.ShouldNotBeNull();
            writer.LastCookie.Value.ShouldBe("BLANK"); // has to do this to keep SelfHost from going wonky
            writer.LastCookie.Expires.ShouldBe(new DateTimeOffset(now.AddYears(-1)));
        }

        [Test]
        public void write_value()
        {
            var now = DateTime.Today.AddHours(8).ToUniversalTime();
            time.Stub(x => x.UtcNow()).Return(now);

            theCookieValue.Value = "some value";

            writer.LastCookie.ShouldNotBeNull();
            writer.LastCookie.Value.ShouldBe("some value");
            writer.LastCookie.Expires.ShouldBe(new DateTimeOffset(now.AddYears(1)));
        }
    }

    public class RecordingOutputWriter : IOutputWriter
    {
        public Cookie LastCookie;

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
            LastCookie = cookie;
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

    
}