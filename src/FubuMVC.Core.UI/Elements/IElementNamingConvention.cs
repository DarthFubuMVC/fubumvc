using System;
using FubuCore.Reflection;

namespace FubuHtml.Elements
{
    public interface IElementNamingConvention
    {
        string GetName(Type modelType, Accessor accessor);
    }
}