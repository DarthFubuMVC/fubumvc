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
        private readonly IList<IViewProfile> _profiles = new List<IViewProfile>();


        public IEnumerable<ProfileViewBag> Profiles(ViewBag views)
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

    public class ProfileViewBag 
    {

        public ProfileViewBag(IViewProfile profile, ViewBag views)
        {
            Condition = profile.Condition;
            Views = profile.Filter(views);
        }

        public ViewBag Views { get; private set; }
        public IConditional Condition { get; private set; }

    }
}