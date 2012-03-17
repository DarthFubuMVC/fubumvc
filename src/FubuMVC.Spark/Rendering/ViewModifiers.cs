using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View.Rendering;
using Spark;
using FubuMVC.Core;

namespace FubuMVC.Spark.Rendering
{
    public class CacheAttacher : BasicViewModifier<IFubuSparkView>
    {
        private readonly ICacheService _cacheService;
        public CacheAttacher(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public override IFubuSparkView Modify(IFubuSparkView view)
        {
            return view.Modify(v => v.CacheService = _cacheService);
        }
    }

    public class SiteResourceAttacher : BasicViewModifier<IFubuSparkView>
    {
        private readonly ISparkViewEngine _engine;
        private readonly CurrentRequest _request;

        public SiteResourceAttacher(ISparkViewEngine engine, IFubuRequest request)
        {
            _engine = engine;
            _request = request.Get<CurrentRequest>();
        }

        public override IFubuSparkView Modify(IFubuSparkView view)
        {
            return view.Modify(v => v.SiteResource = SiteResource);
        }

        public string SiteResource(string path)
        {
            var appPath = _request.ApplicationPath;
            var siteRoot = string.Empty;
            if (appPath.IsNotEmpty() && !string.Equals(appPath, "/"))
            {
                siteRoot = "/{0}".ToFormat(appPath.Trim('/'));
            }

            return _engine.ResourcePathManager.GetResourcePath(siteRoot, path);
        }
    }

    public class ContentActivation : BasicViewModifier<IFubuSparkView>
    {
        private readonly Dictionary<string, TextWriter> _content;
        public ContentActivation()
        {
            _content = new Dictionary<string, TextWriter>();
        }

        public override IFubuSparkView Modify(IFubuSparkView view)
        {
            return view.Modify(v => v.Content = _content);
        }
    }

    public class OnceTableActivation : BasicViewModifier<IFubuSparkView>
    {
        private readonly Dictionary<string, string> _once;

        public OnceTableActivation()
        {
            _once = new Dictionary<string, string>();
        }

        public override IFubuSparkView Modify(IFubuSparkView view)
        {
            return view.Modify(v => v.OnceTable = _once);
        }
    }

    public class ViewContentDisposer : IViewModifier<IFubuSparkView>
    {
        private readonly NestedOutput _nestedOutput;
        public ViewContentDisposer(NestedOutput nestedOutput)
        {
            _nestedOutput = nestedOutput;
        }

        public bool Applies(IFubuSparkView view)
        {
            return !_nestedOutput.IsActive();
        }

        public IFubuSparkView Modify(IFubuSparkView view)
        {
            var disposer = new FubuSparkViewDecorator(view);

            // proactively dispose named content. pools spoolwriter pages. avoids finalizers.
            disposer.PostRender += x => x.Content.Values.Each(c => c.Close());
            disposer.PostRender += x => x.Content.Clear();
            
            return disposer;
        }
    }

    public class OuterViewOutputActivator : IViewModifier<IFubuSparkView>
    {
        private readonly NestedOutput _nestedOutput;
        private readonly ViewOutput _viewOutput;

        public OuterViewOutputActivator(NestedOutput nestedOutput, ViewOutput viewOutput)
        {
            _nestedOutput = nestedOutput;
            _viewOutput = viewOutput;
        }

        public bool Applies(IFubuSparkView view)
        {
            return !_nestedOutput.IsActive();
        }

        public IFubuSparkView Modify(IFubuSparkView view)
        {
            return view.Modify(v => v.Output = _viewOutput);
        }
    }

    public class NestedViewOutputActivator : IViewModifier<IFubuSparkView>
    {
        private readonly NestedOutput _nestedOutput;
        public NestedViewOutputActivator(NestedOutput nestedOutput)
        {
            _nestedOutput = nestedOutput;
        }

        public bool Applies(IFubuSparkView view)
        {
            return view != null
                && view.Output == null;
        }

        public IFubuSparkView Modify(IFubuSparkView view)
        {
            return view.Modify(v => v.Output = _nestedOutput.Writer);
        }
    }

    public class NestedOutputActivation : IViewModifier<IFubuSparkView>
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

        public IFubuSparkView Modify(IFubuSparkView view)
        {
            return view.Modify(v => _nestedOutput.SetWriter(() => v.Output));
        }
    }

}