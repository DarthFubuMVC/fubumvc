using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core
{
    [CanBeMultiples]
    public class RegistryImport : IConfigurationAction
    {
        public string Prefix { get; set; }
        public FubuRegistry Registry { get; set; }

        public Type Type { get; set; }

        public void Configure(BehaviorGraph graph)
        {
            ImportInto(graph, graph.Views);
        }

        public void ImportInto(IChainImporter graph, ViewBag views)
        {
            // TODO -- will want this to suck in the configuration log business somehow
            Registry.Compile();
            var childGraph = Registry.Configuration.BuildForImport(views);
            graph.Import(childGraph, b =>
            {
                b.PrependToUrl(Prefix);
                b.Origin = Registry.Name;
            });
        }

        public bool Equals(RegistryImport other)
        {
            if (Type == null) return false;

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Prefix, Prefix) && Equals(other.Type, Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RegistryImport)) return false;
            return Equals((RegistryImport) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Prefix != null ? Prefix.GetHashCode() : 0)*397) ^ (Type != null ? Type.GetHashCode() : 0);
            }
        }
    }
}