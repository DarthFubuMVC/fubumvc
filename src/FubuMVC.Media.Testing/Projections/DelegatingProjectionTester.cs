using FubuCore;
using FubuMVC.Media.Projections;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuMVC.Media.Testing.Projections
{
    [TestFixture]
    public class DelegatingProjectionTester
    {
        [Test]
        public void creates_and_delegates_to_another_projection()
        {
            var context = MockRepository.GenerateMock<IProjectionContext<ProjectionModel>>();

            var projection = new DelegatingProjection<ProjectionModel, FakeProjector>();
            var theNode = new DictionaryMediaNode();

            projection.Write(context, theNode);

            FakeProjector.theTarget.ShouldBeTheSameAs(context);
            FakeProjector.theNode.ShouldBeTheSameAs(theNode);
        }

        [Test]
        public void include_inside_a_projection()
        {
            var context = MockRepository.GenerateMock<IProjectionContext<ProjectionModel>>();

            var projection = new Projection<ProjectionModel>(DisplayFormatting.RawValues);
            projection.Include<FakeProjector>();

            var theNode = new DictionaryMediaNode();

            projection.As<IProjection<ProjectionModel>>().Write(context, theNode);

            FakeProjector.theTarget.ShouldBeTheSameAs(context);
            FakeProjector.theNode.ShouldBeTheSameAs(theNode);
        }

        public class ProjectionModel
        {
            
        }

        public class FakeProjector : IProjection<ProjectionModel>
        {
            public static IProjectionContext<ProjectionModel> theTarget;
            public static IMediaNode theNode;

            public void Write(IProjectionContext<ProjectionModel> context, IMediaNode node)
            {
                theTarget = context;
                theNode = node;
            }
        }
    }
}