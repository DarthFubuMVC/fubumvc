using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.View.Attachment
{
    /// <summary>
    /// Settings that governs how the view attachment will attempt to match views to actions in a 
    /// FubuMVC application.  It is no longer necessary to explicitly specify the attachment policy
    /// in your application if you just want to use the default matching
    /// 
    /// </summary>
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
            get { return Filters(); }
        }

        internal IEnumerable<ProfileViewBag> Profiles(ViewBag views)
        {
            if (_profiles.Any())
            {
                foreach (var profile in _profiles)
                {
                    yield return new ProfileViewBag(profile, views);
                }

                Func<IViewToken, bool> defaultFilter = x => !_defaultExcludes.Any(test => test(x));
                var defaultProfile = new ViewProfile(Always.Flyweight, defaultFilter, x => x.Name());

                yield return new ProfileViewBag(defaultProfile, views);
            }
            else
            {
                yield return new ProfileViewBag(new DefaultProfile(), views);
            }
        }

        /// <summary>
        /// Create an attachment profile based on a runtime condition.  The original intent of view profiles
        /// was to enable multiple views per action based on the detected device of the user (desktop, tablet, smartphone),
        /// but is not limited to that functionality
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="filter"></param>
        /// <param name="nameCorrection"></param>
        /// <returns></returns>
        public IViewProfile Profile(IConditional condition, Func<IViewToken, bool> filter,
            Func<IViewToken, string> nameCorrection)
        {
            _defaultExcludes.Add(filter);
            var profile = new ViewProfile(condition, filter, nameCorrection);
            _profiles.Add(profile);

            return profile;
        }

        /// <summary>
        /// Add a new strategy for attaching views to actions
        /// </summary>
        /// <param name="filter"></param>
        public void AddFilter(IViewsForActionFilter filter)
        {
            _filters.Add(filter);
        }

        public class ProfileViewBag
        {
            public ProfileViewBag(IViewProfile profile, ViewBag views)
            {
                Profile = profile;
                Views = profile.Filter(views);
            }

            public ViewBag Views { get; private set; }
            public IViewProfile Profile { get; private set; }
        }

        /// <summary>
        /// Explicitly define your attachment policy strategies in order
        /// </summary>
        /// <param name="configure"></param>
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
        public void Profile<T>(string prefix) where T : IConditional, new()
        {
            Func<IViewToken, string> naming = view => {
                var name = view.Name();
                return name.Substring(prefix.Length);
            };

            Profile(new T(), x => x.Name().StartsWith(prefix), naming);
        }
    }

    public class PageActivationExpression
    {
        private readonly ViewEngineSettings _parent;
        private readonly Func<IViewToken, bool> _filter;

        public PageActivationExpression(ViewEngineSettings parent, Func<IViewToken, bool> filter)
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