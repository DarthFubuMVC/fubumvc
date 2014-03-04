using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.View.Attachment
{
    [TestFixture]
    public class ActionlessViewConventionTester
    {
        private BehaviorGraph theGraph;
        private IViewFacility theFacility;
        private StubViewToken t1;
        private StubViewToken t2;

        [SetUp]
        public void SetUp()
        {
            t1 = new StubViewToken { ViewModel  =  typeof(StubViewModel1) };
            t2 = new StubViewToken { ViewModel = typeof(StubViewModel2) };

            theFacility = new StubViewFacility(t1, t2);

            theGraph = BehaviorGraph.BuildFrom(registry => registry.AlterSettings<ViewEngines>(views => views.AddFacility(theFacility)));
            theGraph.AddChain(chainWithOutput<StubViewModel2>(t2));

            new ActionlessViewConvention().Configure(theGraph);
        }

        [Test]
        public void adds_the_actionless_chain()
        {
            actionlessChain.ShouldNotBeNull();
        }

        [Test]
        public void marks_the_actionless_chain_as_partial_only()
        {
            actionlessChain.IsPartialOnly.ShouldBeTrue();
        }

        [Test]
        public void marks_the_actionless_chain_with_the_view_category()
        {
            actionlessChain.MatchesCategoryOrHttpMethod(Categories.VIEW).ShouldBeTrue();
        }

        private BehaviorChain actionlessChain { get { return theGraph.BehaviorFor(typeof (StubViewModel1)); } }

        private BehaviorChain chainWithOutput<T>(IViewToken token) where T : class
        {
            throw new NotImplementedException();
//            var chain = new BehaviorChain();
//            chain.AddToEnd(ActionCall.For<StubController<T>>(x => x.Post(null)));
//            chain.Output.Writers.AddToEnd(new ViewNode(token));
//
//            return chain;
        }

        public class StubViewModel1
        {
        }

        public class StubViewModel2
        {
        }

        public class StubController<T> where T : class
        {
            public T Post(T input)
            {
                return input;
            }
        }

        public class StubViewToken : IViewToken
        {
            public Type ViewType { get; set; }
            public Type ViewModel { get; set; }

            public string Name()
            {
                throw new NotImplementedException();
            }

            public string Namespace { get; private set; }

            public ObjectDef ToViewFactoryObjectDef()
            {
                throw new NotImplementedException();
            }

            public IRenderableView GetView()
            {
                throw new NotImplementedException();
            }

            public IRenderableView GetPartialView()
            {
                throw new NotImplementedException();
            }

            public string ProfileName { get; set; }
        }

        public class StubViewFacility : IViewFacility
        {
            private readonly IEnumerable<IViewToken> _tokens;

            public StubViewFacility(params IViewToken[] tokens)
            {
                _tokens = tokens;
            }

            public IEnumerable<IViewToken> FindViews(BehaviorGraph graph)
            {
                return _tokens;
            }
        }
    }
}