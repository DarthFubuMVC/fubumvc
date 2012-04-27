using System;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuMVC.Diagnostics.Features.Html.Preview
{
    public class PreviewModelTypeResolver : IPreviewModelTypeResolver
    {
        private readonly BehaviorGraph _graph;

        public PreviewModelTypeResolver(BehaviorGraph graph)
        {
            _graph = graph;
        }

        public Type TypeFor(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type; // this is really only going to work for internals

            return _graph.Behaviors
                .Select(x => x.ResourceType())
                .FirstOrDefault(x => x != null && x.FullName == typeName);
        }
    }
}