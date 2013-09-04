using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Logging;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Logging;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using FubuMVC.Core;
using FubuCore;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class when_trying_to_set_an_object_in_fubu_request : InteractionContext<FubuRequest>
    {
        private BindResult theResult;

        protected override void beforeEach()
        {
            theResult = new BindResult{
                Problems = new List<ConvertProblem>{
                    new ConvertProblem(),
                    new ConvertProblem()
                },
                Value = new BinderTarget()
            };

            MockFor<IObjectResolver>().Stub(x => x.BindModel(typeof (BinderTarget), MockFor<IRequestData>()))
                .Return(theResult);

            ClassUnderTest.Get<BinderTarget>().ShouldBeTheSameAs(theResult.Value);
        }

        [Test]
        public void try_to_set_the_same_value_does_nothing()
        {
            ClassUnderTest.Set(theResult.Value.As<BinderTarget>());

            ClassUnderTest.ProblemsFor<BinderTarget>().ShouldHaveTheSameElementsAs(theResult.Problems);
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
    public class when_clearing_a_type_from_the_request : InteractionContext<FubuRequest>
    {
        private BinderTarget _binderTarget;

        protected override void beforeEach()
        {
            _binderTarget = new BinderTarget();
            ClassUnderTest.Set(_binderTarget);
        }

        [Test]
        public void should_not_do_anything_if_the_type_does_not_exist_in_the_request()
        {
            ClassUnderTest.Clear(typeof(when_clearing_a_type_from_the_request));
            ClassUnderTest.Get(_binderTarget.GetType()).ShouldBeTheSameAs(_binderTarget);
        }

        [Test]
        public void should_not_clear_a_subclass_type_when_instructed_to_clear_a_parent_type()
        {
            ClassUnderTest.Clear(typeof(BinderTargetBase));
            ClassUnderTest.Get(_binderTarget.GetType()).ShouldBeTheSameAs(_binderTarget);
        }

        [Test]
        public void should_remove_the_type_from_the_request_if_it_exists()
        {
            ClassUnderTest.Clear(_binderTarget.GetType());
            typeof(NullReferenceException).ShouldBeThrownBy(()=> ClassUnderTest.Get(_binderTarget.GetType()).ShouldBeNull());
        }
    }

    [TestFixture]
    public class setting_an_object_explicitly_registers_the_new_object : InteractionContext<FubuRequest>
    {
        private BinderTarget registered;
        private FubuRequest request;
        private RecordingLogger logs;

        protected override void beforeEach()
        {
            logs = RecordLogging();

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
        public void setting_the_object_records_a_SetValue_log()
        {
            request.SetObject(registered);

            logs.DebugMessages.Single().ShouldEqual(new SetValueReport(registered));

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

        [Test]
        public void trace_the_set_object_from_generic_set()
        {
            request.Set<BinderTargetBase>(registered);

            logs.DebugMessages.Single().ShouldEqual(SetValueReport.For<BinderTargetBase>(registered));
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