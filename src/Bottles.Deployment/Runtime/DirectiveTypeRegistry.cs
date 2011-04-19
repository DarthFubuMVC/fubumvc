using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using StructureMap;


namespace Bottles.Deployment.Runtime
{
    public class DirectiveTypeRegistry : IDirectiveTypeRegistry
    {
        private readonly Cache<string, Type> _directiveTypes = new Cache<string, Type>();

        public DirectiveTypeRegistry(IContainer container)
        {
            container.Model.PluginTypes
                .Select(x => x.PluginType.FindInterfaceThatCloses(typeof (IDeployer<>)))
                .Where(x => x != null).Each(type =>
                {
                    // TODO -- need to blow up if duplicate names are hit?
                    var directiveType = Enumerable.First<Type>(type.GetGenericArguments());
                    _directiveTypes[directiveType.Name] = directiveType;
                });
        }

        public Type DirectiveTypeFor(string name)
        {
            // TODO -- blow up if the directive type cannot be found!
            return _directiveTypes[name];
        }

        public IEnumerable<Type> DirectiveTypes()
        {
            return _directiveTypes.GetAll();
        }


    }
}