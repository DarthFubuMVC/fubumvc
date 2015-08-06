using FubuCore.Binding.Values;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Tests.Urls;
using NUnit.Framework;
using Shouldly;
using System.Linq;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class HeaderValueSourceTester
    {
        private OwinHttpRequest theRequest;
        private HeaderValueSource theSource;

        [SetUp]
        public void SetUp()
        {
            theRequest = new OwinHttpRequest();
            theSource = new HeaderValueSource(theRequest);
        }

        [Test]
        public void has()
        {
            theSource.Has("a").ShouldBeFalse();

            theRequest.AppendHeader("a", "1");

            theSource.Has("a").ShouldBeTrue();
        }

        [Test]
        public void get_with_only_one_value()
        {
            theRequest.AppendHeader("a", "1");
            theSource.Get("a").ShouldBe("1");
        }

        [Test]
        public void get_with_multiple_values()
        {
            theRequest.AppendHeader("a", "1", "2", "3");

            theSource.Get("a").ShouldBe(new string[] { "1", "2", "3" });
        }

        [Test]
        public void value_miss()
        {
            theSource.Value("a", v => {
                Assert.Fail("NOT SUPPOSED TO BE HERE!");
            }).ShouldBeFalse();
        }

        [Test]
        public void value_hit_with_only_one_value()
        {
            theRequest.AppendHeader("a", "1");
            theSource.Value("a", v => {
                v.RawKey.ShouldBe("a");
                v.RawValue.ShouldBe("1");
                v.Source.ShouldBe(RequestDataSource.Header.ToString());
            }).ShouldBeTrue();
        }


        [Test]
        public void value_hit_with_only_multiple_values()
        {
            theRequest.AppendHeader("a", "1", "2", "3");
            theSource.Value("a", v =>
            {
                v.RawKey.ShouldBe("a");
                v.RawValue.ShouldBe(new string[] { "1", "2", "3" });
                v.Source.ShouldBe(RequestDataSource.Header.ToString());
            }).ShouldBeTrue();
        }

        [Test]
        public void report_smoke_test()
        {
            theRequest.AppendHeader("a", "1", "2", "3");
            theRequest.AppendHeader("b", "4", "2", "3");
            theRequest.AppendHeader("c", "7", "2", "3");

            var report = MockRepository.GenerateMock<IValueReport>();
            theSource.WriteReport(report);

            report.AssertWasCalled(x => x.Value("a", new string[] { "1", "2", "3" }));
            report.AssertWasCalled(x => x.Value("b", new string[] { "4", "2", "3" }));
            report.AssertWasCalled(x => x.Value("c", new string[] { "7", "2", "3" }));
        }

        [Test]
        public void provenance()
        {
            // and yes, this *is* important
            theSource.Provenance.ShouldBe(RequestDataSource.Header.ToString());
        }
    }
}