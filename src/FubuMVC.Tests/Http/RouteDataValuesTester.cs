using System.Web.Routing;
using FubuCore.Binding.Values;
using FubuMVC.Core.Http;
using NUnit.Framework;
using Shouldly;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class RouteDataValuesTester
    {
        private RouteDataValues theValues;

        [SetUp]
        public void SetUp()
        {
            var data = new RouteData();
            data.Values.Add("a", 1);
            data.Values.Add("b", 2);

            theValues = new RouteDataValues(data);
        }

        [Test]
        public void got_the_provenance()
        {
            theValues.Provenance.ShouldBe(RequestDataSource.Route.ToString());
        }

        [Test]
        public void get_value()
        {
            theValues.Get("a").ShouldBe(1);
        }

        [Test]
        public void has()
        {
            theValues.Has("a").ShouldBeTrue();
            theValues.Has("b").ShouldBeTrue();
            theValues.Has("c").ShouldBeFalse();
        }

        [Test]
        public void write_report()
        {
            var report = MockRepository.GenerateMock<IValueReport>();
            theValues.WriteReport(report);

            report.AssertWasCalled(x => x.Value("a", 1));
            report.AssertWasCalled(x => x.Value("b", 2));
        }
    }
}