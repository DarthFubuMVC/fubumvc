using System;
using System.Collections.Generic;
using System.Linq;
using Bottles;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Diagnostics;

namespace FubuMVC.Core
{
    public class ConfigGraph
    {
        private readonly Cache<string, ConfigurationActionSet> _configurations
            = new Cache<string, ConfigurationActionSet>(x => new ConfigurationActionSet(x));

        private readonly IList<RegistryImport> _imports = new List<RegistryImport>();
        private readonly Stack<Provenance> _provenanceStack = new Stack<Provenance>();
        private readonly IList<ServiceRegistry> _services = new List<ServiceRegistry>();

        public IEnumerable<Provenance> ProvenanceStack
        {
            get { return _provenanceStack.Reverse(); }
        }

        public void RunActions(string configurationType, BehaviorGraph graph)
        {
            _configurations[configurationType].RunActions(graph);
        }

        public void Push(FubuRegistry registry)
        {
            _provenanceStack.Push(new FubuRegistryProvenance(registry));
        }

        public void Push(IPackageInfo bottle)
        {
            _provenanceStack.Push(new BottleProvenance(bottle));
        }

        public void Push(IFubuRegistryExtension extension)
        {
            _provenanceStack.Push(new FubuRegistryExtensionProvenance(extension));
        }

        public void Pop()
        {
            _provenanceStack.Pop();
        }

        public IEnumerable<ActionLog> LogsFor(string configurationType)
        {
            return _configurations[configurationType].Logs;
        } 

        public IEnumerable<IConfigurationAction> ActionsFor(string configurationType)
        {
            return _configurations[configurationType].Actions;
        } 

        public void Add(ConfigurationPack pack)
        {
            _provenanceStack.Clear(); // may regret doing this later

            _provenanceStack.Push(new ConfigurationPackProvenance(pack));

            pack.WriteTo(this);

            _provenanceStack.Pop();
        }

        public void AddImport(RegistryImport import)
        {
            if (HasImported(import.Registry)) return;

            _imports.Add(import);
        }

        // Tested through the FubuRegistry.Import()
        public bool HasImported(FubuRegistry registry)
        {
            if (_imports.Any(x => x.Registry.GetType() == registry.GetType()))
            {
                return true;
            }

            if (_imports.Any(x => x.Registry.Config.HasImported(registry)))
            {
                return true;
            }

            return false;
        }

        public IEnumerable<RegistryImport> UniqueImports()
        {
            List<RegistryImport> children = allChildrenImports().ToList();

            return _imports.Where(x => !children.Contains(x));
        }

        private IEnumerable<RegistryImport> allChildrenImports()
        {
            foreach (RegistryImport import in _imports)
            {
                foreach (RegistryImport action in import.Registry.Config._imports)
                {
                    yield return action;

                    foreach (
                        RegistryImport descendentAction in
                            _imports.SelectMany(x => x.Registry.Config.allChildrenImports()))
                    {
                        yield return descendentAction;
                    }
                }
            }
        }

        public void Add(IConfigurationAction action, string configurationType = null)
        {
            string type = DetermineConfigurationType(action) ?? configurationType;
            if (type == null)
            {
                throw new ArgumentOutOfRangeException(
                    "No Type specified and unable to determine what the configuration type for " +
                    action.GetType());
            }

            _configurations[type].Fill(ProvenanceStack, action); 
        }



        public void Add(ServiceRegistry services)
        {
            _services.Add(services);
        }

        public IEnumerable<IServiceRegistration> AllServiceRegistrations()
        {
            foreach (RegistryImport import in UniqueImports())
            {
                foreach (IServiceRegistration registry in import.Registry.Config.AllServiceRegistrations())
                {
                    yield return registry;
                }
            }

            foreach (IServiceRegistration registry in _services)
            {
                yield return registry;
            }
        }

        public static string DetermineConfigurationType(IConfigurationAction action)
        {
            if (action.GetType().HasAttribute<ConfigurationTypeAttribute>())
            {
                return action.GetType().GetAttribute<ConfigurationTypeAttribute>().Type;
            }

            return null;
        }
    }
}