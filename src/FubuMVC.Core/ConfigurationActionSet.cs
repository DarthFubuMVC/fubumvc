using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Diagnostics;

namespace FubuMVC.Core
{
    public class ConfigurationActionSet
    {
        private readonly IList<ActionLog> _logs = new List<ActionLog>();
        private readonly string _configurationType;

        public ConfigurationActionSet(string configurationType)
        {
            _configurationType = configurationType;
        }

        public string ConfigurationType
        {
            get { return _configurationType; }
        }

        public IEnumerable<ActionLog> Logs
        {
            get { return _logs; }
        }

        public IEnumerable<IConfigurationAction> Actions
        {
            get { return _logs.Select(x => x.Action); }
        } 

        public void Fill(IEnumerable<Provenance> provenanceStack, IConfigurationAction action)
        {
            if (!provenanceStack.Any())
            {
                throw new ArgumentException("No provenance supplied!");
            }

            Type actionType = action.GetType();


            if (TypeIsUnique(actionType) && _logs.Any(x => x.Action.GetType() == actionType))
            {
                return;
            }

            _logs.Fill(new ActionLog(action, provenanceStack));
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

        public void RunActions(BehaviorGraph graph)
        {
            _logs.Each(x => x.RunAction(graph));
        }
    }
}