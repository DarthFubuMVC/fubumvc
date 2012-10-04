using System;
using System.Linq;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core.View.Activation
{
    public class PageActivationExpression
    {
        private readonly Func<IViewToken, bool> _filter;
        private readonly FubuRegistry _registry;

        public PageActivationExpression(FubuRegistry registry, Func<IViewToken, bool> filter)
        {
            _registry = registry;
            _filter = filter;
        }

        /// <summary>
        /// Sets the profile name for Html conventions
        /// </summary>
        /// <param name="profileName"></param>
        public void SetTagProfileTo(string profileName)
        {
            _registry.AlterSettings<ViewEngines>(x => {
                var description = "Profile = " + profileName;
                x.AddPolicy(new ViewTokenPolicy(_filter, token => token.ProfileName = profileName, description));
            });
        }
    }
}