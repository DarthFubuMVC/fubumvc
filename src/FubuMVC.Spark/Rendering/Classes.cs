using System;
using System.Collections.Generic;
using System.IO;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using Microsoft.Practices.ServiceLocation;
using Spark;

/*
 * Generally, I think this is a _really_ good first stab. :)
 * 
 *   It gives a good idea on which parts are needed in the rendering, thus a good starting point for further work.
 *   
 *   However, there are some things I would like to note on design. 
 * 
 * - Some parts of that "pipe" are overhead. When we go from config to runtime some parts can and should be _locked_ down.
 * 
 * - Too much of contexts. Let's not forget initial goals, Func<Stream>.
 * 
 * - RenderContext, there must already be something in Fubu that we can use.
 *   Why is it needed on the interface IRenderAction? Nested container gives you a singleton, btw.
 * 
 * - Output/PartialOutput contexts. Why are we asking a SparkOutput if it is partial, when we have a PartialOutput etc?
 *   (because we try to handle partial + normal in one long pipe run)
 * 
 * - I really would like to see SparkItem not being so dominant - Seems like everything is very dependant on this 
 *   (it was really just intended to establish enough context to be able to establish a "build recipe" for ISparkViewEntry. 
 *   Was the idea not to hide away the particulars by establishing Func<>s during config time?
 *   
 * - Not sure what to think of the IRenderAction - Seems like too much stuff moved over to runtime.
 *   For instance, why do we need to create descriptors _explicitly_ on each request? We are interested in instances of ISparkView, 
 *   why not inject Func<> ? The runtime seems overly busy.
 *   
 *   What is really interesting is to utilize callbacks and _inject_ Func<ISparkViewEntry> - partial renderer + normal renderer, separate things no if checks.
 *   We need to hide away particulars about SparkItem/Descriptors. We are more interested in Func that gives us sparkview instances on runtime.
 */


namespace FubuMVC.Spark.Rendering
{
    public class RenderContext
    {
        private bool _isPartial;

        public void SetAsPartial()
        {
            _isPartial = true;
        }
        public bool IsPartial()
        {
            return _isPartial;
        }
    }

    public class PartialOutput
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

    public interface ISparkViewActivator
    {
        void Activate(ISparkView sparkView);
    }

    public class SparkViewActivator : ISparkViewActivator
    {
        private readonly IServiceLocator _locator;
        private readonly IFubuRequest _request;

        public SparkViewActivator(IServiceLocator locator, IFubuRequest request)
        {
            _locator = locator;
            _request = request;
        }

        public void Activate(ISparkView sparkView)
        {
            if (sparkView is IFubuPage)
            {
                ((IFubuPage)sparkView).ServiceLocator = _locator;
            }
            if (sparkView is IFubuViewWithModel)
            {
                ((IFubuViewWithModel)sparkView).SetModel(_request);
            }
        }
    }

    public interface ISparkViewProvider
    {
        ISparkView GetView();
    }

    public class SparkViewProvider : ISparkViewProvider
    {
        private readonly SparkViewDescriptor _descriptor;
        private readonly IDictionary<int, ISparkViewEntry> _cache;
        private readonly ISparkViewActivator _activator;
        private readonly ISparkViewEngine _engine;

        public SparkViewProvider(IDictionary<int, ISparkViewEntry> cache, SparkViewDescriptor descriptor, ISparkViewActivator activator, ISparkViewEngine engine)
        {
            _descriptor = descriptor;
            _cache = cache;
            _activator = activator;
            _engine = engine;
        }

        public ISparkView GetView()
        {
            var view = getView();
            _activator.Activate(view);
            return view;
        }

        private ISparkView getView()
        {
            ISparkViewEntry entry;
            var key = _descriptor.GetHashCode();
            _cache.TryGetValue(key, out entry);
            if (entry == null || !entry.IsCurrent())
            {
                entry = _engine.CreateEntry(_descriptor);
                lock (_cache)
                {
                    _cache[key] = entry;
                }
            }
            return entry.CreateInstance();
        }
    }

    public interface IPartialRenderAction
    {
        void Execute();
    }

    public class PartialRenderAction : IPartialRenderAction
    {
        private readonly ISparkViewProvider _provider;
        private readonly PartialOutput _partialOutput;

        public PartialRenderAction(ISparkViewProvider provider, PartialOutput partialOutput )
        {
            _provider = provider;
            _provider = provider;
            _partialOutput = partialOutput;
        }

        public void Execute()
        {
            var view = _provider.GetView();
            view.RenderView(_partialOutput.Writer);
        }
    }

    public interface IRenderAction
    {
        void Execute();
    }

    public class RenderAction : IRenderAction
    {
        private readonly ISparkViewProvider _provider;
        private readonly IOutputWriter _outputWriter;
        private readonly PartialOutput _partialOutput;

        public RenderAction(ISparkViewProvider provider, IOutputWriter outputWriter, PartialOutput partialOutput)
        {
            _provider = provider;
            _partialOutput = partialOutput;
            _outputWriter = outputWriter;
        }

        public void Execute()
        {
            var writer = new StringWriter();
            var view = (SparkViewBase)_provider.GetView();

            _partialOutput.SetWriter(() => view.Output);
            view.Output = writer;
            view.RenderView(writer);

            _outputWriter.WriteHtml(writer);
        }
    }

    public interface ISparkViewRenderer
    {
        void Render();
    }

    public class SparkViewRenderer : ISparkViewRenderer
    {
        private readonly IPartialRenderAction _partialAction;
        private readonly IRenderAction _renderAction;
        private readonly RenderContext _renderContext;

        public SparkViewRenderer(IPartialRenderAction partialAction, IRenderAction renderAction, RenderContext renderContext)
        {
            _partialAction = partialAction;
            _renderAction = renderAction;
            _renderContext = renderContext;
        }

        public void Render()
        {
            if (_renderContext.IsPartial())
            {
                _partialAction.Execute();
            }
            else
            {
                _renderContext.SetAsPartial();
                _renderAction.Execute();
            }
        }
    }
}