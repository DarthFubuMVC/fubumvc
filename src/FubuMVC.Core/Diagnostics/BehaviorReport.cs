using System;
using System.Collections;
using System.Collections.Generic;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Diagnostics
{
    public class BehaviorReport : TimedReport, IEnumerable<IBehaviorDetails>
    {
        private readonly IList<IBehaviorDetails> _details = new List<IBehaviorDetails>();
        private ModelBindingReport _lastBinding;

        public BehaviorReport(IActionBehavior behavior)
        {
            Description = behavior.ToString();
            BehaviorType = behavior.GetType();
        }

        public Type BehaviorType { get; set; }
        public string Description { get; private set; }

        public IEnumerator<IBehaviorDetails> GetEnumerator()
        {
            return _details.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddDetail(IBehaviorDetails detail)
        {
            _details.Add(detail);
        }

        public void StartModelBinding(Type type)
        {
            _lastBinding = new ModelBindingReport
            {
                BoundType = type
            };

            AddDetail(_lastBinding);
        }

        public void AddModelBindingKey(ModelBindingKey key)
        {
            if (_lastBinding != null) _lastBinding.Add(key);
        }

        public void EndModelBinding()
        {
            if (_lastBinding != null) _lastBinding.MarkFinished();
        }
    }
}