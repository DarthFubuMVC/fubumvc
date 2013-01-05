using System;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Diagnostics;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration
{
    public abstract class ServiceEvent : NodeEvent, DescribesItself
    {
        private readonly Type _serviceType;
        private readonly ObjectDef _def;

        protected ServiceEvent(Type serviceType, ObjectDef def)
        {
            _serviceType = serviceType;
            _def = def;
        }

        public string RegistrationSource { get; set; }

        public Type ServiceType
        {
            get { return _serviceType; }
        }

        public ObjectDef Def
        {
            get { return _def; }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_serviceType != null ? _serviceType.GetHashCode() : 0) * 397) ^ (_def != null ? _def.GetHashCode() : 0);
            }
        }



        void DescribesItself.Describe(Description description)
        {
            description.Title = title();
            description.Properties["Service Type"] = _serviceType.FullName;
            description.Children["ObjectDef"] = Description.For(_def);
        }

        protected abstract string title();
    }
}