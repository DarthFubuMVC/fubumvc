using System;
using System.Collections.Generic;
using HtmlTags;

namespace FubuMVC.Diagnostics.Features.Chains
{
    public class BehaviorModel
    {
        public BehaviorModel()
        {
            Logs = new List<string>();
        }

        public Guid Id { get; set; }
        public string BehaviorType { get; set; }
        public string DisplayType { get; set; }
        public IEnumerable<string> Logs { get; set; }
        public HtmlTag BehaviorLabel { get; set; }
    }
}