using System;
using System.Collections.Generic;
using System.Linq;
using Bottles;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Diagnostics;

namespace FubuMVC.Core.Configuration
{
    /// <summary>
    /// Holds and tracks all the IConfigurationAction's used to construct the BehaviorGraph of an application
    /// </summary>
    public class ConfigGraph
    {
        private readonly Cache<string, ConfigurationActionSet> _configurations
            = new Cache<string, ConfigurationActionSet>(x => new ConfigurationActionSet(x));

        private readonly IList<RegistryImport> _imports = new List<RegistryImport>();
        private ProvenanceChain _currentProvenance = new ProvenanceChain(new Provenance[0]);
        private readonly IList<ServiceRegistryLog> _services = new List<ServiceRegistryLog>();

        public ConfigGraph()
        {
            _configurations[ConfigurationType.Discovery] = new ActionSourceConfigurationActionSet();
        }

        /// <summary>
        /// All of the ActionLog's for this application
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ActionLog> AllLogs()
        {
            return _configurations.SelectMany(x => x.Logs);
        }

        public ProvenanceChain CurrentProvenance
        {
            get { return _currentProvenance; }
        }

        public IEnumerable<RegistryImport> Imports
        {
            get { return _imports; }
        }

        public void PrependProvenance(IEnumerable<Provenance> forebears)
        {
            _configurations.SelectMany(x => x.Logs).Select(x => x.ProvenanceChain).Distinct().Each(
                x => x.Prepend(forebears));
        }

        public void RunActions(string configurationType, BehaviorGraph graph)
        {
            _configurations[configurationType].RunActions(graph);
        }

        public void Push(FubuRegistry registry)
        {
            _currentProvenance = (_currentProvenance ?? new ProvenanceChain(new Provenance[0])).Push(new FubuRegistryProvenance(registry));
        }

        public void Push(IPackageInfo bottle)
        {
            _currentProvenance = (_currentProvenance ?? new ProvenanceChain(new Provenance[0])).Push(new BottleProvenance(bottle));
        }

        public void Push(IFubuRegistryExtension extension)
        {
            _currentProvenance = (_currentProvenance ?? new ProvenanceChain(new Provenance[0])).Push(new FubuRegistryExtensionProvenance(extension));
        }

        public void Pop()
        {
            _currentProvenance = _currentProvenance.Pop();
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
            _currentProvenance = new ProvenanceChain(new Provenance[]{new ConfigurationPackProvenance(pack), });

            pack.WriteTo(this);
        }

        public void AddImport(RegistryImport import)
        {
            if (HasImported(import.Registry)) return;

            import.Provenance = CurrentProvenance;
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

            _configurations[type].Fill(_currentProvenance, action); 
        }



        public void Add(ServiceRegistry services)
        {
            _services.Add(new ServiceRegistryLog(services, CurrentProvenance));
        }

        public void Add(IActionSource source)
        {
            Add(new ActionSourceRunner(source), ConfigurationType.Discovery);
        }

        public void RegisterServices(ServiceGraph services)
        {
            AllServiceRegistrations().Each(x => x.Apply(services));
        }

        public IEnumerable<ServiceRegistryLog> AllServiceRegistrations()
        {
            foreach (RegistryImport import in UniqueImports())
            {
                foreach (ServiceRegistryLog log in import.Registry.Config.AllServiceRegistrations())
                {
                    yield return log;
                }
            }

            foreach (ServiceRegistryLog registry in _services)
            {
                yield return registry;
            }
        }

        public static string DetermineConfigurationType(IConfigurationAction action)
        {
            var knowsItself = action as IKnowMyConfigurationType;
            if (knowsItself != null) return knowsItself.DetermineConfigurationType();

            if (action.GetType().HasAttribute<ConfigurationTypeAttribute>())
            {
                return action.GetType().GetAttribute<ConfigurationTypeAttribute>().Type;
            }

            return null;
        }

        public IEnumerable<T> AllEvents<T>()
        {
            return _configurations.SelectMany(x => x.AllEvents<T>()).Union(_services.SelectMany(x => x.Events.OfType<T>()));
        }
        
        /// <summary>
        /// Honestly, this is 50% a HACK.  This just gives ConfigGraph a chance to apply the default endpoint action source
        /// if the FubuRegistry doesn't already have any
        /// </summary>
        public void Seal()
        {
            var actions = _configurations[ConfigurationType.Discovery];

            if (!actions.Logs.Any(x => x.Action is ActionSourceRunner))
            {
                _currentProvenance = new ProvenanceChain(new Provenance[]{new ConfigurationPackProvenance(new DiscoveryActionsConfigurationPack()), });
                Add(new EndpointActionSource());
            }

            Pop();
        }
    }
}