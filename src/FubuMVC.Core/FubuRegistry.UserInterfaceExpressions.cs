using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Diagnostics;

namespace FubuMVC.Core
{
    public partial class FubuRegistry
    {
        public void HtmlConvention<T>() where T : HtmlConventionRegistry, new()
        {
            HtmlConvention(new T());
            includeHtmlDiagnostics();
        }

        public void HtmlConvention(HtmlConventionRegistry conventions)
        {
            Services(x => x.AddService(conventions));

            includeHtmlDiagnostics();
        }

        public void HtmlConvention(Action<HtmlConventionRegistry> configure)
        {
            var conventions = new HtmlConventionRegistry();
            configure(conventions);

            HtmlConvention(conventions);
            includeHtmlDiagnostics();
        }

        private void includeHtmlDiagnostics()
        {
            Import<HtmlDiagnosticsRegistry>(string.Empty);
        }

        public void StringConversions<T>() where T : DisplayConversionRegistry, new()
        {
            var conversions = new T();

            addStringConversions(conversions);
            includeHtmlDiagnostics();
        }

        private void addStringConversions(DisplayConversionRegistry conversions)
        {
            Services(x =>
            {
                x.SetServiceIfNone(new Stringifier());
                var stringifier = x.FindAllValues<Stringifier>().First();

                conversions.Configure(stringifier);
            });
        }

        public void StringConversions(Action<DisplayConversionRegistry> configure)
        {
            var conversions = new DisplayConversionRegistry();
            configure(conversions);

            addStringConversions(conversions);
            includeHtmlDiagnostics();
        }
    }
}