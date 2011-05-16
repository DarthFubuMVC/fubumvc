using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuMVC.Core.View.Activation;
using Spark;
using FubuMVC.Core;
namespace FubuMVC.Spark.Rendering
{
    public interface ISparkViewModification
    {
        bool Applies(IFubuSparkView view);
        IFubuSparkView Modify(IFubuSparkView view);
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

        public IFubuSparkView Modify(IFubuSparkView view)
        {
            _nestedOutput.SetWriter(() => view.Output);
            return view;
        }
    }

    // TODO : UT
    public class ContentActivation : ISparkViewModification
    {
        private readonly Dictionary<string, TextWriter> _content;
        public ContentActivation()
        {
            _content = new Dictionary<string, TextWriter>();
        }

        public bool Applies(IFubuSparkView view)
        {
            return true;
        }

        public IFubuSparkView Modify(IFubuSparkView view)
        {
            view.Content = _content;
            return view;
        }
    }

    // TODO : UT
    public class OnceTableActivation : ISparkViewModification
    {
        private readonly Dictionary<string, string> _once;

        public OnceTableActivation()
        {
            _once = new Dictionary<string, string>();
        }

        public bool Applies(IFubuSparkView view)
        {
            return true;
        }

        public IFubuSparkView Modify(IFubuSparkView view)
        {
            view.OnceTable = _once;
            return view;
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
            return true;
        }

        public IFubuSparkView Modify(IFubuSparkView view)
        {
            _activator.Activate(view);
            return view;
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

        public IFubuSparkView Modify(IFubuSparkView view)
        {
            view.SiteResource = SiteResource;
            return view;
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

    // TODO : UT
    public class ViewDecoratorModification : ISparkViewModification
    {
        private readonly NestedOutput _nestedOutput;

        public ViewDecoratorModification(NestedOutput nestedOutput)
        {
            _nestedOutput = nestedOutput;
        }

        public bool Applies(IFubuSparkView view)
        {
            return !_nestedOutput.IsActive();
        }

        public IFubuSparkView Modify(IFubuSparkView view)
        {
            var decorated = new FubuSparkViewDecorator(view);
            // proactively dispose named content. pools spoolwriter pages. avoids finalizers.
            decorated.PostRender += x => x.Content.Values.Each(c => c.Close());
            decorated.PostRender += x => x.Content.Clear();
            return decorated;
        }
    }


    // TODO : UT
    public class OuterViewOutputActivator : ISparkViewModification
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
            view.Output = _viewOutput;
            return view;
        }
    }

    // TODO : UT
    public class NestedViewOutputActivator : ISparkViewModification
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
            view.Output = _nestedOutput.Writer;
            return view;
        }
    }
}