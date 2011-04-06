using System;
using System.Collections.Generic;
using FubuFastPack.JqGrid;
using FubuFastPack.Querying;
using FubuFastPack.Testing.Security;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
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

        public SortOrder SortOrder()
        {
            throw new NotImplementedException();
        }
    }

    public class LookupRequest : NamedGridRequest
    {
        public override string ToString()
        {
            return "Lookup:  " + GridName;
        }
    }

    [TestFixture]
    public class when_getting_the_url_for_a_grid : InteractionContext<SmartGridService>
    {
        private object[] theArgs;
        private string theResultingUrl;
        private StubUrlRegistry theUrls;
        private string theQuerystringFromTheHarness = "?Something=Else";

        protected override void beforeEach()
        {
            MockFor<IServiceLocator>().Stub(x => x.GetInstance<ISmartGridHarness>(typeof (TheTestGrid).NameForGrid()))
                .Return(MockFor<ISmartGridHarness>());

            MockFor<ISmartGridHarness>().Stub(x => x.GetQuerystring()).Return(theQuerystringFromTheHarness);

            theUrls = new StubUrlRegistry();
            Services.Inject<IUrlRegistry>(theUrls);

            theArgs = new object[]{"a", "b"};
            theResultingUrl = ClassUnderTest.GetUrl<LookupRequest, TheTestGrid>(theArgs);
        }

        [Test]
        public void should_register_the_arguments_with_the_underlying_harness()
        {
            MockFor<ISmartGridHarness>().AssertWasCalled(x => x.RegisterArguments(theArgs));
        }

        [Test]
        public void the_url_includes_the_query_string_from_the_smartgridharness()
        {
            theResultingUrl.ShouldEndWith(theQuerystringFromTheHarness);
        }

        [Test]
        public void the_url_should_start_with_the_url_for_the_grid_name_and_model()
        {
            var expected = theUrls.UrlFor(new LookupRequest(){
                GridName = "TheTest"
            });

            theResultingUrl.ShouldStartWith(expected);
        }
    }

    [TestFixture]
    public class when_getting_the_grid_state_for_a_grid : InteractionContext<SmartGridService>
    {
        private const int theCountFromTheGrid = 123;
        private Case theCase;
        private GridState theGridState;
        private string theQuerystringFromTheHarness;

        protected override void beforeEach()
        {
            MockFor<IServiceLocator>().Stub(x => x.GetInstance<ISmartGridHarness>(typeof (TheTestGrid).NameForGrid()))
                .Return(MockFor<ISmartGridHarness>());

            MockFor<ISmartGridHarness>().Stub(x => x.Count()).Return(theCountFromTheGrid);
            MockFor<ISmartGridHarness>().Stub(x => x.HeaderText()).Return("the header");
            theQuerystringFromTheHarness = "?Case=someguid";
            MockFor<ISmartGridHarness>().Stub(x => x.GetQuerystring()).Return(theQuerystringFromTheHarness);

            Services.Inject<IUrlRegistry>(new StubUrlRegistry());

            theCase = new Case();

            theGridState = ClassUnderTest.StateForGrid<TheTestGrid>(theCase);
        }

        [Test]
        public void should_set_the_arguments_of_the_grid()
        {
            MockFor<ISmartGridHarness>().AssertWasCalled(x => x.RegisterArguments(theCase));
        }

        [Test]
        public void should_set_the_grid_id()
        {
            theGridState.GridId.ShouldEqual(typeof (TheTestGrid).ContainerNameForGrid());
        }

        [Test]
        public void should_set_the_grid_container_id()
        {
            theGridState.ContainerId.ShouldEqual("TheTest-container");
        }

        [Test]
        public void should_set_the_label_id()
        {
            theGridState.LabelId.ShouldEqual(typeof (TheTestGrid).IdForLabel());
        }

        [Test]
        public void should_have_the_row_count()
        {
            theGridState.Count.ShouldEqual(theCountFromTheGrid);
        }

        [Test]
        public void header_text_should_be_from_the_grid()
        {
            theGridState.HeaderText.ShouldEqual("the header");
        }

        [Test]
        public void url_should_be_the_url_for_grid_request_plus_the_querystring_from_the_harness()
        {
            var url = new StubUrlRegistry().UrlFor(new GridRequest<TheTestGrid>()) + theQuerystringFromTheHarness;
            theGridState.Url.ShouldEqual(url);
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