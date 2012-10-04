using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.View.Attachment
{
    [TestFixture]
    public class ViewAttacherTester
    {
        [SetUp]
        public void SetUp()
        {
            theViews = new List<IViewToken>();
            for (int i = 0; i < 15; i++)
            {
                theViews.Add(MockRepository.GenerateMock<IViewToken>());
            }

            theAction = ActionCall.For<GoController>(x => x.Go());
            theChain = new BehaviorChain();
            theChain.AddToEnd(theAction);

            thePolicy = new ViewAttachmentPolicy();
            theAttacher = new ViewAttacher();
        }

        private List<IViewToken> theViews;
        private ActionCall theAction;
        private ViewAttacher theAttacher;
        private BehaviorChain theChain;
        private ViewAttachmentPolicy thePolicy;

        private IEnumerable<IViewToken> views(params int[] indices)
        {
            return indices.Select(x => theViews[x]).ToList();
        }

        private IViewsForActionFilter filterMatching(params int[] indices)
        {
            var filter = new StubViewsForActionFilter(views(indices));
            thePolicy.AddFilter(filter);

            return filter;
        }

        private IViewProfile theProfileMatches<T>(params int[] indices) where T : IConditional
        {
            IEnumerable<IViewToken> matching = views(indices);

            return thePolicy.Profile(typeof (T), x => matching.Contains(x), x => x.Name());
        }

        private void afterAttaching()
        {
            var graph = new BehaviorGraph();
            graph.AddChain(theChain);

            graph.Settings.Replace(new ViewEngines(theViews));
            graph.Settings.Replace(thePolicy);

            theAttacher.Configure(graph);
        }


        [Test]
        public void attach_a_view_for_a_matching_filter_and_policy_simple()
        {
            filterMatching(1);
            theProfileMatches<Conditional1>(1);

            afterAttaching();

            var node = theChain.Output.Writers.Single().ShouldBeOfType<ViewNode>();
            node.View.ShouldBeTheSameAs(theViews[1]);
            node.ConditionType.ShouldEqual(typeof (Conditional1));
        }

        [Test]
        public void attach_for_multiple_profiles_does_not_double_dip_for_views()
        {
            theProfileMatches<Conditional1>(1, 2, 3, 4, 5);
            filterMatching(1, 2, 3);
            filterMatching(1);
            filterMatching(4, 5, 6); // 6 should catch in the default profile

            afterAttaching();

            theChain.Output.HasView(typeof (Conditional1)).ShouldBeTrue();
            theChain.Output.HasView(typeof (Always)).ShouldBeTrue();

            theChain.Output.Writers.OfType<ViewNode>().Single(x => x.ConditionType == typeof (Conditional1)).View
                .ShouldBeTheSameAs(theViews[1]);

            theChain.Output.Writers.OfType<ViewNode>().Single(x => x.ConditionType == typeof (Always)).View
                .ShouldBeTheSameAs(theViews[6]);
        }

        [Test]
        public void attach_for_multiple_profiles_does_not_double_dip_for_views_2()
        {
            theProfileMatches<Conditional1>(1, 2, 3, 4, 5);
            theProfileMatches<Conditional2>(6, 7, 8);
            filterMatching(1, 2, 3);
            filterMatching(1);
            filterMatching(4, 5, 6); // 6 should catch in the second profile
            filterMatching(9, 10); // won't find a unique match for the default

            afterAttaching();

            theChain.Output.HasView(typeof (Conditional1)).ShouldBeTrue();
            theChain.Output.HasView(typeof (Conditional2)).ShouldBeTrue();
            theChain.Output.HasView(typeof (Always)).ShouldBeFalse();

            theChain.Output.Writers.OfType<ViewNode>().Single(x => x.ConditionType == typeof (Conditional1)).View
                .ShouldBeTheSameAs(theViews[1]);

            theChain.Output.Writers.OfType<ViewNode>().Single(x => x.ConditionType == typeof (Conditional2)).View
                .ShouldBeTheSameAs(theViews[6]);
        }

        [Test]
        public void attach_for_multiple_profiles_does_not_double_dip_for_views_3()
        {
            theProfileMatches<Conditional1>(1, 2, 3, 4, 5);
            theProfileMatches<Conditional2>(6, 7, 8);
            filterMatching(1, 2, 3);
            filterMatching(1);
            filterMatching(4, 5, 6); // 6 should catch in the second profile
            filterMatching(10); // 10 will be the unique on default

            afterAttaching();

            theChain.Output.HasView(typeof (Conditional1)).ShouldBeTrue();
            theChain.Output.HasView(typeof (Conditional2)).ShouldBeTrue();
            theChain.Output.HasView(typeof (Always)).ShouldBeTrue();

            theChain.Output.Writers.OfType<ViewNode>().Single(x => x.ConditionType == typeof (Conditional1)).View
                .ShouldBeTheSameAs(theViews[1]);

            theChain.Output.Writers.OfType<ViewNode>().Single(x => x.ConditionType == typeof (Conditional2)).View
                .ShouldBeTheSameAs(theViews[6]);

            theChain.Output.Writers.OfType<ViewNode>().Single(x => x.ConditionType == typeof (Always)).View
                .ShouldBeTheSameAs(theViews[10]);
        }

        [Test]
        public void attach_for_no_profiles_and_a_unique_match()
        {
            filterMatching(1);
            afterAttaching();

            var node = theChain.Output.Writers.Single().ShouldBeOfType<ViewNode>();
            node.View.ShouldBeTheSameAs(theViews[1]);
            node.ConditionType.ShouldEqual(typeof (Always));
        }

        [Test]
        public void cannot_match_if_a_filter_returns_multiple_views_because_it_has_to_be_unique()
        {
            filterMatching(1, 2, 3, 4);

            afterAttaching();

            theChain.Output.Writers.Any().ShouldBeFalse();
        }

        [Test]
        public void create_the_log_files_for_default_only_matching()
        {
            IViewsForActionFilter filter1 = filterMatching(1, 2, 3);
            IViewsForActionFilter filter2 = filterMatching(1, 2);
            IViewsForActionFilter filter3 = filterMatching(2);

            afterAttaching();

            // going to create one log per profile
            var log = theAction.As<ITracedModel>().StagedEvents.OfType<ViewAttachmentLog>().Single();
            log.Profile.ConditionType.ShouldEqual(typeof (Always));
            log.Profile.ShouldBeOfType<DefaultProfile>();

            log.Logs.ElementAt(0).Filter.ShouldBeTheSameAs(filter1);
            log.Logs.ElementAt(0).Views.ShouldHaveTheSameElementsAs(views(1, 2, 3));

            log.Logs.ElementAt(1).Filter.ShouldBeTheSameAs(filter2);
            log.Logs.ElementAt(1).Views.ShouldHaveTheSameElementsAs(views(1, 2));

            log.Logs.ElementAt(2).Filter.ShouldBeTheSameAs(filter3);
            log.Logs.ElementAt(2).Views.ShouldHaveTheSameElementsAs(views(2));
        }

        [Test]
        public void creating_the_logs_for_multiple_profiles_and_filters()
        {
            theProfileMatches<Conditional1>(1, 2, 3, 4, 5);
            IViewsForActionFilter f1 = filterMatching(1, 2, 3);
            IViewsForActionFilter f2 = filterMatching(1);
            IViewsForActionFilter f3 = filterMatching(4, 5, 6); // 6 should catch in the default profile

            afterAttaching();

            var log1 = theAction.As<ITracedModel>().StagedEvents.OfType<ViewAttachmentLog>().ElementAt(0);
            log1.Profile.ConditionType.ShouldEqual(typeof (Conditional1));
            log1.Logs.Count().ShouldEqual(2);

            log1.Logs.ElementAt(0).Filter.ShouldBeTheSameAs(f1);
            log1.Logs.ElementAt(0).Views.ShouldHaveTheSameElementsAs(views(1, 2, 3));

            log1.Logs.ElementAt(1).Filter.ShouldBeTheSameAs(f2);
            log1.Logs.ElementAt(1).Views.ShouldHaveTheSameElementsAs(views(1));


            var log2 = theAction.As<ITracedModel>().StagedEvents.OfType<ViewAttachmentLog>().ElementAt(1);
            log2.Profile.ConditionType.ShouldEqual(typeof (Always));

            log2.Logs.Count().ShouldEqual(1);
            log2.Logs.Single().Filter.ShouldBeTheSameAs(f3);
            log2.Logs.Single().Views.ShouldHaveTheSameElementsAs(views(6));
        }

        [Test]
        public void do_not_attach_anything_if_a_view_for_that_condition_already_exists()
        {
            var existingView = MockRepository.GenerateMock<IViewToken>();
            existingView.Stub(x => x.ViewModel).Return(typeof (GoModel));

            theChain.Output.AddView(existingView, typeof (Conditional1));

            theChain.Output.Writers.Count().ShouldEqual(1);

            // There is a set of filters where it would match, see the test above
            filterMatching(1);
            theProfileMatches<Conditional1>(1);

            afterAttaching();

            theChain.Output.Writers.Count().ShouldEqual(1);
        }

        [Test]
        public void no_profiles_multiple_filters_no_unique_match()
        {
            filterMatching(1, 2, 3);
            filterMatching(1, 2);
            filterMatching(2, 3);

            afterAttaching();

            theChain.Output.Writers.Any().ShouldBeFalse();
        }

        [Test]
        public void no_profiles_multiple_filters_to_get_a_match()
        {
            filterMatching(1, 2, 3);
            filterMatching(1, 2);
            filterMatching(2);

            afterAttaching();


            var node = theChain.Output.Writers.Single().ShouldBeOfType<ViewNode>();
            node.View.ShouldBeTheSameAs(theViews[2]);
        }

        [Test]
        public void stops_on_the_first_unique_match()
        {
            filterMatching(1, 2, 3);
            filterMatching(1, 2);
            filterMatching(2);
            filterMatching(4);

            afterAttaching();


            var node = theChain.Output.Writers.Single().ShouldBeOfType<ViewNode>();
            node.View.ShouldBeTheSameAs(theViews[2]);
        }
    }

    public class Conditional1 : IConditional
    {
        #region IConditional Members

        public bool ShouldExecute()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class Conditional2 : IConditional
    {
        #region IConditional Members

        public bool ShouldExecute()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class Conditional3 : IConditional
    {
        #region IConditional Members

        public bool ShouldExecute()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class StubViewsForActionFilter : IViewsForActionFilter
    {
        private readonly IEnumerable<IViewToken> _matching;

        public StubViewsForActionFilter(IEnumerable<IViewToken> matching)
        {
            _matching = matching;
        }

        #region IViewsForActionFilter Members

        public IEnumerable<IViewToken> Apply(ActionCall call, ViewBag views)
        {
            return views.Views.Where(x => _matching.Contains(x.Resolve())).ToList();
        }

        #endregion
    }

    public class GoController
    {
        public GoModel Go()
        {
            return new GoModel();
        }
    }

    public class GoModel
    {
    }
}