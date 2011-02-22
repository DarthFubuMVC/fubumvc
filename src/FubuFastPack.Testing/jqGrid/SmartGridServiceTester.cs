using System;
using System.Collections.Generic;
using FubuFastPack.JqGrid;
using FubuFastPack.Querying;
using FubuMVC.Core.Urls;
using FubuMVC.Tests;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuFastPack.Testing.jqGrid
{
    public class TheTestGrid : ISmartGrid
    {
        public IGridDefinition Definition
        {
            get { throw new NotImplementedException(); }
        }

        public GridResults Invoke(IServiceLocator services, GridDataRequest request)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FilteredProperty> AllFilteredProperties(IQueryService queryService)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDictionary<string, object>> ToColumnModel()
        {
            throw new NotImplementedException();
        }

        public int Count(IServiceLocator services)
        {
            throw new NotImplementedException();
        }

        public string GetHeader()
        {
            throw new NotImplementedException();
        }

        public void DisableLinks()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Criteria> InitialCriteria()
        {
            throw new NotImplementedException();
        }

        public Type EntityType
        {
            get { throw new NotImplementedException(); }
        }

        public void ApplyPolicies(IEnumerable<IGridPolicy> policies)
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class when_getting_the_querystring_for_a_grid : InteractionContext<SmartGridService>
    {
        private readonly string theGridName = "name of the grid";
        private readonly object[] theArgs = new object[]{"a", "b"};
        private string theResultingUrl;

        protected override void beforeEach()
        {
            MockFor<IServiceLocator>().Stub(x => x.GetInstance<ISmartGridHarness>(theGridName))
                .Return(MockFor<ISmartGridHarness>());

            MockFor<ISmartGridHarness>().Stub(x => x.GetQuerystring()).Return("the url");

            theResultingUrl = ClassUnderTest.QuerystringFor(theGridName, theArgs);
        }

        [Test]
        public void should_set_the_arguments_on_the_harness()
        {
            MockFor<ISmartGridHarness>().AssertWasCalled(x => x.RegisterArguments(theArgs));
        }

        [Test]
        public void should_get_the_resulting_url()
        {
            theResultingUrl.ShouldEqual("the url");
        }
    }

    [TestFixture]
    public class when_getting_the_counts_for_a_grid : InteractionContext<SmartGridService>
    {
        private NamedGridRequest theRequest;
        private object[] theArgs;
        private string theQuerystring;
        private GridCounts theCounts;

        protected override void beforeEach()
        {
            theArgs = new object[0];
            theQuerystring = "?something=other";

            theRequest = new NamedGridRequest(){
                GridName = "TheTestGrid"
            };

            MockFor<IServiceLocator>().Stub(x => x.GetInstance<ISmartGridHarness>(theRequest.GridName))
                .Return(MockFor<ISmartGridHarness>());

            MockFor<ISmartGridHarness>().Stub(x => x.Count()).Return(123);
            
            Services.Inject<IUrlRegistry>(new StubUrlRegistry());

            MockFor<ISmartGridHarness>().Stub(x => x.GridType).Return(typeof (TheTestGrid));
            MockFor<ISmartGridHarness>().Stub(x => x.GetQuerystring()).Return(theQuerystring);
            MockFor<ISmartGridHarness>().Stub(x => x.HeaderText()).Return("the header");

            theCounts = ClassUnderTest.GetCounts<NamedGridRequest>(theRequest.GridName, theArgs);
        }

        [Test]
        public void has_the_actual_count()
        {
            theCounts.Count.ShouldEqual(123);
        }

        [Test]
        public void has_the_header_text()
        {
            theCounts.HeaderText.ShouldEqual("the header");
        }

        [Test]
        public void should_have_the_url_with_the_querystring()
        {
            var url = new StubUrlRegistry().UrlFor(new NamedGridRequest(){
                GridName = theRequest.GridName
            });
            url += theQuerystring;

            theCounts.Url.ShouldEqual(url);
        }
    }
}