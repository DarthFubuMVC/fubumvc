using System;
using System.Web;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using Microsoft.Practices.ServiceLocation;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface ISparkViewModification
    {
        bool Applies(ISparkView view);
        void Modify(ISparkView view);
    }

    public class ModelAttacher : ISparkViewModification
    {
        private readonly IFubuRequest _fubuRequest;
        public ModelAttacher(IFubuRequest fubuRequest)
        {
            _fubuRequest = fubuRequest;
        }

        public bool Applies(ISparkView view)
        {
            return view is IFubuViewWithModel;
        }

        public void Modify(ISparkView view)
        {
            ((IFubuViewWithModel)view).SetModel(_fubuRequest);
        }
    }

    public class ServiceLocatorAttacher : ISparkViewModification
    {
        private readonly IServiceLocator _serviceLocator;
        public ServiceLocatorAttacher(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public bool Applies(ISparkView view)
        {
            return view is IFubuPage;
        }

        public void Modify(ISparkView view)
        {
            ((IFubuPage)view).ServiceLocator = _serviceLocator;
        }
    }

    public class SiteResourceAttacher : ISparkViewModification
    {
        private readonly ISparkViewEngine _engine;
        private readonly HttpContextBase _context;

        public SiteResourceAttacher(ISparkViewEngine engine, HttpContextBase context)
        {
            _engine = engine;
            _context = context;
        }

        public bool Applies(ISparkView view)
        {
            return view is IFubuSparkView;
        }

        public void Modify(ISparkView view)
        {
            // TODO: REFACTOR/IMPROVE
            ((IFubuSparkView) view).SiteResource = SiteResource;
        }
        public string SiteResource(string path)
        {
            return _engine.ResourcePathManager.GetResourcePath(steRoot(), path);
        }
        private string steRoot()
        {
            var context = _context;
            string siteRoot;
            var appPath = context.Request.ApplicationPath;
            if (string.IsNullOrEmpty(appPath) || string.Equals(appPath, "/"))
            {
                siteRoot = string.Empty;
            }
            else
            {
                siteRoot = "/" + appPath.Trim('/');
            }
            return siteRoot;
        }

    }

}