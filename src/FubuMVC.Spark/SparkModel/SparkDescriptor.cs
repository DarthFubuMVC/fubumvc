using System;
using System.Collections.Generic;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.Registration;
using FubuMVC.Spark.Rendering;

namespace FubuMVC.Spark.SparkModel
{
    public class SparkDescriptor : ViewDescriptor<ITemplate>
    {
        private readonly Lazy<ViewDefinition> _definition; 
        private readonly IList<ITemplate> _bindings = new List<ITemplate>();
        
        public SparkDescriptor(ITemplate template) : base(template)
        {
            _definition = new Lazy<ViewDefinition>(this.ToViewDefinition);
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
    }
}