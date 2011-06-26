using System;
using System.Collections;
using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics
{
    public class ModelBindingReport : TimedReport, IEnumerable<IModelBindingDetail>, IBehaviorDetails
    {
        public Type BoundType { get; set; }
        public object StoredObject { get; set; }

        private readonly IList<IModelBindingDetail> _bindings = new List<IModelBindingDetail>();
        public IEnumerator<IModelBindingDetail> GetEnumerator()
        {
            return _bindings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IModelBindingDetail binding)
        {
            _bindings.Add(binding);
        }

        public void AcceptVisitor(IBehaviorDetailsVisitor visitor)
        {
            visitor.ModelBinding(this);
        }
    }
}