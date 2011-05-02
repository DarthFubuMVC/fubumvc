using System;
using FubuMVC.Core.View;

namespace FubuMVC.WebForms
{
    public class PartialViewTypeBindingExpression : IPartialViewTypeBindingExpression
    {
        private readonly IPartialViewTypeRegistry _typeRegistry;
        private readonly Type _modelType;

        public PartialViewTypeBindingExpression(IPartialViewTypeRegistry typeRegistry, Type modelType)
        {
            _typeRegistry = typeRegistry;
            _modelType = modelType;
        }

        public void Use<TPartialView>()
            where TPartialView : IFubuPage
        {
            _typeRegistry.Register(_modelType, typeof(TPartialView));
        }
    }
}