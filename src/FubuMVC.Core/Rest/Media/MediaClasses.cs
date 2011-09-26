using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using FubuCore;

namespace FubuMVC.Core.Rest.Media
{
    public interface IMediaWriter<T>
    {
        IEnumerable<string> Mimetypes { get; }
        void Write(IValues<T> source, IOutputWriter writer);
        void Write(T source, IOutputWriter writer);
    }


    public class MediaDefinition<T>
    {

    }

    public interface IMediaDocument
    {
        IMediaNode Root { get; }

        IEnumerable<string> Mimetypes { get; }
        void Write(IOutputWriter writer);
    }

    public interface IMediaReaderNode
    {
        Type InputType { get; }
        ObjectDef ToObjectDef();
    }

    public interface IMediaWriterNode : IContainerModel
    {
        Type InputType { get; }
    }

    public class MediaWriterNode : IMediaWriterNode
    {
        private readonly Type _inputType;

        public MediaWriterNode(Type inputType)
        {
            _inputType = inputType;
        }



        public ObjectDef ToObjectDef()
        {
            throw new NotImplementedException();
        }

        public Type InputType
        {
            get { return _inputType; }
        }
    }

    public class MediaDependency
    {
        private readonly Type _openInterfaceType;
        private readonly Type _inputType;
        private readonly Type _interface;
        private IDependency _dependency;

        public MediaDependency(Type openInterfaceType, Type inputType)
        {
            _openInterfaceType = openInterfaceType;
            _inputType = inputType;

            _interface = openInterfaceType.MakeGenericType(inputType);
        }

        public void SetType(Type type)
        {
            if (!type.IsConcrete() || !type.CanBeCastTo(_interface))
            {
                throw new ArgumentException("Type {0} cannot be plugged into {1}".ToFormat(type.FullName, _interface.FullName));
            }

            _dependency = new ConfiguredDependency(_interface, type);
        }

        public void SetType<T>()
        {
            SetType(typeof(T));
        }

        public void SetValue(object value)
        {
            if (!value.GetType().CanBeCastTo(_interface))
            {
                throw new ArgumentException("Type {0} cannot be plugged into {1}".ToFormat(value.GetType().FullName, _interface.FullName));
            }

            _dependency = new ValueDependency(_interface, value);
        }

        public IDependency Dependency
        {
            get { return _dependency; }
        }
    }
}