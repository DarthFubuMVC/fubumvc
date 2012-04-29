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
    public class ViewEngineRegistryTester
    {
        private ViewEngineRegistry _runner;

        [SetUp]
        public void Setup()
        {
            var types = new TypePool(null);
            _runner = new ViewEngineRegistry();
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
            public IEnumerable<IViewToken> FindViews()
            {
                yield return new TestViewToken();
            }

            public BehaviorNode CreateViewNode(Type type) { return null; }
        }
    }
}