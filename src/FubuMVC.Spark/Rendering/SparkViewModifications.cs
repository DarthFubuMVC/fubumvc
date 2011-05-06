using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuCore;
using FubuMVC.Core.View.Activation;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface ISparkViewModification
    {
        bool Applies(ISparkView view);
        void Modify(ISparkView view);
    }

    public class PageActivation : ISparkViewModification
    {
        private readonly IPageActivator _activator;
        public PageActivation(IPageActivator activator)
        {
            _activator = activator;
        }

        public bool Applies(ISparkView view)
        {
            return view is IFubuPage;
        }

        public void Modify(ISparkView view)
        {
            _activator.Activate((IFubuPage)view);
        }
    }
	
    public class SiteResourceAttacher : ISparkViewModification
    {
        private readonly ISparkViewEngine _engine;
		private readonly IFubuRequest _request;
		
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