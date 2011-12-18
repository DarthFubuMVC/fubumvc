using System;
using FubuCore;
using FubuMVC.Core.Resources.Media;
using FubuMVC.Core.Resources.Media.Projections;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuMVC.Tests.Resources.Projections
{
    [TestFixture]
    public class DelegatingProjectionTester
    {
        [Test]
        public void creates_and_delegates_to_another_projection()
        {
            var context = MockRepository.GenerateMock<IProjectionContext<ProjectionModel>>();
            var stub = new FakeProjector();

            context.Stub(x => x.Service<FakeProjector>()).Return(stub);

            var projection = new DelegatingProjection<ProjectionModel, FakeProjector>();
            var theNode = new DictionaryMediaNode();

            projection.Write(context, theNode);

            stub.theTarget.ShouldBeTheSameAs(context);
            stub.theNode.ShouldBeTheSameAs(theNode);
        }

        [Test]
        public void include_inside_a_projection()
        {
            var context = MockRepository.GenerateMock<IProjectionContext<ProjectionModel>>();
            var stub = new FakeProjector();

            context.Stub(x => x.Service<FakeProjector>()).Return(stub);

            var projection = new Projection<ProjectionModel>(DisplayFormatting.RawValues);
            projection.Include<FakeProjector>();

            var theNode = new DictionaryMediaNode();

            projection.As<IProjection<ProjectionModel>>().Write(context, theNode);

            stub.theTarget.ShouldBeTheSameAs(context);
            stub.theNode.ShouldBeTheSameAs(theNode);
        }

        public class ProjectionModel
        {
            
        }

        public class FakeProjector : IProjection<ProjectionModel>
        {
            public IProjectionContext<ProjectionModel> theTarget;
            public IMediaNode theNode;

            public void Write(IProjectionContext<ProjectionModel> context, IMediaNode node)
            {
                theTarget = context;
                theNode = node;
            }
        }
    }
}