using FubuCore;

namespace FubuMVC.Core.Assets.Diagnostics
{
    public class AssetRegistrationDiagnostics : IAssetRegistration
    {
        private readonly IAssetRegistration _inner;
        private readonly AssetLogsCache _logs;
        private string _provenance;

        public AssetRegistrationDiagnostics(IAssetRegistration inner, AssetLogsCache logs)
        {
            _inner = inner;
            _logs = logs;
        }

        public void Alias(string name, string alias)
        {
            _logs.FindByName(name)
                .Add(_provenance, "Added alias {0}".ToFormat(alias));

            _logs.FindByName(alias)
                .Add(_provenance, "Is an alias for {0}".ToFormat(name));

            _inner.Alias(name, alias);
        }

        public void Dependency(string dependent, string dependency)
        {
            _logs.FindByName(dependent)
                .Add(_provenance, "requires {0}".ToFormat(dependency));

            _logs.FindByName(dependency)
                .Add(_provenance, "supports {0}".ToFormat(dependent));

            _inner.Dependency(dependent, dependency);
        }

        public void Extension(string extender, string @base)
        {
           _logs.FindByName(@base)
                .Add(_provenance, "Extending with {0}".ToFormat(extender));

            _logs.FindByName(extender)
                .Add(_provenance, "Extends {0}".ToFormat(@base));

            _inner.Extension(extender, @base);
        }

        public void AddToSet(string setName, string name)
        {
            _logs.FindByName(setName)
                .Add(_provenance, "Add {0} to the set".ToFormat(name));

            _logs.FindByName(name)
                .Add(_provenance, "Added to set {0}".ToFormat(setName));

            _inner.AddToSet(setName, name);
        }

        public void Preceeding(string beforeName, string afterName)
        {
            _logs.FindByName(beforeName)
                .Add(_provenance, "putting {0} after".ToFormat(afterName));
            
            _logs.FindByName(afterName)
                .Add(_provenance, "putting {0} before".ToFormat(beforeName));

            _inner.Preceeding(beforeName, afterName);
        }

        public void AddToCombination(string comboName, string names)
        {
            _logs.FindByName(comboName)
                .Add(_provenance, "Combining {0} into me.".ToFormat(names));

            _logs.FindByName(names)
                .Add(_provenance, "was combined into {0}".ToFormat(comboName));

            _inner.AddToCombination(comboName, names);
        }

        public void ApplyPolicy(string typeName)
        {
            _logs.FindByName(typeName)
                .Add(_provenance, "Applying policy {0}".ToFormat(typeName));

            _inner.ApplyPolicy(typeName);
        }

        public void SetCurrentProvenance(string provenance)
        {
            _provenance = provenance;
        }
    }
}
