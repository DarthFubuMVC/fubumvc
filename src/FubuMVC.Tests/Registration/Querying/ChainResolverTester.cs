using System;
using System.Diagnostics;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Urls;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

namespace FubuMVC.Tests.Registration.Querying
{
    [TestFixture]
    public class ChainResolverTester
    {
        private BehaviorGraph graph;
        private TypeResolver typeResolver;
        private ChainResolver resolver;

        [SetUp]
        public void SetUp()
        {
            graph = new FubuRegistry(x =>
            {
                x.Actions.IncludeType<ChainResolverController>();
            }).BuildGraph();

            graph.Behaviors.Each(x => Debug.WriteLine(x.FirstCallDescription));

            typeResolver = new TypeResolver();
            typeResolver.AddStrategy<ProxyDetector>();

            resolver = new ChainResolver(typeResolver, graph);
        }

        [Test]
        public void has_new_negative()
        {
            resolver.FindCreatorOf(typeof (Entity1)).ShouldBeNull();
        }

        [Test]
        public void find_creator_positive()
        {
            var chain = graph.BehaviorFor<ChainResolverController>(x => x.M6(null));
            chain.UrlCategory.Creates.Add(typeof(Entity1));
            chain.UrlCategory.Creates.Add(typeof(Entity2));

            resolver.FindCreatorOf(typeof (Entity1)).FirstCall().Method.Name.ShouldEqual("M6");
            resolver.FindCreatorOf(typeof (Entity2)).FirstCall().Method.Name.ShouldEqual("M6");
            resolver.FindCreatorOf(typeof (Entity3)).ShouldBeNull();
        }

        [Test]
        public void find_creator_negative()
        {
            resolver.FindCreatorOf(typeof(Entity1)).ShouldBeNull();
        }

        [Test]
        public void find_chains_per_input_model()
        {
            resolver.Find(new ChainResolverInput1()).Select(x => x.FirstCall().Method.Name)
                .ShouldHaveTheSameElementsAs("M2", "M3", "M4");
        }

        [Test]
        public void find_chains_per_input_model_respects_the_type_resolution()
        {
            resolver.Find(new Proxy<ChainResolverInput1>()).Select(x => x.FirstCall().Method.Name)
                .ShouldHaveTheSameElementsAs("M2", "M3", "M4");
        }

        [Test]
        public void find_unique_success()
        {
            resolver.FindUnique(new UniqueInput()).FirstCall().Method.Name.ShouldEqual("M9");
        }

        [Test]
        public void find_unique_for_an_unknown_input_type_throws_exception()
        {
            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                resolver.FindUnique(new InputModelThatDoesNotMatchAnyExistingBehaviors());
            }).ErrorCode.ShouldEqual(2102);
        }
        
        [Test]
        public void find_unique_failure_when_there_are_multiple_options()
        {
            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                resolver.FindUnique(new ChainResolverInput1());
            }).ErrorCode.ShouldEqual(2103);
        }

        [Test]
        public void find_unique_can_revert_to_using_the_DEFAULT_category_if_it_exists()
        {
            graph.BehaviorFor<ChainResolverController>(x => x.M3(null))
                .UrlCategory.Category = Categories.DEFAULT;

            resolver.FindUnique(new ChainResolverInput1()).FirstCall().Method.Name.ShouldEqual("M3");
        }

        [Test]
        public void find_unique_can_succeed_with_multiple_options_if_all_but_one_are_categorized()
        {
            graph.BehaviorFor<ChainResolverController>(x => x.M2(null))
                .UrlCategory.Category = Categories.NEW;

            graph.BehaviorFor<ChainResolverController>(x => x.M3(null))
                .UrlCategory.Category = Categories.EDIT;

            resolver.FindUnique(new ChainResolverInput1()).FirstCall().Method.Name.ShouldEqual("M4");
        }

        [Test]
        public void find_by_controller_action_success()
        {
            resolver.Find<ChainResolverController>(x => x.M1()).FirstCall().Method.Name.ShouldEqual("M1");
        }

        [Test]
        public void find_by_controller_action_failure_throws_2106()
        {
            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                resolver.Find<ControllerThatIsNotRegistered>(x => x.Go());
            }).ErrorCode.ShouldEqual(2108);
        }

        [Test]
        public void find_by_input_model_and_category_success()
        {
            graph.BehaviorFor<ChainResolverController>(x => x.M2(null))
                .UrlCategory.Category = Categories.NEW;

            graph.BehaviorFor<ChainResolverController>(x => x.M3(null))
                .UrlCategory.Category = Categories.EDIT;

            resolver.FindUnique(new ChainResolverInput1(), Categories.NEW).FirstCall().Method.Name.ShouldEqual("M2");
            resolver.FindUnique(new ChainResolverInput1(), Categories.EDIT).FirstCall().Method.Name.ShouldEqual("M3");
        }

        [Test]
        public void find_by_input_model_and_category_fails_when_there_are_no_matching_chains()
        {
            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                resolver.FindUnique(new ChainResolverInput1(), Categories.NEW).FirstCall();
            }).ErrorCode.ShouldEqual(2104);
        }

        [Test]
        public void find_by_input_model_and_category_fails_when_there_are_multiple_matching_chains()
        {
            graph.BehaviorFor<ChainResolverController>(x => x.M2(null))
                .UrlCategory.Category = Categories.NEW;

            graph.BehaviorFor<ChainResolverController>(x => x.M3(null))
                .UrlCategory.Category = Categories.NEW;

            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                resolver.FindUnique(new ChainResolverInput1(), Categories.NEW).FirstCall();
            }).ErrorCode.ShouldEqual(2105);
        }

        [Test]
        public void find_unique_should_respect_the_type_resolution()
        {
            resolver.FindUnique(new Proxy<UniqueInput>()).FirstCall().Method.Name.ShouldEqual("M9");
        }

        [Test]
        public void find_unique_with_category_respects_the_type_resolution()
        {
            graph.BehaviorFor<ChainResolverController>(x => x.M2(null))
                .UrlCategory.Category = Categories.NEW;

            resolver.FindUnique(new Proxy<ChainResolverInput1>(), Categories.NEW)
                .FirstCall().Method.Name.ShouldEqual("M2");
        }

        [Test]
        public void find_by_handler_type_and_method_positive_case()
        {
            var method = ReflectionHelper.GetMethod<ChainResolverController>(x => x.M7(null));
            resolver.Find(typeof(ChainResolverController), method).FirstCall().Method.ShouldEqual(method);
        }

        [Test]
        public void find_forwarder_if_there_is_only_one()
        {
            graph.Forward<ForwardedModel>(m => new UniqueInput());

            var forwarder = resolver.FindForwarder(new ForwardedModel());
            forwarder.ShouldNotBeNull();

            forwarder.FindChain(resolver, new ForwardedModel()).FirstCall().Method.Name.ShouldEqual("M9");
        }

        [Test]
        public void find_forwarder_is_null_with_no_forwarders_registered()
        {
            resolver.FindForwarder(new ForwardedModel()).ShouldBeNull();
        }

        [Test]
        public void find_forwarder_with_DEFAULT_category_if_there_is_more_than_one()
        {
            graph.Forward<ForwardedModel>(m => new UniqueInput(), Categories.DEFAULT);
            graph.Forward<ForwardedModel>(m => new ChainResolverInput1(), Categories.NEW);
            graph.Forward<ForwardedModel>(m => new ChainResolverInput1(), Categories.EDIT);

            var forwarder = resolver.FindForwarder(new ForwardedModel());
            forwarder.ShouldNotBeNull();

            forwarder.FindChain(resolver, new ForwardedModel()).FirstCall().Method.Name.ShouldEqual("M9");
        }

    }

    public class InputModelThatDoesNotMatchAnyExistingBehaviors{}

    public class ControllerThatIsNotRegistered
    {
        public void Go(){}
    }

    public class ChainResolverController
    {
        public void M1()
        {
        }

        public void M2(ChainResolverInput1 input){}
        public void M3(ChainResolverInput1 input){}
        public void M4(ChainResolverInput1 input){}
        public void M5(ChainResolverInput2 input){}
        public void M6(ChainResolverInput2 input){}
        public void M7(ChainResolverInput2 input){}
        public void M8(ChainResolverInput3 input){}
        public void M9(UniqueInput input){}
    }

    public class ChainResolverInput1{}
    public class ChainResolverInput2{}
    public class ChainResolverInput3{}
    public class UniqueInput{}
    public class ForwardedModel{}

    public class ProxyDetector : ITypeResolverStrategy
    {
        public Type ResolveType(object model)
        {
            return model.GetType().GetGenericArguments()[0];
        }

        public bool Matches(object model)
        {
            return model.GetType().Closes(typeof(Proxy<>));
        }
    }

    public class Proxy<T> { }


    public class Entity1{}
    public class Entity2{}
    public class Entity3{}
}