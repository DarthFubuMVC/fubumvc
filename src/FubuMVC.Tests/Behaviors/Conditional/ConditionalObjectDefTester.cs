using System;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.StructureMap;
using NUnit.Framework;
using StructureMap;
using FubuTestingSupport;

namespace FubuMVC.Tests.Behaviors.Conditional
{
    [TestFixture]
    public class ConditionalObjectDefTester
    {
        private Container theContainer;
        private StructureMapContainerFacility theFacility;
        private InMemoryFubuRequest theRequest;

        [SetUp]
        public void SetUp()
        {
            theContainer = new Container();
            theFacility = new StructureMapContainerFacility(theContainer);

            theRequest = new InMemoryFubuRequest();
        
            theContainer.Inject<IFubuRequest>(theRequest);
        }

        private IConditional build(ObjectDef def)
        {
            theFacility.Register(typeof(IConditional), def);
            theFacility.BuildFactory();

            return theContainer.GetInstance<IConditional>(def.Name);
        }

        [Test]
        public void for_conditional_type()
        {
            var def = ConditionalObjectDef.For<AlwaysTrue>();
            build(def).ShouldBeOfType<AlwaysTrue>();
        }

        [Test]
        public void for_filter_positive()
        {
            var def = ConditionalObjectDef.For(() => true);
            build(def).ShouldBeOfType<LambdaConditional>().ShouldExecute().ShouldBeTrue();
        }

        [Test]
        public void for_filter_negative()
        {
            var def = ConditionalObjectDef.For(() => false);
            build(def).ShouldBeOfType<LambdaConditional>().ShouldExecute().ShouldBeFalse();
        }

        [Test]
        public void for_model_positive()
        {
            var model = new Model{
                Name = "Jeremy"
            };

            theRequest.Set(model);

            var def = ConditionalObjectDef.ForModel<Model>(x => x.Name == "Jeremy");
            var conditional = build(def);

            conditional.ShouldExecute().ShouldBeTrue();

            // Change the name on the model should now make the test false
            model.Name = "Jessica";
            conditional.ShouldExecute().ShouldBeFalse();
        }

        [Test]
        public void for_service_positive()
        {
            var theService = new Service(){IsTrue = true};
            theContainer.Inject(theService);

            var def = ConditionalObjectDef.ForService<Service>(x => x.IsTrue);
            build(def).ShouldExecute().ShouldBeTrue();
        }

        [Test]
        public void for_service_negative()
        {
            var theService = new Service(){IsTrue = false};
            theContainer.Inject(theService);

            var def = ConditionalObjectDef.ForService<Service>(x => x.IsTrue);
            build(def).ShouldExecute().ShouldBeFalse();
        }

        [Test]
        public void for_conditional_type_positive()
        {
            var def = ConditionalObjectDef.For(typeof (AlwaysTrue));
            def.Type.ShouldEqual(typeof (AlwaysTrue));
        }

        [Test]
        public void for_conditional_with_the_wrong_kind_of_type()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                ConditionalObjectDef.For(typeof (Model));
            });
        }

        public class Service
        {
            public bool IsTrue = false;
        }

        public class Model
        {
            public string Name { get; set; }
        }


        public class AlwaysTrue : IConditional
        {
            public bool ShouldExecute()
            {
                throw new NotImplementedException();
            }
        }
    }
}