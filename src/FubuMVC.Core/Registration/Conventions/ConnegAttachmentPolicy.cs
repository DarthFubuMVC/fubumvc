using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;
using FubuMVC.Core.Rest.Conneg;

namespace FubuMVC.Core.Registration.Conventions
{
    public class ConnegAttachmentPolicy : IConfigurationAction
    {
        private readonly IList<AttachmentFilter> _filters = new List<AttachmentFilter>();

        public void AddFilter(string description, Func<BehaviorChain, bool> filter)
        {
            _filters.Add(new AttachmentFilter(){
                Description = description,
                Filter = filter
            });
        }

        public void Configure(BehaviorGraph graph)
        {
            _filters.Each(filter =>
            {
                graph.Behaviors.Where(filter.Filter).ToList().Each(chain =>
                {
                    // TODO -- need to do better config logging here
                    chain.ApplyConneg();
                });
            });
        }

        public class AttachmentFilter
        {
            public string Description { get; set; }
            public Func<BehaviorChain, bool> Filter { get; set; }
        }
    }


}