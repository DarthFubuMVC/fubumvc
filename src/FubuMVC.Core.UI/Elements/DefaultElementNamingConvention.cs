using System;
using FubuCore.Reflection;

namespace FubuHtml.Elements
{
    public class DefaultElementNamingConvention : IElementNamingConvention
    {
        public string GetName(Type modelType, Accessor accessor)
        {
            return accessor.Name;
        }
    }
}