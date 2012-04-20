using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.View
{
    [TestFixture]
    public class ViewBagConventionRunnerTester
    {
        private ViewBagConventionRunner _runner;

        [SetUp]
        public void Setup()
        {
            var types = new TypePool(null);
            _runner = new ViewBagConventionRunner(types);
        }

        [Test]
        public void should_not_add_same_type_of_facility_more_than_once()
        {
            _runner.AddFacility(new TestViewFacility());
            _runner.AddFacility(new TestViewFacility());

            _runner.Facilities.ShouldHaveCount(1);
        }

        [Test]
        public void should_run_inner_bag_convention()
        {
            _runner.AddFacility(new TestViewFacility());

            var convention = new TestViewBagConvention();
            _runner.Apply(convention);

            _runner
                .Configure(new FubuRegistry().BuildGraph());

            convention
                .Executed
                .ShouldBeTrue();
        }

        public class TestViewBagConvention : IViewBagConvention
        {
            public void Configure(ViewBag bag, BehaviorGraph graph)
            {
                Executed = true;
            }

            public bool Executed { get; set; }
        }

        public class TestViewToken : IViewToken
        {
            public BehaviorNode ToBehavioralNode()
            {
                throw new NotImplementedException();
            }

            public ObjectDef ToViewFactoryObjectDef()
            {
                throw new NotImplementedException();
            }

            public Type ViewType
            {
                get { throw new NotImplementedException(); }
            }

            public Type ViewModel
            {
                get { throw new NotImplementedException(); }
            }

            public string Name()
            {
                throw new NotImplementedException();
            }

            public string Namespace
            {
                get { throw new NotImplementedException(); }
            }

            public override bool Equals(object obj)
            {
                return obj.GetType() == GetType();
            }

            public bool Equals(TestViewToken other)
            {
                return !ReferenceEquals(null, other);
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        public class TestViewFacility : IViewFacility
        {
            public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
            {
                yield return new TestViewToken();
            }

            public BehaviorNode CreateViewNode(Type type) { return null; }
        }
    }
}