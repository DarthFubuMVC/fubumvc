using System.Collections.Generic;
using System.Web;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Extensibility;
using FubuMVC.Razor.Rendering;
using HtmlTags;
using RazorEngine.Configuration;

namespace FubuMVC.Razor
{
	public class RazorActivator : IActivator
	{
        private readonly ITemplateServiceConfiguration _engine;

		public RazorActivator (ITemplateServiceConfiguration engine)
		{
			_engine = engine;
		}

		public void Activate (IEnumerable<IPackageInfo> packages, IPackageLog log)
		{
            log.Trace("Running {0}".ToFormat(GetType().Name));
			
            configureRazorSettings(log);
            setEngineDependencies(log);
		}

        private void configureRazorSettings(IPackageLog log)
        {
            _engine.Namespaces.Add(typeof(VirtualPathUtility).Namespace); // System.Web
            _engine.Namespaces.Add(typeof(FubuRegistryExtensions).Namespace); // FubuMVC.Razor
            _engine.Namespaces.Add(typeof(FubuPageExtensions).Namespace); // FubuMVC.Core.UI
            _engine.Namespaces.Add(typeof(ContentExtensions).Namespace); // FubuMVC.Core.UI.Extensibility
            _engine.Namespaces.Add(typeof(HtmlTag).Namespace); // HtmlTags   

            log.Trace("Adding namespaces to RazorSettings:");
            _engine.Namespaces.Each(x => log.Trace("  - {0}".ToFormat(x)));
        }

	    private void setEngineDependencies(IPackageLog log)
	    {
	        ((TemplateServiceConfiguration) _engine).BaseTemplateType = typeof (FubuRazorView);
        }
	}
}