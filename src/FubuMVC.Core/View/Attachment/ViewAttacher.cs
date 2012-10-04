using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.View.Attachment
{
    public class ViewAttachmentPolicy
    {
        private readonly IList<Func<IViewToken, bool>> _defaultExcludes = new List<Func<IViewToken, bool>>();
        private readonly IList<IViewsForActionFilter> _filters = new List<IViewsForActionFilter>();
        private readonly IList<IViewProfile> _profiles = new List<IViewProfile>();

        private static IEnumerable<IViewsForActionFilter> defaultFilters()
        {
            yield return new ActionWithSameNameAndFolderAsViewReturnsViewModelType();
            yield return new ActionInSameFolderAsViewReturnsViewModelType();
            yield return new ActionReturnsViewModelType();
        }

        public IEnumerable<IViewsForActionFilter> Filters()
        {
            return _filters.Any() ? _filters : defaultFilters().ToArray();
        }

        public IEnumerable<IViewsForActionFilter> ActiveFilters
        {
            get
            {
                return Filters();
            }
        }

        public IEnumerable<ProfileViewBag> Profiles(BehaviorGraph graph)
        {
            if (_profiles.Any())
            {
                foreach (var profile in _profiles)
                {
                    yield return new ProfileViewBag(profile, graph);
                }

                Func<IViewToken, bool> defaultFilter = x => !_defaultExcludes.Any(test => test(x));
                var defaultProfile = new ViewProfile(typeof(Always), defaultFilter, x => x.Name());

                yield return new ProfileViewBag(defaultProfile, graph);
            }
            else
            {
                yield return new ProfileViewBag(new DefaultProfile(), graph);
            }
        }

        public IViewProfile AddProfile(Type conditionType, Func<IViewToken, bool> filter,
                                       Func<IViewToken, string> nameCorrection)
        {
            _defaultExcludes.Add(filter);
            var profile = new ViewProfile(conditionType, filter, nameCorrection);
            _profiles.Add(profile);

            return profile;
        }

        public void AddFilter(IViewsForActionFilter filter)
        {
            _filters.Add(filter);
        }

        public class ProfileViewBag
        {
            public ProfileViewBag(IViewProfile profile, BehaviorGraph graph)
            {
                Profile = profile;
                Views = profile.Filter(graph.Settings.Get<ViewBag>());
            }

            public ViewBag Views { get; private set; }
            public IViewProfile Profile { get; private set; }
        }

    }

    [Policy]
    public class ViewAttacher : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var policy = graph.Settings.Get<ViewAttachmentPolicy>();

            FindLastActions(graph).Where(x => x.OutputType() != null).Each(action => {
                policy.Profiles(graph).Each(x => { Attach(policy, x.Profile, x.Views, action); });
            });
        }

        public virtual void Attach(ViewAttachmentPolicy policy, IViewProfile viewProfile, ViewBag bag, ActionCall action)
        {
            // No duplicate views!
            var outputNode = action.ParentChain().Output;
            if (outputNode.HasView(viewProfile.ConditionType)) return;

            var log = new ViewAttachmentLog(viewProfile);
            action.Trace(log);

            foreach (var filter in policy.Filters())
            {
                var viewTokens = filter.Apply(action, bag);
                var count = viewTokens.Count();

                if (count > 0)
                {
                    log.FoundViews(filter, viewTokens.Select(x => x.Resolve()));
                }

                if (count != 1) continue;

                var token = viewTokens.Single().Resolve();
                outputNode.AddView(token, viewProfile.ConditionType);

                break;
            }
        }



        public static IEnumerable<ActionCall> FindLastActions(BehaviorGraph graph)
        {
            foreach (var chain in graph.Behaviors)
            {
                var last = chain.Calls.LastOrDefault();
                if (last != null)
                {
                    yield return last;
                }
            }
        }


    }
}