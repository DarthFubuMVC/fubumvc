using FubuMVC.Core;
using FubuMVC.Core.Conneg;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace FubuMVC.Tests.Conneg
{
    [TestFixture]
    public class ConnegNodeTester
    {
        [Test]
        public void build_object_def_with_both_input_and_output()
        {
            var node = new ConnegNode{
                InputType = typeof (ConnegNodeInput),
                OutputType = typeof (ConnegNodeOutput)
            };

            var objectDef = node.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None);

            objectDef.Type.ShouldEqual(typeof (ConnegBehavior));

            objectDef.DependencyFor<IConnegInputHandler>().ShouldBeOfType<ConfiguredDependency>()
                .Definition.Type.ShouldEqual(typeof(ConnegInputHandler<ConnegNodeInput>));

            objectDef.DependencyFor<IConnegOutputHandler>().ShouldBeOfType<ConfiguredDependency>()
                .Definition.Type.ShouldEqual(typeof(ConnegOutputHandler<ConnegNodeOutput>));
        }

        [Test]
        public void build_object_with_input_but_no_output()
        {
            var node = new ConnegNode
            {
                InputType = typeof(ConnegNodeInput),
                OutputType = null
            };

            var objectDef = node.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None);

            objectDef.Type.ShouldEqual(typeof(ConnegBehavior));

            objectDef.DependencyFor<IConnegInputHandler>().ShouldBeOfType<ConfiguredDependency>()
                .Definition.Type.ShouldEqual(typeof(ConnegInputHandler<ConnegNodeInput>));

            objectDef.DependencyFor<IConnegOutputHandler>().ShouldBeOfType<ConfiguredDependency>()
                .Definition.Type.ShouldEqual(typeof(NulloConnegHandler)); 
        }

        [Test]
        public void build_object_def_with_output_but_no_input()
        {
            var node = new ConnegNode
            {
                InputType = null,
                OutputType = typeof(ConnegNodeOutput)
            };

            var objectDef = node.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None);

            objectDef.Type.ShouldEqual(typeof(ConnegBehavior));

            objectDef.DependencyFor<IConnegInputHandler>().ShouldBeOfType<ConfiguredDependency>()
                .Definition.Type.ShouldEqual(typeof(NulloConnegHandler));

            objectDef.DependencyFor<IConnegOutputHandler>().ShouldBeOfType<ConfiguredDependency>()
                .Definition.Type.ShouldEqual(typeof(ConnegOutputHandler<ConnegNodeOutput>));
        }
    }

    public class ConnegNodeInput{}
    public class ConnegNodeOutput{}
}