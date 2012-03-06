using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;
using Spark;
using FubuMVC.Core;

namespace FubuMVC.Spark.Rendering
{
    public class CacheAttacher : BasicViewModifier
    {
        private readonly ICacheService _cacheService;
        public CacheAttacher(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public override bool Applies(IRenderableView view)
        {
            return view is IFubuSparkView;
        }

        public override IRenderableView Modify(IRenderableView view)
        {
            return view.Modify(v => v.As<IFubuSparkView>().CacheService = _cacheService);
        }
    }

    public class SiteResourceAttacher : BasicViewModifier
    {
        private readonly ISparkViewEngine _engine;
        private readonly CurrentRequest _request;

        public SiteResourceAttacher(ISparkViewEngine engine, IFubuRequest request)
        {
            _engine = engine;
            _request = request.Get<CurrentRequest>();
        }

        public override bool Applies(IRenderableView view)
        {
            return view is IFubuSparkView;
        }

        public override IRenderableView Modify(IRenderableView view)
        {
            return view.As<IFubuSparkView>().Modify(v => v.SiteResource = SiteResource);
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

    public class ContentActivation : BasicViewModifier
    {
        private readonly Dictionary<string, TextWriter> _content;
        public ContentActivation()
        {
            _content = new Dictionary<string, TextWriter>();
        }

        public override bool Applies(IRenderableView view)
        {
            return view is IFubuSparkView;
        }

        public override IRenderableView Modify(IRenderableView view)
        {
            return view.As<IFubuSparkView>().Modify(v => v.Content = _content);
        }
    }

    public class OnceTableActivation : BasicViewModifier
    {
        private readonly Dictionary<string, string> _once;

        public OnceTableActivation()
        {
            _once = new Dictionary<string, string>();
        }

        public override bool Applies(IRenderableView view)
        {
            return view is IFubuSparkView;
        }

        public override IRenderableView Modify(IRenderableView view)
        {
            return view.As<IFubuSparkView>().Modify(v => v.OnceTable = _once);
        }
    }

    public class ViewContentDisposer : IViewModifier
    {
        private readonly NestedOutput _nestedOutput;
        public ViewContentDisposer(NestedOutput nestedOutput)
        {
            _nestedOutput = nestedOutput;
        }

        public bool Applies(IRenderableView view)
        {
            return !_nestedOutput.IsActive()
                && view is IFubuSparkView;
        }

        public IRenderableView Modify(IRenderableView view)
        {
            var disposer = new FubuSparkViewDecorator(view.As<IFubuSparkView>());

            // proactively dispose named content. pools spoolwriter pages. avoids finalizers.
            disposer.PostRender += x => x.Content.Values.Each(c => c.Close());
            disposer.PostRender += x => x.Content.Clear();
            
            return disposer;
        }
    }

    public class OuterViewOutputActivator : IViewModifier
    {
        private readonly NestedOutput _nestedOutput;
        private readonly ViewOutput _viewOutput;

        public OuterViewOutputActivator(NestedOutput nestedOutput, ViewOutput viewOutput)
        {
            _nestedOutput = nestedOutput;
            _viewOutput = viewOutput;
        }

        public bool Applies(IRenderableView view)
        {
            return !_nestedOutput.IsActive()
                && view is IFubuSparkView;
        }

        public IRenderableView Modify(IRenderableView view)
        {
            return view.Modify(v => v.As<IFubuSparkView>().Output = _viewOutput);
        }
    }

    public class NestedViewOutputActivator : IViewModifier
    {
        private readonly NestedOutput _nestedOutput;
        public NestedViewOutputActivator(NestedOutput nestedOutput)
        {
            _nestedOutput = nestedOutput;
        }

        public bool Applies(IRenderableView view)
        {
            var sparkView = view as IFubuSparkView;
            return sparkView != null
                && sparkView.Output == null;
        }

        public IRenderableView Modify(IRenderableView view)
        {
            return view.Modify(v => v.As<IFubuSparkView>().Output = _nestedOutput.Writer);
        }
    }

    public class NestedOutputActivation : IViewModifier
    {
        private readonly NestedOutput _nestedOutput;
        public NestedOutputActivation(NestedOutput nestedOutput)
        {
            _nestedOutput = nestedOutput;
        }

        public bool Applies(IRenderableView view)
        {
            return !_nestedOutput.IsActive()
                && view is IFubuSparkView;
        }

        public IRenderableView Modify(IRenderableView view)
        {
            return view.Modify(v => _nestedOutput.SetWriter(() => v.As<IFubuSparkView>().Output));
        }
    }

}