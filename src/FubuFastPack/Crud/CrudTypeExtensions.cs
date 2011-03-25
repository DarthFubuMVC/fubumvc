using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuFastPack.Crud
{
    public static class CrudTypeExtensions
    {
        public static bool IsCrud(this BehaviorChain chain)
        {
            return chain.Calls.Any(x => IsCrudController(x.HandlerType));
        }

        public static bool IsCrudController(this Type type)
        {
            return type.Closes(typeof(CrudController<,>));
        }

        public static Type GetEntityType(this Type type)
        {
            if (!type.IsCrudController()) return null;

            var @interface = type.FindInterfaceThatCloses(typeof(CrudController<,>));
            return @interface.GetGenericArguments()[0];
        }

        public static Type GetEditEntityModelType(this Type type)
        {
            if (!type.IsCrudController()) return null;

            try
            {
                var @interface = type.FindInterfaceThatCloses(typeof(CrudController<,>));
                return @interface.GetGenericArguments()[1];
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error trying to find the EditEntityModel for " + type.FullName, e);
            }
        }

        public static Type GetCrudEntityType(this BehaviorChain chain)
        {
            return chain.Calls.First().HandlerType.GetEntityType();
        }
    }
}