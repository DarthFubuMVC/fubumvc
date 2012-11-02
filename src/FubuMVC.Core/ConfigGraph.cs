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
        private readonly Cache<string, IList<ActionLog>> _configurations
            = new Cache<string, IList<ActionLog>>(x => new List<ActionLog>());

        private readonly IList<RegistryImport> _imports = new List<RegistryImport>();
        private readonly Stack<Provenance> _provenanceStack = new Stack<Provenance>();
        private readonly IList<IServiceRegistry> _services = new List<IServiceRegistry>();

        public IEnumerable<Provenance> ProvenanceStack
        {
            get { return _provenanceStack; }
        }

        public void RunActions(string configurationType)
        {
            throw new NotImplementedException();
        }

        public void Push(FubuRegistry registry)
        {
            throw new NotImplementedException();
        }

        // This'll do all the work of getting the registrations out of the bottle
        public void Add(IPackageInfo bottle)
        {
            throw new NotImplementedException();
        }

        public void Pop()
        {
            throw new NotImplementedException();
        }

        // TODO -- this has to be idempotent!!!!
        public void Add(ConfigurationPack pack)
        {
            throw new NotImplementedException();
        }

        public void AddImport(RegistryImport import)
        {
            if (HasImported(import.Registry)) return;

            _imports.Add(import);
        }

        public bool HasImported(FubuRegistry registry)
        {
            if (_imports.Any(x => x.Registry.GetType() == registry.GetType()))
            {
                return true;
            }

            // TODO -- this will have to change
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

            throw new NotImplementedException();
            //_configurations[type].FillAction(action); 
        }

//        internal static void FillAction<T>(this IList<T> actions, T action)
//        {
//            throw new NotImplementedException();
//
//            var actionType = action.GetType();
//
//
//            if (TypeIsUnique(actionType) && actions.Any(x => x.GetType() == actionType))
//            {
//                return;
//            }
//
//            actions.Fill(action);
//        }


        public void Add(IServiceRegistry services)
        {
            _services.Add(services);
        }

        public IEnumerable<IServiceRegistry> AllServiceRegistrations()
        {
            foreach (RegistryImport import in UniqueImports())
            {
                foreach (IServiceRegistry registry in import.Registry.Config.AllServiceRegistrations())
                {
                    yield return registry;
                }
            }

            foreach (IServiceRegistry registry in _services)
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