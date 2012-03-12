using System.Collections.Generic;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public class SparkDescriptor : ViewDescriptor<ITemplate>
    {
        private readonly IList<ITemplate> _bindings = new List<ITemplate>();
        public SparkDescriptor(ITemplate template) : base(template)
        {
        }

        public void AddBinding(ITemplate template) { _bindings.Add(template); }
        public IEnumerable<ITemplate> Bindings { get { return _bindings; } }
    }
}