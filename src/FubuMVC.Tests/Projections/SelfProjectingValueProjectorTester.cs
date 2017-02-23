using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Projections;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Projections
{
    
    public class SelfProjectingValueProjectorTester
    {
        [Fact]
        public void accessors()
        {
            var accessor = ReflectionHelper.GetAccessor<HoldsProjectsItself>(x => x.Itself);
            var projection = new SelfProjectingValueProjector<HoldsProjectsItself, ProjectsItself>(accessor);

            projection.As<IProjection<HoldsProjectsItself>>()
                .Accessors()
                .Single()
                .ShouldBe(accessor);
        }
    }

    public class ProjectsItself : IProjectMyself
    {
        public void Project(string attributeName, IMediaNode node)
        {
            throw new System.NotImplementedException();
        }
    }

    public class HoldsProjectsItself
    {
        public ProjectsItself Itself { get; set; }
    }
}