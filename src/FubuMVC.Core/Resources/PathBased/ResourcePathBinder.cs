using System;
using System.Collections.Generic;
using System.ComponentModel;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Resources.PathBased
{
    [Description("Binds models that inherit from ResourcePath for n-deep paths")]
    public class ResourcePathBinder : IModelBinder
    {
        public bool Matches(Type type)
        {
            return type.CanBeCastTo<ResourcePath>();
        }

        public void BindProperties(Type type, object instance, IBindingContext context)
        {
            throw new NotSupportedException();
        }

        public object Bind(Type type, IBindingContext context)
        {
            var path = FindPath(context.Service<IRequestData>());
            var instance = Activator.CreateInstance(type, path);

            // Setting additional properties
            // TODO -- have this delegate to a new method on BindingContext instead
            context.BindProperties(instance);    // START HERE AFTER BASEBALL

            return instance;
        }

        public static string FindPath(IRequestData dictionary)
        {
            var routeData = dictionary.ValuesFor(RequestDataSource.Route);
            var list = new List<string>();

            for (var i = 0; i < 10; i++)
            {
                routeData.Value("Part" + i, o =>
                {
                    if (o.RawValue != null) list.Add(o.RawValue.ToString());
                });
            }

            return list.Join("/");
        }
    }
}