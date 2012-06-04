using System;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Tests.Diagnostics;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Nodes
{
    [TestFixture]
    public class Behavior_node_ObjectDef_creation_Tester
    {
        [Test]
        public void creating_an_object_def_for_no_tracing()
        {
            var node = new Wrapper(typeof(SimpleBehavior));
            node.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None)
                .Type.ShouldEqual(typeof(SimpleBehavior));


        }

        public class SimpleBehavior : IActionBehavior
        {
            public void Invoke()
            {

            }

            public void InvokePartial()
            {
            }
        }

        public class DifferentBehavior : SimpleBehavior { }
    

    }
}