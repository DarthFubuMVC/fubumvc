using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Projections;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Projections
{
    [TestFixture]
    public class ExternallyFormattedValueProjectorTester
    {
        [Test]
        public void accessors()
        {
            var accessor = ReflectionHelper.GetAccessor<ExternalFormatTarget>(x => x.Name);
            var formatter = new ExternallyFormattedValueProjector<ExternalFormatTarget, string>(accessor, null);
            formatter.As<IProjection<ExternalFormatTarget>>().Accessors().Single().ShouldEqual(accessor);
        }
    }

    public class ExternalFormatTarget
    {
        public string Name { get; set; }
    }
}