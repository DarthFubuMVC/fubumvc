using System;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using Shouldly;
using Xunit;
using StructureMap.Pipeline;

namespace FubuMVC.Tests.Registration.Nodes
{
    
    public class BehaviorChain_building_an_object_def
    {
        public BehaviorChain_building_an_object_def()
        {
            theChain = new BehaviorChain();
            var action = ActionCall.For<Controller1>(x => x.Go(null));
            theChain.AddToEnd(action);

            theOriginalGuid = action.UniqueId;
        }


        private BehaviorChain theChain;
        private Guid theOriginalGuid;

        private Instance toObjectDef()
        {
            return theChain.As<IContainerModel>().ToInstance();
        }

        public class Controller1
        {
            public void Go(Input1 input)
            {
            }

            #region Nested type: Input1

            public class Input1
            {
            }

            #endregion
        }

        [Fact]
        public void the_unique_id_matches_the_top_id_in_no_diagnostic_mode()
        {
            toObjectDef().Name.ShouldBe(theOriginalGuid.ToString());
        }
    }
}