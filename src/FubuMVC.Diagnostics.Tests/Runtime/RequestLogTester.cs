using System;
using System.Net;
using FubuCore.Logging;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Logging;
using FubuMVC.Diagnostics.Runtime;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class RequestLogTester
    {
        [Test]
        public void add_log()
        {
            var log = new RequestLog();

            var stringMessage = new StringMessage("something");
            log.AddLog(123.45, stringMessage);

            log.AddLog(234, "something");

            log.AllSteps()
                .ShouldHaveTheSameElementsAs(new RequestStep(123.45, stringMessage), new RequestStep(234, "something"));
        }

        [Test]
        public void failed_is_false_by_default()
        {
            // trivial code, but kind of important
            new RequestLog().Failed.ShouldBeFalse();
        }

        [Test]
        public void has_a_unique_id()
        {
            var log1 = new RequestLog();
            var log2 = new RequestLog();
            var log3 = new RequestLog();

            log1.Id.ShouldNotEqual(Guid.Empty);
            log2.Id.ShouldNotEqual(Guid.Empty);
            log3.Id.ShouldNotEqual(Guid.Empty);

            log1.Id.ShouldNotEqual(log2.Id);
            log1.Id.ShouldNotEqual(log3.Id);
            log2.Id.ShouldNotEqual(log3.Id);
        }

        [Test]
        public void status_is_assumed_to_be_200_if_not_failed_and_no_explicit_status_was_recorded()
        {
            var log = new RequestLog();
            log.Failed.ShouldBeFalse();

            log.HttpStatus.Status.ShouldEqual(HttpStatusCode.OK);
            log.HttpStatus.Description.ShouldEqual("OK");
        }


        [Test]
        public void status_is_assumed_to_be_500_if_failed_and_no_other_explicit_status_was_recorded()
        {
            var log = new RequestLog();

            log.Failed = true;

            log.HttpStatus.Status.ShouldEqual(HttpStatusCode.InternalServerError);
            log.HttpStatus.Description.ShouldEqual("Internal Server Error");
        }

        [Test]
        public void use_the_last_status_written()
        {
            var log = new RequestLog();
            log.AddLog(12, new HttpStatusReport{Status = HttpStatusCode.Unauthorized});
            log.AddLog(15, new HttpStatusReport{Status = HttpStatusCode.NotAcceptable});

            log.HttpStatus.Status.ShouldEqual(HttpStatusCode.NotAcceptable);
            log.HttpStatus.Description.ShouldEqual("Not Acceptable");
        }

        [Test]
        public void use_the_last_status_written_with_custom_description()
        {
            var log = new RequestLog();
            log.AddLog(12, new HttpStatusReport { Status = HttpStatusCode.Unauthorized });
            log.AddLog(15, new HttpStatusReport { Status = HttpStatusCode.NotAcceptable, Description = "I didn't like this"});

            log.HttpStatus.Status.ShouldEqual(HttpStatusCode.NotAcceptable);
            log.HttpStatus.Description.ShouldEqual("I didn't like this");
        }

        [Test]
        public void find_step_positive()
        {
            var log = new RequestLog();
            log.AddLog(10, new Fake{Name = "Jeremy"});
            log.AddLog(12, new Fake{Name = "Lindsey"});
            log.AddLog(13, new Fake{Name = "Max"});
            log.AddLog(15, new Fake{Name = "Shiner"});

            var step = log.FindStep<Fake>(x => x.Name == "Shiner");
            step.Log.Name.ShouldEqual("Shiner");
            step.RequestTimeInMilliseconds.ShouldEqual(15);
        }

        [Test]
        public void find_step_negative()
        {
            var log = new RequestLog();
            log.AddLog(10, new Fake { Name = "Jeremy" });
            log.AddLog(12, new Fake { Name = "Lindsey" });
            log.AddLog(13, new Fake { Name = "Max" });
            log.AddLog(15, new Fake { Name = "Shiner" });

            log.FindStep<Fake>(x => x.Name == "Josh").ShouldBeNull();
        }

        [Test]
        public void content_type_with_out_any_headers()
        {
            var log = new RequestLog();
            log.ContentType.ShouldEqual("Unknown");
        }

        [Test]
        public void content_type_with_no_content_type_header()
        {
            var log = new RequestLog();
            log.ResponseHeaders = new Header[0];

            log.ContentType.ShouldEqual("Unknown");
        }

        [Test]
        public void content_type_with_a_content_type_header()
        {
            var log = new RequestLog();
            log.ResponseHeaders = new Header[]{new Header(HttpResponseHeader.ContentType, MimeType.Javascript.Value)};

            log.ContentType.ShouldEqual(MimeType.Javascript.Value);
        }
    }

    public class Fake
    {
        public string Name { get; set; }
    }
}