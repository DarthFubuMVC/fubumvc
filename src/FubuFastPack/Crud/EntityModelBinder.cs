using System;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuFastPack.Domain;

namespace FubuFastPack.Crud
{
    public class EntityModelBinder : IModelBinder
    {
        public bool Matches(Type type)
        {
            return type.CanBeCastTo<DomainEntity>() && !type.HasAttribute<IgnoreEntityInBindingAttribute>();
        }

        public void Bind(Type type, object instance, IBindingContext context)
        {
            throw new NotSupportedException();
        }

        public object Bind(Type type, IBindingContext context)
        {
            var entity = context.ValueAs(type, "Id");

            if (entity != null) return entity;

            if (type.IsConcrete())
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}