using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface IRenderInfo
    {
        bool IsNested();
        bool IsPartial();
        bool IsAjax();
    }

    public class RenderInfo : IRenderInfo
    {
        private readonly IRequestData _requestData;

        public RenderInfo(IRequestData requestData)
        {
            _requestData = requestData;
        }

        public bool IsNested()
        {
            var stack = new StackTrace();
            var frames = stack.GetFrames();
            if (frames == null || frames.Length == 0)
            {
                return false;
            }
            var methods = frames.Select(x => x.GetMethod());
            var type = typeof(IRenderStrategy);
            var name = ReflectionHelper.GetMethod<IRenderStrategy>(z => z.Invoke()).Name;
            return methods
                .Where(x => x.Name == name)
                .Where(x => type.IsAssignableFrom(x.DeclaringType))
                .Any();
        }


        public bool IsPartial()
        {
            var stack = new StackTrace();
            var frames = stack.GetFrames();
            if (frames == null || frames.Length == 0)
            {
                return false;
            }
            var methods = frames.Select(x => x.GetMethod());
            var type = typeof(IPartialInvoker);
            var invokeName = ReflectionHelper.GetMethod<IPartialInvoker>(z => z.Invoke<string>()).Name;
            var invokeObjectName = ReflectionHelper.GetMethod<IPartialInvoker>(z => z.InvokeObject(null)).Name;

            return methods
                .Where(x => x.Name == invokeName || x.Name == invokeObjectName)
                .Where(x => type.IsAssignableFrom(x.DeclaringType))
                .Any();
        }

        public bool IsAjax()
        {
            return _requestData.IsAjaxRequest();
        }
    }

    public class NestedOutput
    {
        private Func<TextWriter> _writer;

        public void SetWriter(Func<TextWriter> writer)
        {
            _writer = writer;
        }
        public TextWriter Writer { get { return _writer(); } }
    }

    public class SparkItemDescriptors
    {
        public SparkItemDescriptors(SparkViewDescriptor viewDescriptor, SparkViewDescriptor partialDescriptor)
        {
            ViewDescriptor = viewDescriptor;
            PartialDescriptor = partialDescriptor;
        }

        public SparkViewDescriptor ViewDescriptor { get; private set; }
        public SparkViewDescriptor PartialDescriptor { get; private set; }
    }

    public interface IViewFactory
    {
        ISparkView GetView();
        ISparkView GetPartialView();
    }

    public class ViewFactory : IViewFactory
    {
        private readonly SparkItemDescriptors _descriptors;
        private readonly IDictionary<int, ISparkViewEntry> _cache;
        private readonly ISparkViewEngine _engine;
        private readonly IEnumerable<ISparkViewModification> _modifications;

        public ViewFactory(SparkItemDescriptors descriptors, IDictionary<int, ISparkViewEntry> cache, ISparkViewEngine engine, IEnumerable<ISparkViewModification> modifications)
        {
            _descriptors = descriptors;
            _modifications = modifications;
            _cache = cache;
            _engine = engine;
        }

        public ISparkView GetView()
        {
            var view = getView(_descriptors.ViewDescriptor);

            return view;
        }

        public ISparkView GetPartialView()
        {
            var view = getView(_descriptors.PartialDescriptor);

            return view;
        }

        private ISparkView getView(SparkViewDescriptor descriptor)
        {
            ISparkViewEntry entry;
            var key = descriptor.GetHashCode();

            _cache.TryGetValue(key, out entry);
            if (entry == null || !entry.IsCurrent())
            {
                entry = _engine.CreateEntry(descriptor);
                lock (_cache)
                {
                    _cache[key] = entry;
                }
            }
            var instance = entry.CreateInstance();

            _modifications
               .Where(m => m.Applies(instance))
               .Each(m => m.Modify(instance));

            return instance;
        }
    }


    public interface IRenderStrategy
    {
        bool Applies();
        void Invoke();
    }

    public class NestedRenderStrategy : IRenderStrategy
    {
        private readonly IRenderInfo _renderInfo;
        private readonly IViewFactory _viewFactory;
        private readonly NestedOutput _nestedOutput;

        public NestedRenderStrategy(IRenderInfo renderInfo, IViewFactory viewFactory, NestedOutput nestedOutput)
        {
            _renderInfo = renderInfo;
            _viewFactory = viewFactory;
            _nestedOutput = nestedOutput;
        }

        public bool Applies()
        {
            return _renderInfo.IsNested();
        }

        public void Invoke()
        {
            var partial = _viewFactory.GetPartialView();
            partial.RenderView(_nestedOutput.Writer);
        }
    }

    public class AjaxRenderStrategy : IRenderStrategy
    {
        private readonly IRenderInfo _renderInfo;
        private readonly IViewFactory _viewFactory;
        private readonly NestedOutput _nestedOutput;
        private readonly IOutputWriter _outputWriter;

        public AjaxRenderStrategy(IRenderInfo renderInfo, IViewFactory viewFactory, NestedOutput nestedOutput, IOutputWriter outputWriter)
        {
            _renderInfo = renderInfo;
            _nestedOutput = nestedOutput;
            _outputWriter = outputWriter;
            _viewFactory = viewFactory;
        }

        public bool Applies()
        {
            return _renderInfo.IsAjax();
        }

        public void Invoke()
        {
            var partial = (SparkViewBase)_viewFactory.GetPartialView();
            var writer = new StringWriter();
            partial.Output = writer;
            _nestedOutput.SetWriter(() => partial.Output);
            partial.RenderView(writer);
            _outputWriter.WriteHtml(writer);
        }
    }

    public class PartialRenderStrategy : IRenderStrategy
    {
        private readonly IRenderInfo _renderInfo;
        private readonly IViewFactory _viewFactory;
        private readonly NestedOutput _nestedOutput;
        private readonly IOutputWriter _outputWriter;


        public PartialRenderStrategy(IRenderInfo renderInfo, IViewFactory viewFactory, NestedOutput nestedOutput, IOutputWriter outputWriter)
        {
            _renderInfo = renderInfo;
            _outputWriter = outputWriter;
            _nestedOutput = nestedOutput;
            _viewFactory = viewFactory;
        }

        public bool Applies()
        {
            return _renderInfo.IsPartial();
        }

        public void Invoke()
        {
            var partial = (SparkViewBase)_viewFactory.GetPartialView();
            var writer = new StringWriter();
            partial.Output = writer;
            _nestedOutput.SetWriter(() => partial.Output);
            partial.RenderView(writer);
            _outputWriter.WriteHtml(writer);
        }
    }

    public class DefaultRenderStrategy : IRenderStrategy
    {
        private readonly IViewFactory _viewFactory;
        private readonly NestedOutput _nestedOutput;
        private readonly IOutputWriter _outputWriter;

        public DefaultRenderStrategy(IViewFactory viewFactory, NestedOutput nestedOutput, IOutputWriter outputWriter)
        {
            _viewFactory = viewFactory;
            _outputWriter = outputWriter;
            _nestedOutput = nestedOutput;
        }

        public bool Applies()
        {
            return true;
        }

        public void Invoke()
        {
            var view = (SparkViewBase)_viewFactory.GetView();
            var writer = new StringWriter();
            view.Output = writer;
            _nestedOutput.SetWriter(() => view.Output);
            view.RenderView(writer);
            _outputWriter.WriteHtml(writer);
        }
    }
}