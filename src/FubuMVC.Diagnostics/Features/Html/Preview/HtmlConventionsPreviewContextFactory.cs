using System;
using System.Collections.Generic;
using System.Reflection;

namespace FubuMVC.Diagnostics.Features.Html.Preview
{
    public class HtmlConventionsPreviewContextFactory : IHtmlConventionsPreviewContextFactory
    {
        private readonly IPreviewModelTypeResolver _typeResolver;
        private readonly IPreviewModelActivator _activator;

        public HtmlConventionsPreviewContextFactory(IPreviewModelTypeResolver typeResolver, IPreviewModelActivator activator)
        {
            _typeResolver = typeResolver;
            _activator = activator;
        }

        public HtmlConventionsPreviewContext BuildFromPath(string modelPath)
        {
            var propertyPath = new List<string>(modelPath.Split('-'));
            var rootModelTypeName = propertyPath[0];
            propertyPath.RemoveAt(0);

            Type modelType = _typeResolver.TypeFor(rootModelTypeName);
            var modelInstance = _activator.Activate(modelType);

            var propertyChainParts = new List<PropertyInfo>();
            while (propertyPath.Count > 0)
            {
                var parentPropertyInfo = modelType.GetProperty(propertyPath[0]);
                propertyChainParts.Add(parentPropertyInfo);
                var currentModelType = parentPropertyInfo.PropertyType;
                var currentModel = _activator.Activate(currentModelType);
                setProperty(parentPropertyInfo, modelInstance, currentModel);

                modelType = currentModelType;
                modelInstance = currentModel;
                propertyPath.RemoveAt(0);
            }

            return new HtmlConventionsPreviewContext(modelPath, modelType, modelInstance, propertyChainParts);
        }

        private static void setProperty(PropertyInfo property, object instance, object propertyValue)
        {
            var setMethod = property.GetSetMethod();
            setMethod.Invoke(instance, new[] { propertyValue });
        }
    }
}