using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;
using FubuMVC.Core.View.Model;
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
                theViews.Add(MockRepository.GenerateMock<ITemplateFile>());
            }

            theAction = ActionCall.For<GoController>(x => x.Go());
            theChain = new BehaviorChain();
            theChain.AddToEnd(theAction);

            thePolicy = new ViewAttachmentPolicy();
        }

        private List<IViewToken> theViews;
        private ActionCall theAction;
        private ViewAttachmentWorker theAttacher;
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

        private IViewProfile theProfileMatches<T>(params int[] indices) where T : IConditional, new()
        {
            IEnumerable<IViewToken> matching = views(indices);

            return thePolicy.Profile(new T(), x => matching.Contains(x), x => x.Name());
        }

        private void afterAttaching()
        {
            var graph = new BehaviorGraph();
            graph.AddChain(theChain);

            graph.Settings.Replace(thePolicy);

            var worker = new ViewAttachmentWorker(new ViewBag(theViews), thePolicy);
            worker.Configure(graph);
        }



        [Test]
        public void attach_a_view_for_a_matching_filter_and_policy_simple()
        {
            filterMatching(1);
            theProfileMatches<Conditional1>(1);

            afterAttaching();

            var node = theChain.Output.Media().Single(x => x.Writer is IViewWriter);
            node.Writer.As<IViewWriter>().View.ShouldBeTheSameAs(theViews[1]);
            node.Condition.ShouldBeOfType<Conditional1>();
        }

        [Test]
        public void attach_for_multiple_profiles_does_not_double_dip_for_views()
        {
            var profile = theProfileMatches<Conditional1>(1, 2, 3, 4, 5);
            filterMatching(1, 2, 3);
            filterMatching(1);
            filterMatching(4, 5, 6); // 6 should catch in the default profile

            afterAttaching();

            theChain.Output.HasView(profile.Condition).ShouldBeTrue();
            theChain.Output.HasView(Always.Flyweight).ShouldBeTrue();

            theChain.Output.ViewFor(profile.Condition)
                .ShouldBeTheSameAs(theViews[1]);

            theChain.Output.ViewFor(Always.Flyweight)
                .ShouldBeTheSameAs(theViews[6]);
        }

        [Test]
        public void attach_for_multiple_profiles_does_not_double_dip_for_views_2()
        {
            var profile1 = theProfileMatches<Conditional1>(1, 2, 3, 4, 5);
            var profile2 = theProfileMatches<Conditional2>(6, 7, 8);
            filterMatching(1, 2, 3);
            filterMatching(1);
            filterMatching(4, 5, 6); // 6 should catch in the second profile
            filterMatching(9, 10); // won't find a unique match for the default

            afterAttaching();

            theChain.Output.HasView(profile1.Condition).ShouldBeTrue();
            theChain.Output.HasView(profile2.Condition).ShouldBeTrue();
            theChain.Output.HasView(Always.Flyweight).ShouldBeFalse();

            theChain.Output.ViewFor(profile1.Condition)
                .ShouldBeTheSameAs(theViews[1]);

            theChain.Output.Media().Single(x => x.Condition == profile2.Condition).Writer
                .As<IViewWriter>().View
                .ShouldBeTheSameAs(theViews[6]);
        }

        [Test]
        public void attach_for_multiple_profiles_does_not_double_dip_for_views_3()
        {
            var profile1 = theProfileMatches<Conditional1>(1, 2, 3, 4, 5);
            var profile2 = theProfileMatches<Conditional2>(6, 7, 8);
            filterMatching(1, 2, 3);
            filterMatching(1);
            filterMatching(4, 5, 6); // 6 should catch in the second profile
            filterMatching(10); // 10 will be the unique on default

            afterAttaching();

            theChain.Output.HasView(profile1.Condition).ShouldBeTrue();
            theChain.Output.HasView(profile2.Condition).ShouldBeTrue();
            theChain.Output.HasView(Always.Flyweight).ShouldBeTrue();


            theChain.Output.ViewFor(profile1.Condition).ShouldBeTheSameAs(theViews[1]);

            theChain.Output.Media().Single(x => x.Condition == profile2.Condition).Writer
                .As<IViewWriter>().View
                .ShouldBeTheSameAs(theViews[6]);

            theChain.Output.ViewFor(Always.Flyweight)
                .ShouldBeTheSameAs(theViews[10]);
        }

        [Test]
        public void attach_for_no_profiles_and_a_unique_match()
        {
            filterMatching(1);
            afterAttaching();

            theChain.Output.ViewFor(Always.Flyweight).ShouldBeTheSameAs(theViews[1]);
        }

        [Test]
        public void cannot_match_if_a_filter_returns_multiple_views_because_it_has_to_be_unique()
        {
            filterMatching(1, 2, 3, 4);

            afterAttaching();

            theChain.Output.Writes(MimeType.Html).ShouldBeFalse();
        }


        [Test]
        public void do_not_attach_anything_if_a_view_for_that_condition_already_exists()
        {
            var existingView = MockRepository.GenerateMock<ITemplateFile>();
            existingView.Stub(x => x.ViewModel).Return(typeof (GoModel));

            var profile = theProfileMatches<Conditional1>(1);

            theChain.Output.AddView(existingView, profile.Condition);

            theChain.Output.Media().Where(x => x.Mimetypes.Contains(MimeType.Html.Value)).Count().ShouldEqual(1);

            // There is a set of filters where it would match, see the test above
            filterMatching(1);
            

            afterAttaching();

            theChain.Output.Media().Where(x => x.Mimetypes.Contains(MimeType.Html.Value)).Count().ShouldEqual(1);
        }

        [Test]
        public void no_profiles_multiple_filters_no_unique_match()
        {
            filterMatching(1, 2, 3);
            filterMatching(1, 2);
            filterMatching(2, 3);

            afterAttaching();

            theChain.Output.Media().Any(x => x.Mimetypes.Contains(MimeType.Html.Value)).ShouldBeFalse();
        }

        [Test]
        public void no_profiles_multiple_filters_to_get_a_match()
        {
            filterMatching(1, 2, 3);
            filterMatching(1, 2);
            filterMatching(2);

            afterAttaching();

            theChain.Output.ViewFor(Always.Flyweight)
                .ShouldBeTheSameAs(theViews[2]);
        }

        [Test]
        public void stops_on_the_first_unique_match()
        {
            filterMatching(1, 2, 3);
            filterMatching(1, 2);
            filterMatching(2);
            filterMatching(4);

            afterAttaching();


            theChain.Output.ViewFor(Always.Flyweight)
                .ShouldBeTheSameAs(theViews[2]);
        }
        
    }

    public class Conditional1 : IConditional
    {
        public bool ShouldExecute(IFubuRequestContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class Conditional2 : IConditional
    {
        public bool ShouldExecute(IFubuRequestContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class Conditional3 : IConditional
    {
        public bool ShouldExecute(IFubuRequestContext context)
        {
            throw new NotImplementedException();
        }
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