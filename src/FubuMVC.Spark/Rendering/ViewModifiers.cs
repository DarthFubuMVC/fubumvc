using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuMVC.Core.View.Activation;
using Spark;
using FubuMVC.Core;
namespace FubuMVC.Spark.Rendering
{
    public interface IViewModifier
    {
        bool Applies(IFubuSparkView view);
        IFubuSparkView Modify(IFubuSparkView view);
    }

    public class BasicViewModifier : IViewModifier
    {
        public virtual bool Applies(IFubuSparkView view) { return true; }
        public virtual IFubuSparkView Modify(IFubuSparkView view) { return view; }
    }

    public class PageActivation : BasicViewModifier
    {
        private readonly IPageActivator _activator;
        public PageActivation(IPageActivator activator)
        {
            _activator = activator;
        }

        public override IFubuSparkView Modify(IFubuSparkView view)
        {
            return view.Modify(v => _activator.Activate(v));
        }
    }

    public class SiteResourceAttacher : BasicViewModifier
    {
        private readonly ISparkViewEngine _engine;
        private readonly CurrentRequest _request;

        public SiteResourceAttacher(ISparkViewEngine engine, CurrentRequest request)
        {
            _engine = engine;
            _request = request;
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

    // TODO : UT
    public class ContentActivation : BasicViewModifier
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

    // TODO : UT
    public class OnceTableActivation : BasicViewModifier
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

    // TODO : UT
    public class ViewContentDisposer : IViewModifier
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

    // TODO : UT
    public class OuterViewOutputActivator : IViewModifier
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

    // TODO : UT
    public class NestedViewOutputActivator : IViewModifier
    {
        private readonly NestedOutput _nestedOutput;
        public NestedViewOutputActivator(NestedOutput nestedOutput)
        {
            _nestedOutput = nestedOutput;
        }

        public bool Applies(IFubuSparkView view)
        {
            return view.Output == null;
        }

        public IFubuSparkView Modify(IFubuSparkView view)
        {
            return view.Modify(v => v.Output = _nestedOutput.Writer);
        }
    }

    // TODO : UT
    public class NestedOutputActivation : IViewModifier
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