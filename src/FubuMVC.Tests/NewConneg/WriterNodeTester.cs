using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class WriterNodeTester
    {
        private FakeWriterNode theNode;
        private Lazy<ObjectDef> _def;

        [SetUp]
        public void SetUp()
        {
            theNode = new FakeWriterNode();
            _def = new Lazy<ObjectDef>(() => theNode.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None));
        }

        private ObjectDef theResultingObjectDef
        {
            get
            {
                return _def.Value;
            }
        }

        [Test]
        public void puts_the_object_def_for_the_writer_from_the_sub_class_as_a_dependency()
        {
            theResultingObjectDef.FindDependencyDefinitionFor<IMediaWriter<TheResource>>()
                .ShouldBeTheSameAs(theNode.TheWriterDef);
        }

        [Test]
        public void the_concrete_type_is_media()
        {
            theResultingObjectDef.Type.ShouldEqual(typeof (Media<TheResource>));
        }

        [Test]
        public void by_default_the_condition_is_always_true()
        {
            theResultingObjectDef.FindDependencyDefinitionFor<IConditional>()
                .Value
                .ShouldBeTheSameAs(Always.Flyweight);
        }

        [Test]
        public void does_not_have_a_condition_by_default()
        {
            theNode.HasCondition().ShouldBeFalse();
        }

        [Test]
        public void replace_the_conditional_with_a_type()
        {
            theNode.Condition<FakeConditional>();

            theNode.HasCondition().ShouldBeTrue();

            theResultingObjectDef.FindDependencyDefinitionFor<IConditional>()
                .Type.ShouldEqual(typeof (FakeConditional));
        }

        [Test]
        public void replace_the_conditional_with_a_lambda()
        {
            theNode.Condition(() => false);

            theNode.HasCondition().ShouldBeTrue();

            theResultingObjectDef.FindDependencyDefinitionFor<IConditional>()
                .Type.ShouldEqual(typeof (LambdaConditional));
        }

        [Test]
        public void replace_the_conditional_with_a_service_lamda()
        {
            theNode.ConditionByService<ISomeService>(x => true);

            theNode.HasCondition().ShouldBeTrue();

            theResultingObjectDef.FindDependencyDefinitionFor<IConditional>()
                .Type.ShouldEqual(typeof(LambdaConditional<ISomeService>));
        }

        [Test]
        public void replace_the_conditional_with_a_model_lambda()
        {
            theNode.ConditionByModel<ISomeService>(x => true);

            theNode.HasCondition().ShouldBeTrue();

            theResultingObjectDef.FindDependencyDefinitionFor<IConditional>()
                .Type.ShouldEqual(typeof(LambdaConditional<IFubuRequest>));
        }
    }

    public interface ISomeService{}
    public class FakeConditional : IConditional
    {
        public bool ShouldExecute()
        {
            throw new NotImplementedException();
        }
    }

    public class FakeWriterNode : WriterNode
    {
        public override Type ResourceType
        {
            get { return typeof(TheResource); }
        }

        public ObjectDef TheWriterDef = ObjectDef.ForValue(MockRepository.GenerateMock<IMediaWriter<TheResource>>());

        protected override ObjectDef toWriterDef()
        {
            return TheWriterDef;
        }
    }

    public class TheResource{}
}