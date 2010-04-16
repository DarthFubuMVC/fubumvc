using System;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Registration.Nodes
{
    public class RenderJsonNode : OutputNode
    {
        private readonly Type _modelType;

        public RenderJsonNode(Type modelType)
            : base(typeof (RenderJsonBehavior<>).MakeGenericType(modelType))
        {
            _modelType = modelType;
        }

        public Type ModelType { get { return _modelType; } }
        public override string Description { get { return "Json"; } }

        public bool Equals(RenderJsonNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._modelType, _modelType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RenderJsonNode)) return false;
            return Equals((RenderJsonNode) obj);
        }

        public override int GetHashCode()
        {
            return (_modelType != null ? _modelType.GetHashCode() : 0);
        }
    }
}