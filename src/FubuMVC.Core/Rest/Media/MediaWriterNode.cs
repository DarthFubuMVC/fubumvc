using System;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Rest.Media.Projections;

namespace FubuMVC.Core.Rest.Media
{
    public class MediaWriterNode : IMediaWriterNode
    {
        private readonly Type _inputType;

        public MediaWriterNode(Type inputType)
        {
            _inputType = inputType;

            Links = new MediaDependency(typeof (ILinkSource<>), _inputType);
            Projection = new MediaDependency(typeof (IValueProjection<>), _inputType);
            Document = new MediaDependency(typeof (IMediaDocument));
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