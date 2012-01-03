using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View.Activation;
using FubuMVC.Core;

namespace FubuMVC.Razor.Rendering
{
    public interface IViewModifier
    {
        bool Applies(IFubuRazorView view);
        IFubuRazorView Modify(IFubuRazorView view);
    }

    public interface IViewModifierService
    {
        IFubuRazorView Modify(IFubuRazorView view);
    }

    public class ViewModifierService : IViewModifierService
    {
        private readonly IEnumerable<IViewModifier> _modifications;

        public ViewModifierService(IEnumerable<IViewModifier> modifications)
        {
            _modifications = modifications;
        }

        public IFubuRazorView Modify(IFubuRazorView view)
        {
            foreach (var modification in _modifications)
            {
                if (modification.Applies(view))
                {
                    view = modification.Modify(view); // consider if we should add a "?? view;" or just let it fail
                }
            }
            return view;
        }
    }

    public class BasicViewModifier : IViewModifier
    {
        public virtual bool Applies(IFubuRazorView view) { return true; }
        public virtual IFubuRazorView Modify(IFubuRazorView view) { return view; }
    }

    public class PageActivation : BasicViewModifier
    {
        private readonly IPageActivator _activator;
        public PageActivation(IPageActivator activator)
        {
            _activator = activator;
        }

        public override IFubuRazorView Modify(IFubuRazorView view)
        {
            return view.Modify(v => _activator.Activate(v));
        }
    }

    public class LayoutActivation : BasicViewModifier
    {
        private readonly IPageActivator _pageActivator;

        public LayoutActivation(IPageActivator pageActivator)
        {
            _pageActivator = pageActivator;
        }

        public override IFubuRazorView Modify(IFubuRazorView view)
        {
            if (view.Layout != null)
            {
                view.Layout.As<IFubuRazorView>().ServiceLocator = view.ServiceLocator;
                Modify(view.Layout.As<IFubuRazorView>());
            }
            return view;
        }
    }

    //public class SiteResourceAttacher : BasicViewModifier
    //{
    //    private readonly IRazorViewEntryFactory _entryFactory;
    //    private readonly CurrentRequest _request;

    //    public SiteResourceAttacher(IRazorViewEntryFactory entryFactory, IFubuRequest request)
    //    {
    //        _entryFactory = entryFactory;
    //        _request = request.Get<CurrentRequest>();
    //    }

    //    public override IFubuRazorView Modify(IFubuRazorView view)
    //    {
    //        return view.Modify(v => v.SiteResource = SiteResource);
    //    }

    //    public string SiteResource(string path)
    //    {
    //        var appPath = _request.ApplicationPath;
    //        var siteRoot = string.Empty;
    //        if (appPath.IsNotEmpty() && !string.Equals(appPath, "/"))
    //        {
    //            siteRoot = "/{0}".ToFormat(appPath.Trim('/'));
    //        }

    //        //TODO handle resource path

    //        return string.Empty;
    //        //return _entryFactory.ResourcePathManager.GetResourcePath(siteRoot, path);
    //    }
    //}
}