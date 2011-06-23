using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
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

        public class TestViewFacility : IViewFacility
        {
            public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
            {
                return Enumerable.Empty<IViewToken>();
            }

            public BehaviorNode CreateViewNode(Type type) { return null; }
        }
    }
}