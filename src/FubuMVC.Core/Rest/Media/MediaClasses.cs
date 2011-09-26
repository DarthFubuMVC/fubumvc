using System;
using System.Collections;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Rest.Media.Projections;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Rest.Media
{
    public interface IMediaWriter<T>
    {
        IEnumerable<string> Mimetypes { get; }
        void Write(IValues<T> source, IOutputWriter writer);
        void Write(T source, IOutputWriter writer);
    }

    // Fancier stuff?  Later?
    //public class MediaDefinition<T>
    //{

    //}

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

            Links = new MediaDependency(typeof(ILinkSource<>), _inputType);
            Projection = new MediaDependency(typeof(IValueProjection<>), _inputType);
            Document = new MediaDependency(typeof(IMediaDocument));
        }

        public MediaDependency Links { private set; get; }
        public MediaDependency Projection { private set; get; }
        public MediaDependency Document { private set; get; }

        public ObjectDef ToObjectDef()
        {
            var objectDef = new ObjectDef(typeof (MediaWriter<>).MakeGenericType(_inputType));
            if (Links.Dependency != null) objectDef.Dependency(Links.Dependency);
            if (Projection.Dependency != null) objectDef.Dependency(Projection.Dependency);
            if (Document.Dependency != null) objectDef.Dependency(Document.Dependency);

            return objectDef;
        }

        public Type InputType
        {
            get { return _inputType; }
        }
    }
}