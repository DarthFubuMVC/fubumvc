using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Binding;
using FubuFastPack.Domain;
using FubuFastPack.Persistence;
using FubuFastPack.Validation;

namespace FubuFastPack.Crud
{
    public interface IEntityDefaults
    {
        void ApplyDefaultsToNewEntity(DomainEntity entity);
    }

    public class NulloEntityDefaults : IEntityDefaults
    {
        public void ApplyDefaultsToNewEntity(DomainEntity entity)
        {
            // nothing
        }
    }

    public class EditEntityModelBinder : IModelBinder
    {
        private readonly IModelBinder _innerBinder;
        private readonly IEntityDefaults _entityDefaults;

        public EditEntityModelBinder(IModelBinder innerBinder, IEntityDefaults entityDefaults)
        {
            _innerBinder = innerBinder;
            _entityDefaults = entityDefaults;
        }

        public bool Matches(Type type)
        {
            return type.CanBeCastTo<EditEntityModel>();
        }

        public void Bind(Type type, object instance, IBindingContext context)
        {
            throw new NotImplementedException();
        }

        public object Bind(Type type, IBindingContext context)
        {
            var entityType = type.GetConstructors().Single(x => x.GetParameters().Count() == 1).GetParameters().Single().ParameterType;

            // This is our convention.  
            var prefix = entityType.Name;
            var prefixedContext = context.PrefixWith(prefix);

            DomainEntity entity = tryFindExistingEntity(context, prefixedContext, entityType) ?? createNewEntity(entityType, prefixedContext);

            var model = (EditEntityModel)Activator.CreateInstance(type, entity);


            // Get the binding errors from conversion of the Entity
            prefixedContext.Problems.Each(x =>
            {
                model.Notification.RegisterMessage(x.Properties.Last(), FastPackKeys.PARSE_VALUE);
            });

            _innerBinder.Bind(type, model, context);

            // Get the binding errors from conversion of the EditEntityModel
            context.Problems.Each(x =>
            {
                model.Notification.RegisterMessage(x.Properties.Last(), FastPackKeys.PARSE_VALUE);
            });

            return model;
        }

        private DomainEntity createNewEntity(Type entityType, IBindingContext prefixedContext)
        {
            var entity = (DomainEntity)_innerBinder.Bind(entityType, prefixedContext);
            entity.Id = Guid.Empty;
            _entityDefaults.ApplyDefaultsToNewEntity(entity);

            return entity;
        }

        private DomainEntity tryFindExistingEntity(IBindingContext context, IBindingContext prefixedContext, Type entityType)
        {
            DomainEntity entity = null;

            context.Service<IRequestData>().Value("Id", rawId =>
            {
                if (rawId == null || rawId.ToString() == string.Empty) return;

                Guid id;
                var gotIt = Guid.TryParse(rawId.ToString(), out id);
                if (gotIt)
                {
                    entity = context.Service<IRepository>().Find(entityType, id);
                    _innerBinder.Bind(entityType, entity, prefixedContext);
                }
            });

            return entity;
        }
    }
}