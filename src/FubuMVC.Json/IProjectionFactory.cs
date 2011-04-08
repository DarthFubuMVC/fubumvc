using System;

namespace FubuMVC.Json
{
    public interface IProjectionFactory
    {
        IProjection ProjectionFor(Type type);
        IProjection ProjectionFor(Type type, string name);
    }
}