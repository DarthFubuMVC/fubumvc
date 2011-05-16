using FubuCore;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Activation;
using Spark;
using FubuMVC.Core;
namespace FubuMVC.Spark.Rendering
{
    public interface ISparkViewModification
    {
        bool Applies(IFubuSparkView view);
        void Modify(IFubuSparkView view);
    }
	
	// TODO : UT
	public class NestedOutputActivation : ISparkViewModification
    {
        private readonly NestedOutput _nestedOutput;		
        public NestedOutputActivation(NestedOutput nestedOutput)
        {
            _nestedOutput = nestedOutput;
        }

        public bool Applies(IFubuSparkView view)
        {
            return !_nestedOutput.IsActive();
        }

        public void Modify(IFubuSparkView view)
        {
            _nestedOutput.SetView(() => view);
        }
    }
	
	// TODO : UT
	public class NestedOutputSwitch : ISparkViewModification
    {
        private readonly NestedOutput _nestedOutput;
        public NestedOutputSwitch(NestedOutput nestedOutput)
        {
            _nestedOutput = nestedOutput;
        }

        public bool Applies(IFubuSparkView view)
        {
            return _nestedOutput.IsActive();
        }

        public void Modify(IFubuSparkView view)
        {
			// assume the values of the outer view collections			
			var outerView = _nestedOutput.View;			
			view.Content = outerView.Content;
            view.OnceTable = outerView.OnceTable;
        }
    }
	
	
    public class PageActivation : ISparkViewModification
    {
        private readonly IPageActivator _activator;
        public PageActivation(IPageActivator activator)
        {
            _activator = activator;
        }

        public bool Applies(IFubuSparkView view)
        {
            return view is IFubuPage;
        }

        public void Modify(IFubuSparkView view)
        {
            _activator.Activate((IFubuPage)view);
        }
    }
	
    public class SiteResourceAttacher : ISparkViewModification
    {
        private readonly ISparkViewEngine _engine;
		private readonly CurrentRequest _request;
		
        public SiteResourceAttacher(ISparkViewEngine engine, CurrentRequest request)
        {
            _engine = engine;
			_request = request;
        }

        public bool Applies(IFubuSparkView view)
        {
            return true;
        }

        public void Modify(IFubuSparkView view)
        {
            view.SiteResource = SiteResource;
        }
		
        public string SiteResource(string path)
        {
            return _engine.ResourcePathManager.GetResourcePath(siteRoot(), path);
        }
		
        private string siteRoot()
        {			
			var appPath = _request.ApplicationPath;
			var siteRoot = string.Empty;
            
			if (appPath.IsNotEmpty() && !string.Equals(appPath, "/"))
            {
                siteRoot = "/{0}".ToFormat(appPath.Trim('/'));
            }
			
            return siteRoot;
        }
    }
}