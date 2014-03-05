using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View.Rendering;
using Spark;

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