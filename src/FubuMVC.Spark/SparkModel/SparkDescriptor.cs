using System;
using System.Collections.Generic;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.Registration;
using FubuMVC.Spark.Rendering;
using Spark;

namespace FubuMVC.Spark.SparkModel
{
    public class SparkDescriptor : ViewDescriptor<ITemplate>
    {
        private readonly Lazy<ViewDefinition> _definition; 
        private readonly IList<ITemplate> _bindings = new List<ITemplate>();
        private readonly WatchedSparkEntry _viewEntry;
        private readonly WatchedSparkEntry _partialViewEntry; 
        
        public SparkDescriptor(ITemplate template, ISparkViewEngine engine) : base(template)
        {
            _definition = new Lazy<ViewDefinition>(this.ToViewDefinition);
            _viewEntry = new WatchedSparkEntry(() => engine.CreateEntry(_definition.Value.ViewDescriptor));
            _partialViewEntry = new WatchedSparkEntry(() => engine.CreateEntry(_definition.Value.PartialDescriptor));

        }

        public ViewDefinition Definition
        {
            get
            {
                return _definition.Value;
            }
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
    }

    public class WatchedSparkEntry
    {
        private readonly Func<ISparkViewEntry> _source;
        private ISparkViewEntry _entry;

        public WatchedSparkEntry(Func<ISparkViewEntry> source)
        {
            _source = source;
        }

        public void Precompile()
        {
            if (_entry == null)
            {
                _entry = _source();
            }
        }

        public ISparkViewEntry Value
        {
            get
            {
                if (_entry == null || !_entry.IsCurrent())
                {
                    _entry = _source();
                }

                return _entry;
            }
        }
    }
}