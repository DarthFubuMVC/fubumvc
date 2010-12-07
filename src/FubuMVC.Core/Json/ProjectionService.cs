using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Json
{
    public class ProjectionService
    {
        private readonly IProjectionFactory _factory;
        private readonly ITypeResolver _typeResolver;

        public ProjectionService(IProjectionFactory factory, ITypeResolver typeResolver)
        {
            _factory = factory;
            _typeResolver = typeResolver;
        }

        public Cache<string, object> Flatten(object target)
        {
            var type = _typeResolver.ResolveType(target);
            return _factory.ProjectionFor(type).Flatten(target);
        }

        public Cache<string, object> Flatten(object target, string name)
        {
            var type = _typeResolver.ResolveType(target);
            return _factory.ProjectionFor(type, name).Flatten(target);
        }
    }
}