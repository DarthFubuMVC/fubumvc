using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.DSL
{
    [TestFixture]
    public class ViewExpressionTester
    {
        private FubuRegistry _parent;
        private FubuRegistry _child;

        private Test1Facility _facility1;
        private Test2Facility _facility2;

        [SetUp]
        void before_each()
        {
            _parent = new FubuRegistry();
            _child = new FubuRegistry();

            _facility1 = new Test1Facility();
            _facility2 = new Test2Facility();

            _parent.Views.Facility(_facility1);

            _parent.Views.TryToAttachWithDefaultConventions();
            _parent.Views.TryToAttachViewsInPackages();
        }

        [Test]
        public void try_to_attach_views_from_packages_exports_facility_to_imports()
        {
            _parent.Import(_child, "import");
            _parent.BuildGraph();

            _facility1.Invocations.ShouldEqual(2);
        }

        [Test]
        public void try_to_attach_views_from_packages_does_not_replace_existing_facilities()
        {
            var childFacility = new Test1Facility();
            _child.Views.Facility(childFacility);

            _parent.Import(_child, "import");
            _parent.BuildGraph();

            _facility1.Invocations.ShouldEqual(1);
            childFacility.Invocations.ShouldEqual(1);
        }

        [Test]
        public void try_to_attach_views_from_packages_is_additive()
        {
            var childFacility = new Test2Facility();
            _child.Views.Facility(childFacility);

            _parent.Import(_child, "import");
            _parent.BuildGraph();

            _facility1.Invocations.ShouldEqual(2);
            childFacility.Invocations.ShouldEqual(1);
        }

        // UT : More coverage on filter 
    }


    public class Test1Facility : IViewFacility
    {
        public int Invocations { get; private set; }
        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            Invocations++;
            return Enumerable.Empty<IViewToken>();
        }
        public BehaviorNode CreateViewNode(Type type) { return null; }
    }

    public class Test2Facility : IViewFacility
    {
        public int Invocations { get; private set; }
        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            Invocations++;
            return Enumerable.Empty<IViewToken>();
        }
        public BehaviorNode CreateViewNode(Type type) { return null; }
    }
}
