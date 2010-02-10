using System;
using System.Linq;
using System.Text;
using FubuMVC.Core.Models;
using FubuMVC.Core.Runtime;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.Configuration
{
    public class AppSettingsProvider : ISettingsProvider
    {
        private readonly IObjectResolver _resolver;
        private readonly IServiceLocator _locator;

        public AppSettingsProvider(IObjectResolver resolver, IServiceLocator locator)
        {
            _resolver = resolver;
            _locator = locator;
        }

        public T SettingsFor<T>() where T : class, new()
        {
            var context = new BindingContext(new AppSettingsRequestData(), _locator)
                .PrefixWith(typeof(T).Name + ".");
            
            BindResult result = _resolver.BindModel(typeof (T), context);

            assertNoProblems<T>(result);

            return (T) result.Value;
        }

        private void assertNoProblems<T>(BindResult item)
        {
            if (item.Problems.Count() == 0) return;
            var bldr = new StringBuilder();
            item.Problems.Each(p => bldr.AppendFormat(
                                        "Property: {0}, Value: '{1}', Exception:{2}{3}{2}",
                                        p.Property.Name, p.Value, Environment.NewLine, p.Exception));

            throw new InvalidOperationException(
                "Could not load settings object '{0}' from appSetttings:{1}"
                    .ToFormat(typeof(T).Name, bldr.ToString()));
        }
    }
}