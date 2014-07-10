using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Querying
{
    [TestFixture]
    public class ChainResolverTester
    {
        #region Setup/Teardown

        [TestFixtureSetUp]
        public void SetUp()
        {
            graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<ChainResolverController>();
            });

            typeResolver = new TypeResolver();
            typeResolver.AddStrategy<ProxyDetector>();

            _resolutionCache = new ChainResolutionCache(typeResolver, graph);
        }

        [SetUp]
        public void CleanUp()
        {
            graph.Behaviors.OfType<RoutedChain>().Each(x => {
                x.UrlCategory.Category = null;
                x.UrlCategory.Creates.Clear();
            });

            _resolutionCache.ClearAll();

            graph.Forwarders.Clear();
        }

        #endregion

        private BehaviorGraph graph;
        private TypeResolver typeResolver;
        private ChainResolutionCache _resolutionCache;

        [Test]
        public void find_by_controller_action_failure_throws_2104()
        {
            Exception<FubuException>.ShouldBeThrownBy(
                () => { _resolutionCache.Find<ControllerThatIsNotRegistered>(x => x.Go()); }).ErrorCode.ShouldEqual(2104);
        }

        [Test]
        public void find_by_controller_action_success()
        {
            _resolutionCache.Find<ChainResolverController>(x => x.M1()).FirstCall().Method.Name.ShouldEqual("M1");
        }

        [Test]
        public void find_by_handler_type_and_method_positive_case()
        {
            var method = ReflectionHelper.GetMethod<ChainResolverController>(x => x.M7(null));
            _resolutionCache.Find(typeof (ChainResolverController), method).FirstCall().Method.ShouldEqual(method);
        }

        [Test]
        public void find_by_type_with_method_null_can_find_by_input_type()
        {
            _resolutionCache.Find(typeof (UniqueInput), null).FirstCall().Method.Name.ShouldEqual("M9");
        }

        [Test]
        public void find_by_input_model_and_category_fails_when_there_are_multiple_matching_chains()
        {
            var routedChain = graph.BehaviorFor<ChainResolverController>(x => x.M2(null))
                .As<RoutedChain>();
            routedChain
                .UrlCategory.Category = Categories.NEW;

            graph.BehaviorFor<ChainResolverController>(x => x.M3(null))
                .As<RoutedChain>()
                .UrlCategory.Category = Categories.NEW;

            Exception<FubuException>.ShouldBeThrownBy(
                () => { _resolutionCache.FindUnique(new ChainResolverInput1(), Categories.NEW).FirstCall(); }).ErrorCode
                .ShouldEqual(2108);
        }

        [Test]
        public void find_by_input_model_and_category_fails_when_there_are_no_matching_chains()
        {
            Exception<FubuException>.ShouldBeThrownBy(
                () => { _resolutionCache.FindUnique(new ChainResolverInput1(), Categories.NEW).FirstCall(); }).ErrorCode
                .ShouldEqual(2104);
        }

        [Test]
        public void find_by_input_model_and_category_success()
        {
            graph.BehaviorFor<ChainResolverController>(x => x.M2(null))
                .As<RoutedChain>()
                .UrlCategory.Category = Categories.NEW;

            graph.BehaviorFor<ChainResolverController>(x => x.M3(null))
                .As<RoutedChain>()
                .UrlCategory.Category = Categories.EDIT;

            _resolutionCache.FindUnique(new ChainResolverInput1(), Categories.NEW).FirstCall().Method.Name.ShouldEqual(
                "M2");
            _resolutionCache.FindUnique(new ChainResolverInput1(), Categories.EDIT).FirstCall().Method.Name.ShouldEqual(
                "M3");
        }

        [Test]
        public void find_chain_by_http_method()
        {
            _resolutionCache.FindUniqueByType(typeof (ChainResolverInput4), "POST")
                .FirstCall().Description.ShouldContain("post_input");

            _resolutionCache.FindUniqueByType(typeof (ChainResolverInput4), "GET")
                .FirstCall().Description.ShouldContain("get_input");
        }

        [Test]
        public void find_creator_negative()
        {
            _resolutionCache.FindCreatorOf(typeof (Entity1)).ShouldBeNull();
        }

        [Test]
        public void find_creator_positive()
        {
            var chain = graph.BehaviorFor<ChainResolverController>(x => x.M6(null));
            chain.As<RoutedChain>().UrlCategory.Creates.Add(typeof(Entity1));
            chain.As<RoutedChain>().UrlCategory.Creates.Add(typeof(Entity2));

            _resolutionCache.FindCreatorOf(typeof (Entity1)).FirstCall().Method.Name.ShouldEqual("M6");
            _resolutionCache.FindCreatorOf(typeof (Entity2)).FirstCall().Method.Name.ShouldEqual("M6");
            _resolutionCache.FindCreatorOf(typeof (Entity3)).ShouldBeNull();
        }

        [Test]
        public void find_forwarder_if_there_is_only_one()
        {
            graph.Forward<ForwardedModel>(m => new UniqueInput());

            var forwarder = _resolutionCache.FindForwarder(new ForwardedModel());
            forwarder.ShouldNotBeNull();
            forwarder.FindChain(_resolutionCache, new ForwardedModel()).Chain.FirstCall().Method.Name.ShouldEqual("M9");
        }

        [Test]
        public void find_forwarder_is_null_with_no_forwarders_registered()
        {
            _resolutionCache.FindForwarder(new ForwardedModel()).ShouldBeNull();
        }

        [Test]
        public void find_forwarder_with_DEFAULT_category_if_there_is_more_than_one()
        {
            graph.Forward<ForwardedModel>(m => new UniqueInput(), Categories.DEFAULT);
            graph.Forward<ForwardedModel>(m => new ChainResolverInput1(), Categories.NEW);
            graph.Forward<ForwardedModel>(m => new ChainResolverInput1(), Categories.EDIT);

            var forwarder = _resolutionCache.FindForwarder(new ForwardedModel());
            forwarder.ShouldNotBeNull();
            forwarder.FindChain(_resolutionCache, new ForwardedModel()).Chain.FirstCall().Method.Name.ShouldEqual("M9");
        }

        [Test]
        public void find_unique_can_revert_to_using_the_DEFAULT_category_if_it_exists()
        {
            graph.BehaviorFor<ChainResolverController>(x => x.M3(null))
                .As<RoutedChain>()
                .UrlCategory.Category = Categories.DEFAULT;

            _resolutionCache.FindUnique(new ChainResolverInput1()).FirstCall().Method.Name.ShouldEqual("M3");
        }

        [Test]
        public void find_unique_can_succeed_with_multiple_options_if_all_but_one_are_categorized()
        {
            graph.BehaviorFor<ChainResolverController>(x => x.M2(null))
                .As<RoutedChain>()
                .UrlCategory.Category = Categories.NEW;

            graph.BehaviorFor<ChainResolverController>(x => x.M3(null))
                .As<RoutedChain>()
                .UrlCategory.Category = Categories.EDIT;

            _resolutionCache.FindUnique(new ChainResolverInput1()).FirstCall().Method.Name.ShouldEqual("M4");
        }

        [Test]
        public void find_unique_failure_when_there_are_multiple_options()
        {
            Exception<FubuException>.ShouldBeThrownBy(() => { _resolutionCache.FindUnique(new ChainResolverInput1()); })
                .ErrorCode.ShouldEqual(2108);
        }

        [Test]
        public void find_unique_for_an_unknown_input_type_throws_exception()
        {
            Exception<FubuException>.ShouldBeThrownBy(
                () => { _resolutionCache.FindUnique(new InputModelThatDoesNotMatchAnyExistingBehaviors()); }).ErrorCode.
                ShouldEqual(2104);
        }

        [Test]
        public void find_unique_should_respect_the_type_resolution()
        {
            _resolutionCache.FindUnique(new Proxy<UniqueInput>()).FirstCall().Method.Name.ShouldEqual("M9");
        }

        [Test]
        public void find_unique_success()
        {
            _resolutionCache.FindUnique(new UniqueInput()).FirstCall().Method.Name.ShouldEqual("M9");
        }

        [Test]
        public void find_unique_with_category_respects_the_type_resolution()
        {
            graph.BehaviorFor<ChainResolverController>(x => x.M2(null))
                .As<RoutedChain>()
                .UrlCategory.Category = Categories.NEW;

            _resolutionCache.FindUnique(new Proxy<ChainResolverInput1>(), Categories.NEW)
                .FirstCall().Method.Name.ShouldEqual("M2");
        }

        [Test]
        public void has_new_negative()
        {
            _resolutionCache.FindCreatorOf(typeof (Entity1)).ShouldBeNull();
        }
    }

    public class InputModelThatDoesNotMatchAnyExistingBehaviors
    {
    }

    public class ControllerThatIsNotRegistered
    {
        public void Go()
        {
        }
    }

    public class ChainResolverController
    {
        public void M1()
        {
        }

        public void M2(ChainResolverInput1 input)
        {
        }

        public void M3(ChainResolverInput1 input)
        {
        }

        public void M4(ChainResolverInput1 input)
        {
        }

        public void M5(ChainResolverInput2 input)
        {
        }

        public void M6(ChainResolverInput2 input)
        {
        }

        public void M7(ChainResolverInput2 input)
        {
        }

        public void M8(ChainResolverInput3 input)
        {
        }

        public void M9(UniqueInput input)
        {
        }

        public void post_input(ChainResolverInput4 input)
        {
        }

        public void get_input(ChainResolverInput4 input)
        {
        }
    }

    public class ChainResolverInput1
    {
    }

    public class ChainResolverInput2
    {
    }

    public class ChainResolverInput3
    {
    }

    public class ChainResolverInput4
    {
    }

    public class UniqueInput
    {
    }

    public class ForwardedModel
    {
    }

    public class ProxyDetector : ITypeResolverStrategy
    {
        public Type ResolveType(object model)
        {
            return model.GetType().GetGenericArguments()[0];
        }

        public bool Matches(object model)
        {
            return model.GetType().Closes(typeof (Proxy<>));
        }
    }

    public class Proxy<T>
    {
    }


    public class Entity1
    {
    }

    public class Entity2
    {
    }

    public class Entity3
    {
    }
}