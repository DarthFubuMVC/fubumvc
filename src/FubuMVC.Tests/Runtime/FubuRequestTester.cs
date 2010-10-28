using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using FubuMVC.Core;
using FubuCore;

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

            var container = StructureMapContainerFacility.GetBasicFubuContainer(x => binders.Each(b => x.For<IModelBinder>().Add(b)));

            resolver = container.GetInstance<ObjectResolver>();
        }

        protected abstract void setupContext();
    }

    [TestFixture]
    public class object_resolver_throws_exception_on_bind_model_with_no_matching_model_binder : InteractionContext<ObjectResolver>
    {
        [Test]
        public void throw_fubu_exception_if_there_is_no_suitable_binder()
        {
            MockFor<IModelBinderCache>().Stub(x => x.BinderFor(GetType())).Return(null);


            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.BindModel(typeof (ClassWithNoCtor), MockFor<IBindingContext>());
            }).ErrorCode.ShouldEqual(2200);



        }

        public class ClassWithNoCtor
        {
            private ClassWithNoCtor()
            {
            }
        }
    }

    [TestFixture]
    public class when_binding_model_throws_exception : ObjectResolverContext
    {
        private IModelBinder matchingBinder;
        private Type _type = typeof(BinderTarget);

        protected override void setupContext()
        {
            matchingBinder = MockRepository.GenerateStub<IModelBinder>();
            matchingBinder.Stub(x => x.Matches(_type)).Return(true);
            matchingBinder.Stub(x => x.Bind(_type, data)).Throw(new Exception("fake message"));
            binders.Add(matchingBinder);
        }

        [Test]
        public void should_throw_fubu_exception_2201()
        {
            FubuException fubuException = typeof(FubuException).ShouldBeThrownBy(() =>
                resolver.BindModel(_type, data)) as FubuException;
            fubuException.ShouldNotBeNull().Message.ShouldEqual(
                "FubuMVC Error 2201:  \nFatal error while binding model of type {0}.  See inner exception"
                .ToFormat(_type.AssemblyQualifiedName));
        }
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
    public class when_fetching_a_type_that_has_an_infinite_recursion
    {
        private IFubuRequest _fubuRequest;

        [SetUp]
        public void beforeEach()
        {
            IContainer container = StructureMapContainerFacility.GetBasicFubuContainer();
            _fubuRequest = container.GetInstance<IFubuRequest>();
        }

        [Test]
        public void should_mark_the_recursive_property_as_a_conversion_problem()
        {
            _fubuRequest.Get<BinderKiller>().ShouldNotBeNull();
            var convertProblems = _fubuRequest.ProblemsFor<BinderKiller>();
            convertProblems.ShouldHaveCount(1);
            convertProblems.Single().PropertyName().ShouldEqual("Friend.Unlucky.Enemy");
        }

        public class BinderKiller
        {
            public string Name { get; set; }
            public BinderAccomplice Friend { get; set; }
        }

        public class BinderAccomplice
        {
            public BinderBystander Unlucky { get; set; }
        }

        public class BinderBystander
        {
            public BinderNarc Enemy { get; set; }
        }

        public class BinderNarc
        {
            public BinderAccomplice Guilty { get; set; }
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
}