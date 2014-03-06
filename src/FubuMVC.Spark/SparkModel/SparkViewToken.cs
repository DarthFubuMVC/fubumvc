using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;
using Spark;

namespace FubuMVC.Spark.SparkModel
{
    public class SparkViewToken : IViewToken
    {
        private readonly Lazy<SparkViewDescriptor> _full;
        private readonly Lazy<SparkViewDescriptor> _partial;
        private readonly IList<ISparkTemplate> _bindings = new List<ISparkTemplate>();
        private readonly WatchedSparkEntry _viewEntry;
        private readonly WatchedSparkEntry _partialViewEntry;
        private readonly ISparkTemplate _template;

        public SparkViewToken(ISparkTemplate template, ISparkViewEngine engine)
        {
            _template = template;

            _full = new Lazy<SparkViewDescriptor>(() => createSparkDescriptor(true));
            _partial = new Lazy<SparkViewDescriptor>(() => createSparkDescriptor(false));

            _viewEntry = new WatchedSparkEntry(() => engine.CreateEntry(_full.Value));
            _partialViewEntry = new WatchedSparkEntry(() => engine.CreateEntry(_partial.Value));
        }

        public ISparkTemplate Template
        {
            get { return _template; }
        }

        public Type ViewType
        {
            get { return typeof(ISparkView); }
        }

        public Type ViewModel
        {
            get
            {
                return _template.ViewModel;
            }
        }

        public string Name()
        {
            return _template.Name();
        }

        public string Namespace
        {
            get { return _template.Namespace; }
        }

        public IRenderableView GetView()
        {
            return ViewEntry.CreateInstance().As<IRenderableView>();
        }

        public IRenderableView GetPartialView()
        {
            return PartialViewEntry.CreateInstance().As<IRenderableView>();
        }

        public string ProfileName { get; set; }

        public void AttachViewModels(ViewTypePool types, ITemplateLogger logger)
        {
            _template.AttachViewModels(types, logger);
        }

        public void AddBinding(ISparkTemplate template)
        {
            _bindings.Add(template);
        }

        public IEnumerable<ISparkTemplate> Bindings
        {
            get { return _bindings; }
        }


        public ISparkViewEntry ViewEntry
        {
            get { return _viewEntry.Value; }
        }

        public ISparkViewEntry PartialViewEntry
        {
            get { return _partialViewEntry.Value; }
        }

        public void Precompile()
        {
            _viewEntry.Precompile();
            _partialViewEntry.Precompile();
        }


        private SparkViewDescriptor createSparkDescriptor(bool useMaster)
        {
            var sparkDescriptor = new SparkViewDescriptor().AddTemplate(_template.ViewPath);
            if (useMaster && _template.Master != null)
            {
                appendMasterPage(sparkDescriptor, (ISparkTemplate) _template.Master);
            }

            return sparkDescriptor;
        }

        private static void appendMasterPage(SparkViewDescriptor descriptor, ISparkTemplate template)
        {
            if (template == null)
            {
                return;
            }

            descriptor.AddTemplate(template.ViewPath);
            
            appendMasterPage(descriptor, template.Master as ISparkTemplate);

            var viewDescriptor = template as ISparkTemplate;
            if (viewDescriptor != null)
            {
                appendMasterPage(descriptor, viewDescriptor);
            }
        }
    }
}