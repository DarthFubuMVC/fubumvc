using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuMVC.Razor.Rendering;

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

        protected override void registerServices(ServiceRegistry services)
        {
            services.SetServiceIfNone<IPartialRenderer, PartialRenderer>();
        }

        protected override void registerWatchSettings(AssetSettings settings)
        {
            settings.ContentMatches.Add("*.cshtml");
        }

        protected override void addNamespacesForViews(CommonViewNamespaces namespaces)
        {
            namespaces.AddForType<RazorViewFacility>(); // FubuMVC.Razor
            namespaces.AddForType<IPartialInvoker>(); // FubuMVC.Core.UI
        }

        public override void ReadSharedNamespaces(CommonViewNamespaces namespaces)
        {
            // nothing
        }
    }
}