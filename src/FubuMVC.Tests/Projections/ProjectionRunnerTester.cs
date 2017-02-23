using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Projections;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Projections
{


    
    public class ProjectionRunnerTester : InteractionContext<ProjectionRunner>
    {
        private TargetProjection theProjection;
        private Target theTarget;
        private DictionaryMediaNode theNode;

        protected override void beforeEach()
        {
            theProjection = new TargetProjection();
            theTarget = new Target();

            MockFor<IServiceLocator>().Stub(x => x.GetInstance<TargetProjection>())
                .Return(theProjection);

            theNode = new DictionaryMediaNode();
        }


        [Fact]
        public void run_by_the_projection_and_values()
        {
            ClassUnderTest.Run(theProjection, new SimpleValues<Target>(theTarget), theNode);

            assertTheProjectionWasRun();
        }

        [Fact]
        public void run_by_the_projection_and_target()
        {
            ClassUnderTest.Run(theProjection, theTarget, theNode);

            assertTheProjectionWasRun();
        }

        [Fact]
        public void run_by_the_projection_type()
        {
            ClassUnderTest.Run<Target, TargetProjection>(new SimpleValues<Target>(theTarget), theNode);

            assertTheProjectionWasRun();
        }

        [Fact]
        public void run_by_the_projection_type_2()
        {
            ClassUnderTest.Run<Target, TargetProjection>(theTarget, theNode);

            assertTheProjectionWasRun();
        }

        private void assertTheProjectionWasRun()
        {
            theProjection.TheNode.ShouldBeTheSameAs(theNode);
            theProjection.TheTarget.ShouldBeTheSameAs(theTarget);
        }


        public class TargetProjection : IProjection<Target>
        {
            public Target TheTarget;
            public IMediaNode TheNode;

            public void Write(IProjectionContext<Target> context, IMediaNode node)
            {
                TheTarget = context.Subject;
                TheNode = node;
            }

            public IEnumerable<Accessor> Accessors()
            {
                throw new System.NotImplementedException();
            }
        }

        public class Target
        {
            
        }
    }

}