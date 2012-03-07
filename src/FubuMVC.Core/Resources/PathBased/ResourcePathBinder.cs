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
            var path = FindPath(context.Service<IRequestData>());
            object instance = Activator.CreateInstance(type, path);

            // Setting additional properties
            // TODO -- have this delegate to a new method on BindingContext instead
            context.BindProperties(instance);

            return instance;
        }

        public static string FindPath(IRequestData dictionary)
        {
            var routeData = dictionary.ValuesFor(RequestDataSource.Route);
            var list = new List<string>();

            for (var i = 0; i < 10; i++)
            {
                routeData.Value("Part" + i, o => list.Add(o.RawValue.ToString()));
            }

            return list.Join("/");
        }
    }
}