using System;
using System.ComponentModel;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.Persistence;

namespace FubuFastPack.Crud
{
    public class EntityModelBinder : IModelBinder
    {
        private static readonly TypeConverter _converter = TypeDescriptor.GetConverter(typeof(Guid));


        public bool Matches(Type type)
        {
            return type.CanBeCastTo<DomainEntity>() && !type.HasAttribute<IgnoreEntityInBindingAttribute>();
        }

        public void Bind(Type type, object instance, IBindingContext context)
        {
            throw new NotSupportedException();
        }

        // TODO: [FubuBound("Has to be a better way of exposing this mess")]
        public object Bind(Type type, IBindingContext context)
        {
            var rawId = context.Service<IRequestData>().Value("Id");
            if (rawId == null) return null;


            var id = (Guid)_converter.ConvertFrom(rawId);

            var entity = context.Service<IRepository>().Find(type, id);
            if (entity != null) return entity;

            if (type.IsConcrete())
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}