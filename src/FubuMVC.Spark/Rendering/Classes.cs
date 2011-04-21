using System;
using System.Collections.Generic;
using System.IO;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Spark.SparkModel;
using Microsoft.Practices.ServiceLocation;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface IRenderAction
    {
        void Invoke(RenderContext context);
    }

    public class RenderContext
    {
        private readonly IDictionary<Type, object> _items = new Dictionary<Type, object>();

        public T Get<T>()
        {
            return (T)_items[typeof(T)];
        }

        public void Set<T>(T value)
        {
            _items[typeof(T)] = value;
        }
    }

    public class OutputContext
    {
        private SparkItem _item;

        public OutputContext()
        {
            Writer = new StringWriter();
        }

        public TextWriter Writer { get; private set; }

        public void SetSparkItem(SparkItem sparkItem)
        {
            if (_item == null)
            {
                _item = sparkItem;
            }
        }

        public bool IsPartial(SparkItem sparkItem)
        {
            return sparkItem != _item;
        }
    }

    public class PartialOutputContext
    {
        private Func<TextWriter> _writer;

        public void SetWriter(Func<TextWriter> writer)
        {
            _writer = writer;
        }

        public TextWriter Writer
        {
            get
            {
                return _writer();
            }
        }
    }

    public class InvokeRenderAction : IRenderAction
    {
        private readonly IRenderAction _inner;
        private readonly OutputContext _outputContext;

        public InvokeRenderAction(IRenderAction inner, OutputContext outputContext)
        {
            _inner = inner;
            _outputContext = outputContext;
        }

        public void Invoke(RenderContext context)
        {
            var item = context.Get<SparkItem>();
            if (!_outputContext.IsPartial(item))
            {
                _inner.Invoke(context);
            }
        }
    }

    public class InvokePartialRenderAction : IRenderAction
    {
        private readonly IRenderAction _inner;
        private readonly OutputContext _outputContext;

        public InvokePartialRenderAction(IRenderAction inner, OutputContext outputContext)
        {
            _inner = inner;
            _outputContext = outputContext;
        }

        public void Invoke(RenderContext context)
        {
            if (_outputContext.IsPartial(context.Get<SparkItem>()))
            {
                _inner.Invoke(context);
            }
        }
    }

    public class SetSparkItemAction : IRenderAction
    {
        private readonly SparkItem _item;
        private readonly OutputContext _outputContext;

        public SetSparkItemAction(SparkItem item, OutputContext outputContext)
        {
            _item = item;
            _outputContext = outputContext;
        }

        public void Invoke(RenderContext context)
        {
            context.Set(_item);
            _outputContext.SetSparkItem(_item);
        }
    }

    public class SetDescriptorAction : IRenderAction
    {
        public void Invoke(RenderContext context)
        {
            var descriptor = new SparkViewDescriptor();
            descriptor.AddTemplate(context.Get<SparkItem>().PrefixedRelativePath);
            context.Set(descriptor);
        }
    }

    public class SetMasterAction : IRenderAction
    {
        // Methods
        public void Invoke(RenderContext context)
        {
            context.Get<SparkViewDescriptor>().AddTemplate(context.Get<SparkItem>().Master.PrefixedRelativePath);
        }
    }

    public class SetEntryAction : IRenderAction
    {
        // Fields
        private readonly IDictionary<int, ISparkViewEntry> _cache;
        private readonly ISparkViewEngine _engine;

        // Methods
        public SetEntryAction(IDictionary<int, ISparkViewEntry> cache, ISparkViewEngine engine)
        {
            _cache = cache;
            _engine = engine;
        }

        public void Invoke(RenderContext context)
        {
            ISparkViewEntry entry;
            var descriptor = context.Get<SparkViewDescriptor>();
            var hashCode = descriptor.GetHashCode();
            lock (_cache)
            {
                _cache.TryGetValue(hashCode, out entry);
                if (!((entry != null) && entry.IsCurrent()))
                {
                    entry = this._engine.CreateEntry(descriptor);
                    _cache[hashCode] = entry;
                }
            }
            context.Set(entry);
        }
    }

    public class SetSparkViewInstanceAction : IRenderAction
    {
        // Methods
        public void Invoke(RenderContext context)
        {
            context.Set(context.Get<ISparkViewEntry>().CreateInstance());
        }
    }

    public class ActivateSparkViewAction : IRenderAction
    {
        // Fields
        private readonly IServiceLocator _locator;
        private readonly IFubuRequest _request;

        // Methods
        public ActivateSparkViewAction(IFubuRequest request, IServiceLocator locator)
        {
            _request = request;
            _locator = locator;
        }

        public void Invoke(RenderContext context)
        {
            var page = (IFubuPage)context.Get<ISparkView>();
            page.ServiceLocator = _locator;
            if (page is IFubuViewWithModel)
            {
                ((IFubuViewWithModel)page).SetModel(this._request);
            }
        }
    }

    public class SetPartialOutputWriterAction : IRenderAction
    {
        private readonly PartialOutputContext _partialOutputContext;

        public SetPartialOutputWriterAction(PartialOutputContext partialOutputContext)
        {
            _partialOutputContext = partialOutputContext;
        }

        public void Invoke(RenderContext context)
        {
            var view = (SparkViewBase)context.Get<ISparkView>();
            _partialOutputContext.SetWriter(() => view.Output);
        }
    }

    public class RenderPartialViewAction : IRenderAction
    {
        private readonly PartialOutputContext _partialOutputContext;

        public RenderPartialViewAction(PartialOutputContext partialOutputContext)
        {
            _partialOutputContext = partialOutputContext;
        }

        public void Invoke(RenderContext context)
        {
            context.Get<ISparkView>().RenderView(this._partialOutputContext.Writer);
        }
    }

    public class RenderViewAction : IRenderAction
    {
        private readonly OutputContext _outputContext;

        public RenderViewAction(OutputContext outputContext)
        {
            _outputContext = outputContext;
        }

        public void Invoke(RenderContext context)
        {
            context.Get<ISparkView>().RenderView(_outputContext.Writer);
        }
    }

    public class WriteViewOutputAction : IRenderAction
    {
        private readonly OutputContext _outputContext;
        private readonly IOutputWriter _writer;

        public WriteViewOutputAction(IOutputWriter writer, OutputContext outputContext)
        {
            _writer = writer;
            _outputContext = outputContext;
        }

        public void Invoke(RenderContext context)
        {
            _writer.WriteHtml(_outputContext.Writer.ToString());
        }
    }
}