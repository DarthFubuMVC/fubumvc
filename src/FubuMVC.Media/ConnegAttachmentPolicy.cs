using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Media
{
    public class ConnegAttachmentPolicy : IConfigurationAction
    {
        private readonly TypePool _types;
        private readonly IList<AttachmentFilter> _filters = new List<AttachmentFilter>();

        public ConnegAttachmentPolicy(TypePool types)
        {
            _types = types;
        }

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

            var connegGraph = new ConnegGraph(graph);

            // TODO -- add some config logging here
            _types
                .TypesMatching(x => x.IsConcreteTypeOf<IResourceRegistration>() && !x.IsOpenGeneric())
                .Each(t => t.Create<IResourceRegistration>().Modify(connegGraph, graph));
        }

        public class AttachmentFilter
        {
            public string Description { get; set; }
            public Func<BehaviorChain, bool> Filter { get; set; }
        }
    }


}