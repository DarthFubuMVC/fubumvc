using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Projections;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Projections
{
    
    public class ExternallyFormattedValueProjectorTester
    {
        [Fact]
        public void accessors()
        {
            var accessor = ReflectionHelper.GetAccessor<ExternalFormatTarget>(x => x.Name);
            var formatter = new ExternallyFormattedValueProjector<ExternalFormatTarget, string>(accessor, null);
            formatter.As<IProjection<ExternalFormatTarget>>().Accessors().Single().ShouldBe(accessor);
        }
    }

    public class ExternalFormatTarget
    {
        public string Name { get; set; }
    }
}