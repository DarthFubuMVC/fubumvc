using FubuCore.Reflection;
using FubuMVC.Core.Registration.Routes;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class RouteInputTester
    {
        private RouteInput _input;
        public class FakeInput {public string Code { get; set; }}
        
        [SetUp]
        public void SetUp()
        {
            _input = new RouteInput(ReflectionHelper.GetAccessor<FakeInput>(x => x.Code));
        }

        [Test]
        public void to_query_string_of_empty_value_returns_name()
        {
            _input.ToQueryString(new FakeInput()).ShouldEqual(_input.Name + "=");
        }
    }
}