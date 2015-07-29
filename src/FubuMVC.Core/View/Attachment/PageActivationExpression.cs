using System;

namespace FubuMVC.Core.View.Attachment
{
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