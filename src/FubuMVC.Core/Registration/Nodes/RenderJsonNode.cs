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
    }
}