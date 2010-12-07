using System;

namespace FubuMVC.Core.Json
{
    public interface IProjectionFactory
    {
        IProjection ProjectionFor(Type type);
        IProjection ProjectionFor(Type type, string name);
    }
}