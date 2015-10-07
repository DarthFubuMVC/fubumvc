using System;
using System.IO;
using System.Net;
using System.Web;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServerSentEvents;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using Cookie = FubuMVC.Core.Http.Cookies.Cookie;

namespace FubuMVC.Tests.ServerSentEvents
{
    [TestFixture]
    public class ServerEventWriterTester : InteractionContext<ServerEventWriter>
    {
        [Test]
        public void should_write_the_content_type_on_the_first_call_to_write_data()
        {
            ClassUnderTest.WriteData("something");

            MockFor<IOutputWriter>().AssertWasCalled(x => x.ContentType(MimeType.EventStream));
        }

        [Test]
        public void should_flush_after_each_write()
        {
            ClassUnderTest.WriteData("something");

            MockFor<IOutputWriter>().AssertWasCalled(x => x.Flush());

            ClassUnderTest.WriteData("else");

            MockFor<IOutputWriter>().AssertWasCalled(x => x.Flush(), x => x.Repeat.Twice());

        }

        [Test]
        public void only_writes_the_mimetype_once()
        {
            ClassUnderTest.WriteData("something");
            ClassUnderTest.WriteData("something");
            ClassUnderTest.WriteData("something");
            ClassUnderTest.WriteData("something");
            ClassUnderTest.WriteData("something");
            ClassUnderTest.WriteData("something");

            MockFor<IOutputWriter>().AssertWasCalled(x => x.ContentType(MimeType.EventStream), x => x.Repeat.Once());
        }

        [Test]
        public void returns_true_on_successful_write()
        {
            ClassUnderTest.WriteData("something").ShouldBeTrue();
        }

        [Test]
        public void returns_false_when_an_HttpException_occurs_on_flush()
        {
            MockFor<IOutputWriter>().Stub(x => x.Flush()).Throw(new HttpException());
            ClassUnderTest.WriteData("something").ShouldBeFalse();
        }

		[Test]
		public void returns_false_when_an_AccessViolationException_occurs_on_flush()
		{
			MockFor<IOutputWriter>().Stub(x => x.Flush()).Throw(new AccessViolationException());
			ClassUnderTest.WriteData("something").ShouldBeFalse();
		}
    }

    [TestFixture]
    public class ServerEventWriter_output_Tester
    {
        private RecordingOutputWriter output;
        private ServerEventWriter writer;

        [SetUp]
        public void SetUp()
        {
            output = new RecordingOutputWriter();
            writer = new ServerEventWriter(output);
        }

        [Test]
        public void write_only_data_and_id()
        {
            writer.Write(new ServerEvent("the id", "the data"));
            output.Text.ShouldBe("id: the id\ndata: the data\n\n");
        }

        [Test]
        public void write_with_id_data_and_event()
        {
            writer.Write(new ServerEvent("the id", "the data"){Event = "something"});
            output.Text.ShouldBe("id: the id/something\ndata: the data\n\n");  
        }

        [Test]
        public void write_with_id_data_and_event_and_retry()
        {
            writer.Write(new ServerEvent("the id", "the data") { Event = "something", Retry = 1000});
            output.Text.ShouldBe("id: the id/something\nretry: 1000\ndata: the data\n\n");
        }
    }

    public class RecordingOutputWriter : IOutputWriter
    {
        private readonly StringWriter _writer = new StringWriter();

        public string Text
        {
            get
            {
                return _writer.ToString();
            }
        }

        public void Write(string renderedOutput)
        {
            _writer.Write(renderedOutput);
        }

        public void WriteFile(string contentType, string localFilePath, string displayName)
        {
            throw new NotImplementedException();
        }

        public void Write(string contentType, string renderedOutput)
        {
            throw new NotImplementedException();
        }

        public void RedirectToUrl(string url)
        {
            throw new NotImplementedException();
        }

        public void AppendCookie(Cookie cookie)
        {
            throw new NotImplementedException();
        }

        public void AppendCookie(HttpCookie cookie)
        {
            throw new NotImplementedException();
        }

        public void AppendHeader(string key, string value)
        {
        }

        public void Write(string contentType, Action<Stream> output)
        {
            throw new NotImplementedException();
        }

        public void WriteResponseCode(HttpStatusCode status, string description)
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
        }

        public void Dispose()
        {
        }
    }
}