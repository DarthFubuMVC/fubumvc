using System;
using FubuCore.Reflection;

namespace FubuMVC.Core.UI.Elements
{
    public interface IElementNamingConvention
    {
        string GetName(Type modelType, Accessor accessor);
    }
}