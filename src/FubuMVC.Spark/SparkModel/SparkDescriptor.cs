using System;
using System.Collections.Generic;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.Rendering;
using Spark;

namespace FubuMVC.Spark.SparkModel
{
    public class SparkDescriptor : ViewDescriptor<ITemplate>
    {
        private readonly Lazy<SparkViewDescriptor> _full; 
        private readonly Lazy<SparkViewDescriptor> _partial; 
        private readonly IList<ITemplate> _bindings = new List<ITemplate>();
        private readonly WatchedSparkEntry _viewEntry;
        private readonly WatchedSparkEntry _partialViewEntry; 
        
        public SparkDescriptor(ITemplate template, ISparkViewEngine engine) : base(template)
        {
            _full = new Lazy<SparkViewDescriptor>(() => createSparkDescriptor(true));
            _partial = new Lazy<SparkViewDescriptor>(() => createSparkDescriptor(false));

            _viewEntry = new WatchedSparkEntry(() => engine.CreateEntry(_full.Value));
            _partialViewEntry = new WatchedSparkEntry(() => engine.CreateEntry(_partial.Value));

        }

        public void AddBinding(ITemplate template) { _bindings.Add(template); }
        public IEnumerable<ITemplate> Bindings { get { return _bindings; } }


        public ISparkViewEntry ViewEntry
        {
            get
            {
                return _viewEntry.Value;
            }
        }

        public ISparkViewEntry PartialViewEntry
        {
            get
            {
                return _partialViewEntry.Value;
            }
        }

        public void Precompile()
        {
            _viewEntry.Precompile();
            _partialViewEntry.Precompile();
        }




        private SparkViewDescriptor createSparkDescriptor(bool useMaster)
        {
            var sparkDescriptor = new SparkViewDescriptor().AddTemplate(ViewPath);
            if (useMaster && Master != null)
            {
                appendMasterPage(sparkDescriptor, Master);
            }

            return sparkDescriptor;
        }

        private static void appendMasterPage(SparkViewDescriptor descriptor, ITemplate template)
        {
            if (template == null)
            {
                return;
            }
            descriptor.AddTemplate(template.ViewPath);
            var viewDescriptor = template.Descriptor as SparkDescriptor;
            if (viewDescriptor != null)
            {
                appendMasterPage(descriptor, viewDescriptor.Master);
            }
        }
    }
}