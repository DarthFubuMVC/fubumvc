using System;
using System.Collections;
using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics
{
    public class ModelBindingReport : TimedReport, IEnumerable<ModelBindingKey>, IBehaviorDetails
    {
        public Type BoundType { get; set; }
        public object StoredObject { get; set; }

        private readonly IList<ModelBindingKey> _bindings = new List<ModelBindingKey>();
        public IEnumerator<ModelBindingKey> GetEnumerator()
        {
            return _bindings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ModelBindingKey binding)
        {
            _bindings.Add(binding);
        }

        public void AcceptVisitor(IBehaviorDetailsVisitor visitor)
        {
            visitor.ModelBinding(this);
        }
    }
}