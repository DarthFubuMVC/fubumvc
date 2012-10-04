using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
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

        public IViewProfile Profile(Type conditionType, Func<IViewToken, bool> filter,
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
                Views = profile.Filter(graph.Settings.Get<ViewEngines>().Views);
            }

            public ViewBag Views { get; private set; }
            public IViewProfile Profile { get; private set; }
        }

        public void TryToAttach(Action<ViewsForActionFilterExpression> configure)
        {
            var expression = new ViewsForActionFilterExpression(this);
            configure(expression);
        }

        public class ViewsForActionFilterExpression
        {
            private readonly ViewAttachmentPolicy _policy;

            public ViewsForActionFilterExpression(ViewAttachmentPolicy policy)
            {
                _policy = policy;
            }

            /// <summary>
            /// views are matched to actions based on same namespace and the Action's underlying method name
            /// </summary>
            public void by_ViewModel_and_Namespace_and_MethodName()
            {
                @by<ActionWithSameNameAndFolderAsViewReturnsViewModelType>();
            }

            /// <summary>
            /// views are matched to Actions based on the view model (Action's output model -> view's ViewModel)
            /// and same namespace
            /// </summary>
            public void by_ViewModel_and_Namespace()
            {
                @by<ActionInSameFolderAsViewReturnsViewModelType>();
            }

            /// <summary>
            /// views are matched to Actions solely based on the view model (Action's output model -> view's ViewModel)
            /// </summary>
            public void by_ViewModel()
            {
                @by<ActionReturnsViewModelType>();
            }

            /// <summary>
            /// Specify your custom strategy to find attach views to Actions.
            /// </summary>
            public void @by<TFilter>() where TFilter : IViewsForActionFilter, new()
            {
                @by(new TFilter());
            }

            /// <summary>
            /// Specify your custom strategy to find attach views to Actions.
            /// </summary>
            public void @by(IViewsForActionFilter strategy)
            {
                _policy.AddFilter(strategy);
            }
        }

        /// <summary>
        ///   This creates a view profile for the view attachment.  Used for scenarios like
        ///   attaching multiple views to the same chain for different devices.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "prefix"></param>
        /// <example>
        ///   Profile<IsMobile>("m.") -- where "m" would mean look for views that are named "m.something"
        /// </example>
        /// <returns></returns>
        public void Profile<T>(string prefix) where T : IConditional
        {
            Func<IViewToken, string> naming = view =>
            {
                string name = view.Name();
                return name.Substring(prefix.Length);
            };

            Profile(typeof(T), x => x.Name().StartsWith(prefix), naming);
        }
    }

    public class PageActivationExpression
    {
        private readonly ViewEngines _parent;
        private readonly Func<IViewToken, bool> _filter;

        public PageActivationExpression(ViewEngines parent, Func<IViewToken, bool> filter)
        {
            _parent = parent;
            _filter = filter;
        }

        /// <summary>
        /// Sets the profile name for Html conventions
        /// </summary>
        /// <param name="profileName"></param>
        public void SetTagProfileTo(string profileName)
        {
            var description = "Profile = " + profileName;
            _parent.AddPolicy(new ViewTokenPolicy(_filter, token => token.ProfileName = profileName, description));
        }
    }

}