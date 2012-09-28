using System;
using FubuCore.Reflection;

namespace FubuMVC.Core.UI.Elements
{
    public class DefaultElementNamingConvention : IElementNamingConvention
    {
        public string GetName(Type modelType, Accessor accessor)
        {
            return accessor.Name;
        }
    }
}