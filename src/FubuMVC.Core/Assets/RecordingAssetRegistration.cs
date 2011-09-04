using System;
using System.Collections.Generic;
using Bottles.Diagnostics;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets
{
    // TODO -- make this be intelligent enough to get it's own FubuRegistry name and try to figure
    // out provenance
    public class RecordingAssetRegistration : IAssetRegistration, IAssetPolicy
    {
        private readonly IList<Action<IAssetRegistration>> _registrations = new List<Action<IAssetRegistration>>();

        private Action<IAssetRegistration> push
        {
            set
            {
                _registrations.Add(value);
            }
        }

        public void Apply(IPackageLog log, IAssetPipeline pipeline, AssetGraph graph)
        {
            // TODO -- make this recording later?
            Replay(graph);
        }

        public void Replay(IAssetRegistration registration)
        {
            _registrations.Each(x => x(registration));
        }

        public void Alias(string name, string alias)
        {
            push = x => x.Alias(name, alias);
        }

        public void Dependency(string dependent, string dependency)
        {
            push = x => x.Dependency(dependent, dependency);
        }

        public void Extension(string extender, string @base)
        {
            push = x => x.Extension(extender, @base);
        }

        public void AddToSet(string setName, string name)
        {
            push = x => x.AddToSet(setName, name);
        }

        public void Preceeding(string beforeName, string afterName)
        {
            push = x => x.Preceeding(beforeName, afterName);
        }


    }
}