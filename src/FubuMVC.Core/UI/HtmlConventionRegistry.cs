using System;
using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.UI.Tags;

namespace FubuMVC.Core.UI
{
    public class HtmlConventionRegistry : TagProfileExpression
    {
        private readonly Cache<string, TagProfile> _profiles =
            new Cache<string, TagProfile>(name => new TagProfile(name));


        public HtmlConventionRegistry()
            : base(new TagProfile(TagProfile.DEFAULT))
        {
            _profiles[TagProfile.DEFAULT] = profile;
        }

        public IEnumerable<TagProfile> Profiles { get { return _profiles.GetAll(); } }

        public void Profile(string profileName, Action<TagProfileExpression> configure)
        {
            var expression = new TagProfileExpression(_profiles[profileName]);
            configure(expression);
        }
    }
}