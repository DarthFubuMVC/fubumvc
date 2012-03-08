using System;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public class ConditionAdded : NodeEvent
    {
        private readonly string _description;
        private readonly Type _type;

        public ConditionAdded(string description)
        {
            _description = description;
        }

        public ConditionAdded(Type type)
        {
            _type = type;
        }

        public string Description
        {
            get { return _description; }
        }

        public Type Type
        {
            get {
                return _type;
            }
        }
    }
}