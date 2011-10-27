using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Resources.PathBased
{
    public class ResourcePathBinder : IModelBinder
    {
        public bool Matches(Type type)
        {
            return type.CanBeCastTo<ResourcePath>();
        }

        public void Bind(Type type, object instance, IBindingContext context)
        {
            throw new NotSupportedException();
        }

        public object Bind(Type type, IBindingContext context)
        {
            var path = FindPath(context.Service<AggregateDictionary>());
            return Activator.CreateInstance(type, path);
        }

        public static string FindPath(AggregateDictionary dictionary)
        {
            var routeData = dictionary.DataFor(RequestDataSource.Route.ToString());
            var list = new List<string>();

            for (var i = 0; i < 10; i++)
            {
                routeData.Value("Part" + i, o => list.Add(o.ToString()));
            }

            return list.Join("/");
        }
    }
}