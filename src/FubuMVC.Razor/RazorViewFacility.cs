using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor
{
    public class RazorViewFacility : ViewFacility<RazorTemplate>
    {
        public override Func<IFubuFile, RazorTemplate> CreateBuilder(SettingsCollection settings)
        {
            var razorSettings = settings.Get<RazorEngineSettings>();
            var namespaces = settings.Get<CommonViewNamespaces>();

            var factory = new TemplateFactoryCache(namespaces, razorSettings, new TemplateCompiler(),
                new RazorTemplateGenerator());

            return file => new RazorTemplate(file, factory);
        }

        public override FileSet FindMatching(SettingsCollection settings)
        {
            return settings.Get<RazorEngineSettings>().Search;
        }

        
    }
}