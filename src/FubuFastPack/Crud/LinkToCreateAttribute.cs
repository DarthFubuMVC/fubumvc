using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security;

namespace FubuFastPack.Crud
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class LinkToCreateAttribute : AuthorizationAttribute
    {
        private readonly Type _entityType;

        public LinkToCreateAttribute()
        {
        }

        public LinkToCreateAttribute(Type entityType)
        {
            _entityType = entityType;
        }

        public override void Alter(ActionCall call)
        {
            var entityType = _entityType ?? call.HandlerType.GetEntityType();

            var role = CrudRules.SecurableNameForCreation(entityType);
            call.ParentChain().Authorization.AddRole(role);
        }

        public Type EntityType
        {
            get { return _entityType; }
        }
    }
}