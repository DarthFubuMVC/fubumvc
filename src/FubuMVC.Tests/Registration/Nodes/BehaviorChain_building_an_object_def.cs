using System;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Nodes
{
    [TestFixture]
    public class BehaviorChain_building_an_object_def
    {
        private BehaviorChain theChain;
        private Guid theOriginalGuid;

        [SetUp]
        public void SetUp()
        {
            theChain = new BehaviorChain();
            var action = ActionCall.For<Controller1>(x => x.Go(null));
            theChain.AddToEnd(action);

            theOriginalGuid = action.UniqueId;
        }

        private ObjectDef toObjectDef(DiagnosticLevel level)
        {
            return theChain.As<IContainerModel>().ToObjectDef(); 
        }

        [Test]
        public void no_diagnostic_behavior_for_no_diagnostics()
        {
            toObjectDef(DiagnosticLevel.None).Type.ShouldNotEqual(typeof (DiagnosticBehavior));
        }

        [Test]
        public void the_unique_id_matches_the_top_id_in_no_diagnostic_mode()
        {
            toObjectDef(DiagnosticLevel.None).Name.ShouldEqual(theOriginalGuid);
        }


        public class Controller1
        {
            public void Go(Input1 input){}

            public class Input1{}
        }
    }

    
}