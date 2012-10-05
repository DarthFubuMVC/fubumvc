using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Core.View.Testing
{
    [TestFixture]
    public class ViewEnginesTester
    {
        private ViewEngines _runner;

        [SetUp]
        public void Setup()
        {
            var types = new TypePool(null);
            _runner = new ViewEngines();
        }

        [Test]
        public void should_not_add_same_type_of_facility_more_than_once()
        {
            _runner.AddFacility(new TestViewFacility());
            _runner.AddFacility(new TestViewFacility());

            _runner.Facilities.ShouldHaveCount(1);
        }


        public class TestViewToken : IViewToken
        {
            public ObjectDef ToViewFactoryObjectDef()
            {
                throw new NotImplementedException();
            }

            public string ProfileName { get; set; }

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
            public IEnumerable<IViewToken> FindViews(BehaviorGraph graph)
            {
                yield return new TestViewToken();
            }

            public BehaviorNode CreateViewNode(Type type) { return null; }
        }
    }
}