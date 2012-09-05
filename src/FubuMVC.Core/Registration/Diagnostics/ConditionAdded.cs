using System;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public class ConditionAdded : NodeEvent, DescribesItself
    {
        private readonly string _description;
        private readonly Type _type;

        public ConditionAdded(string description)
        {
            _description = description;
        }

        public ConditionAdded(Type type) : this(type.Name)
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

        void DescribesItself.Describe(Description description)
        {
            description.Properties["Condition"] = _description;
        }
    }
}