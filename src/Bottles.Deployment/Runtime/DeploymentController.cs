using System;
using System.Collections.Generic;
using FubuCore.Util;
using StructureMap;
using FubuCore;
using System.Linq;

namespace Bottles.Deployment.Runtime
{
    public interface IDeploymentController
    {
        
    }

    public class DeploymentController : IDeploymentController
    {
        
    }

    public class DeployerFactory
    {
        
    }

    public class DirectiveBuilder
    {
        private readonly IContainer _container;
        private readonly Cache<string, Type> _directiveTypes = new Cache<string, Type>();

        public DirectiveBuilder(IContainer container)
        {
            _container = container;
            _container.Model.PluginTypes
                .Select(x => x.PluginType.FindInterfaceThatCloses(typeof (IDeployer<>)))
                .Where(x => x != null)
                .Each(type =>
                {
                    // TODO -- need to blow up if duplicate names are hit?
                    var directiveType = type.GetGenericArguments().First();
                    _directiveTypes[directiveType.Name] = directiveType;
                });
        }

        public Type DirectiveTypeFor(string name)
        {
            return _directiveTypes[name];
        }

        public IEnumerable<Type> DirectiveTypes()
        {
            return _directiveTypes.GetAll();
        }

        public IEnumerable<IDirective> BuildDirectives(HostManifest host)
        {
            return host.UniqueDirectiveNames().Select(name =>
            {
                var directiveType = _directiveTypes[name];
                return host.GetDirective(directiveType);
            });
        }
    }
}