using System;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuCore;
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
	
	// TODO: UT
    public class SiteResourceAttacher : ISparkViewModification
    {
        private readonly ISparkViewEngine _engine;
		private IFubuRequest _request;
		
        public SiteResourceAttacher(ISparkViewEngine engine, IFubuRequest request)
        {
            _engine = engine;
			_request = request;
        }

        public bool Applies(ISparkView view)
        {
            return view is IFubuSparkView;
        }

        public void Modify(ISparkView view)
        {
            ((IFubuSparkView) view).SiteResource = SiteResource;
        }
		
        public string SiteResource(string path)
        {
            return _engine.ResourcePathManager.GetResourcePath(siteRoot(), path);
        }
		
        private string siteRoot()
        {			
			var appPath = _request.Get<AppPath>().ApplicationPath;
			var siteRoot = string.Empty;
            
			if (appPath.IsNotEmpty() && !string.Equals(appPath, "/"))
            {
                siteRoot = "/{0}".ToFormat(appPath.Trim('/'));
            }
			
            return siteRoot;
        }
		
		#region Nested Class: AppPath
		
		public class AppPath
		{
			public string ApplicationPath {get;set;}
        }
		
		#endregion
    }

}