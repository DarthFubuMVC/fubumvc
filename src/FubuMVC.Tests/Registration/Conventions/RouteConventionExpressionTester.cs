using FubuMVC.Core;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class RouteConventionExpressionTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        // TODO -- lots more testing here.
        [Test]
        public void ignore_controller_suffixes()
        {
            var registry = new FubuRegistry(x =>
            {
                x.Actions.IncludeType<SomethingRouter>();
                //x.Routes.IgnoreMethodSuffix()
            });


        }
    }


    public class SomethingRouter
    {
        public void Go() { }
    }
}