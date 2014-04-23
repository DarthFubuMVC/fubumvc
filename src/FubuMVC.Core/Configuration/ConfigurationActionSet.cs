using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Configuration
{
    public class ConfigurationActionSet
    {
        private readonly IList<IConfigurationAction> _actions = new List<IConfigurationAction>();

        public IEnumerable<IConfigurationAction> Actions
        {
            get { return _actions; }
        }

        public void Fill(IConfigurationAction action)
        {
            Type actionType = action.GetType();


            if (TypeIsUnique(actionType) && _actions.Any(x => x.GetType() == actionType))
            {
                return;
            }

            _actions.Fill(action);
        }

        public static bool TypeIsUnique(Type type)
        {
            if (type.HasAttribute<CanBeMultiplesAttribute>()) return false;

            // If it does not have any non-default constructors
            if (type.GetConstructors().Any(x => x.GetParameters().Any()))
            {
                return false;
            }

            if (type.GetProperties().Any(x => x.CanWrite))
            {
                return false;
            }

            return true;
        }

        public virtual void RunActions(BehaviorGraph graph)
        {
            _actions.Each(x => x.Configure(graph));
        }

        public void Import(ConfigurationActionSet others)
        {
            others._actions.Each(Fill);
        }
    }

}