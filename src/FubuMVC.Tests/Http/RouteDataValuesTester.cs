using System.Web.Routing;
using FubuCore.Binding.Values;
using FubuMVC.Core.Http;
using Xunit;
using Shouldly;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http
{
    
    public class RouteDataValuesTester
    {
        private RouteDataValues theValues;

        public RouteDataValuesTester()
        {
            var data = new RouteData();
            data.Values.Add("a", 1);
            data.Values.Add("b", 2);

            theValues = new RouteDataValues(data);
        }

        [Fact]
        public void got_the_provenance()
        {
            theValues.Provenance.ShouldBe(RequestDataSource.Route.ToString());
        }

        [Fact]
        public void get_value()
        {
            theValues.Get("a").ShouldBe(1);
        }

        [Fact]
        public void has()
        {
            theValues.Has("a").ShouldBeTrue();
            theValues.Has("b").ShouldBeTrue();
            theValues.Has("c").ShouldBeFalse();
        }

        [Fact]
        public void write_report()
        {
            var report = MockRepository.GenerateMock<IValueReport>();
            theValues.WriteReport(report);

            report.AssertWasCalled(x => x.Value("a", 1));
            report.AssertWasCalled(x => x.Value("b", 2));
        }
    }
}