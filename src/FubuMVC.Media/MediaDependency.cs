using System;
using FubuCore;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Media
{
    public class MediaDependency
    {
        private readonly Type _interface;
        private IDependency _dependency;

        public MediaDependency(Type openInterfaceType, Type inputType)
        {
            _interface = openInterfaceType.MakeGenericType(inputType);
        }

        public MediaDependency(Type @interface)
        {
            _interface = @interface;
        }

        public IDependency Dependency
        {
            get { return _dependency; }
        }

        public ObjectDef UseType(Type type)
        {
            if (!type.IsConcrete() || !type.CanBeCastTo(_interface))
            {
                throw new ArgumentException("Type {0} cannot be plugged into {1}".ToFormat(type.FullName,
                                                                                           _interface.FullName));
            }

            var configuredDependency = new ConfiguredDependency(_interface, type);
            _dependency = configuredDependency;

            return configuredDependency.Definition;
        }

        public ObjectDef UseType<T>()
        {
            return UseType(typeof (T));
        }

        public void UseValue(object value)
        {
            if (!value.GetType().CanBeCastTo(_interface))
            {
                throw new ArgumentException("Type {0} cannot be plugged into {1}".ToFormat(value.GetType().FullName,
                                                                                           _interface.FullName));
            }

            _dependency = new ValueDependency(_interface, value);
        }
    }
}