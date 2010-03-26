using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using FubuMVC.Core;

namespace FubuMVC.Tests.Runtime
{
    public abstract class ObjectResolverContext
    {
        protected List<IModelBinder> binders;
        protected InMemoryBindingContext data;
        protected ObjectResolver resolver;


        [SetUp]
        public void SetUp()
        {
            binders = new List<IModelBinder>();
            data = new InMemoryBindingContext();

            setupContext();

            var container = StructureMapContainerFacility.GetBasicFubuContainer();
            container.Configure(x =>
            {
                binders.Each(b => x.For<IModelBinder>().Add(b));
            });

            resolver = container.GetInstance<ObjectResolver>();
        }

        protected abstract void setupContext();
    }


    [TestFixture]
    public class fetching_an_object_should_choose_the_first_model_binder_applicable : ObjectResolverContext
    {
        private IModelBinder binder1;
        private IModelBinder binder2;
        private IModelBinder matchingBinder;
        private BinderTarget expectedResult;

        protected override void setupContext()
        {
            binder1 = MockRepository.GenerateMock<IModelBinder>();
            binder2 = MockRepository.GenerateMock<IModelBinder>();
            matchingBinder = MockRepository.GenerateMock<IModelBinder>();

            binder1.Stub(x => x.Matches(typeof (BinderTarget))).Return(false);
            binder2.Stub(x => x.Matches(typeof (BinderTarget))).Return(false);
            matchingBinder.Stub(x => x.Matches(typeof (BinderTarget))).Return(true);

            expectedResult = new BinderTarget();

            binders.Add(binder1);
            binders.Add(binder2);
            binders.Add(matchingBinder);

            matchingBinder.Stub(x => x.Bind(typeof (BinderTarget), data)).Return(expectedResult);
        }

        [Test]
        public void should_resolve_the_requested_model_with_the_first_binder_that_matches()
        {
            resolver.BindModel(typeof (BinderTarget), data).Value.ShouldBeTheSameAs(expectedResult);
        }
    }

    [TestFixture]
    public class fetching_an_object_for_the_second_time_gets_the_same_object : InteractionContext<FubuRequest>
    {
        private object target;

        protected override void beforeEach()
        {
            target = new BinderTarget();

            MockFor<IObjectResolver>().Stub(x => x.BindModel(typeof (BinderTarget), MockFor<IRequestData>()))
                .Return(new BindResult
                {
                    Value = target,
                    Problems = new List<ConvertProblem>()
                });
        }

        [Test]
        public void the_object_is_only_created_once()
        {
            FubuRequest request = ClassUnderTest;
            var target1 = request.Get<BinderTarget>();

            request.Get<BinderTarget>().ShouldBeTheSameAs(target1);
            request.Get<BinderTarget>().ShouldBeTheSameAs(target1);
            request.Get<BinderTarget>().ShouldBeTheSameAs(target1);
        }
    }

    [TestFixture]
    public class when_finding_something_by_a_base_class : InteractionContext<FubuRequest>
    {
        [Test]
        public void inject_by_concrete_find_by_abstraction()
        {
            FubuRequest request = ClassUnderTest;

            var target = new BinderTarget();
            request.Set(target);

            request.Get<BinderTarget>().ShouldBeTheSameAs(target);


            request.Find<BinderTargetBase>().First().ShouldBeTheSameAs(target);
        }
    }

    [TestFixture]
    public class fetching_an_object_that_had_conversion_problems : ObjectResolverContext
    {
        protected override void setupContext()
        {
            data["Age"] = "abc";
            data["Name"] = "Jeremy";
        }

        [Test]
        public void the_conversion_problems_should_be_recorded()
        {
            resolver.BindModel(typeof (BinderTarget), data).Problems.Count().ShouldEqual(1);
        }
    }

    [TestFixture]
    public class setting_an_object_explicitly_registers_the_new_object : InteractionContext<FubuRequest>
    {
        private BinderTarget registered;
        private FubuRequest request;

        protected override void beforeEach()
        {
            registered = new BinderTarget();
            request = ClassUnderTest;
        }


        [Test]
        public void the_object_registered_by_set_object_is_always_returned_from_get()
        {
            request.SetObject(registered);

            request.Get<BinderTarget>().ShouldBeTheSameAs(registered);
            request.Get<BinderTarget>().ShouldBeTheSameAs(registered);
            request.Get<BinderTarget>().ShouldBeTheSameAs(registered);

            request.ProblemsFor<BinderTarget>().Count().ShouldEqual(0);
        }

        [Test]
        public void the_object_registered_is_always_returned_from_get()
        {
            request.Set(registered);

            request.Get<BinderTarget>().ShouldBeTheSameAs(registered);
            request.Get<BinderTarget>().ShouldBeTheSameAs(registered);
            request.Get<BinderTarget>().ShouldBeTheSameAs(registered);

            request.ProblemsFor<BinderTarget>().Count().ShouldEqual(0);
        }
    }


    public class BinderTargetBase
    {
    }

    public class BinderTarget : BinderTargetBase
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsTrue { get; set; }
    }

    public class BinderTarget2
    {
    }
}