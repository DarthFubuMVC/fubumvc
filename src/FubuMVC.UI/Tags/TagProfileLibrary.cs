using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Util;

namespace FubuMVC.UI.Tags
{
    public class TagProfileLibrary
    {
        private readonly Cache<string, TagProfile> _profiles = new Cache<string, TagProfile>(key => new TagProfile(key));

        public TagProfile DefaultProfile { get { return _profiles[TagProfile.DEFAULT]; } }

        public TagProfile this[string name] { get { return _profiles[name]; } }

        public void ImportProfile(TagProfile profile)
        {
            _profiles[profile.Name].Import(profile);
        }

        public void ImportRegistry(HtmlConventionRegistry registry)
        {
            registry.Profiles.Each(ImportProfile);
        }

        public void Seal()
        {
            TagProfile defaults = DefaultProfile;
            _profiles.Each(p =>
            {
                if (p == defaults) return;

                p.Import(defaults);
            });
        }
    }
}