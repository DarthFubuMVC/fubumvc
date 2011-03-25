using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security;

namespace FubuFastPack.Crud
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class LinkToViewAttribute : AuthorizationAttribute
    {
        private readonly Type _entityType;

        public LinkToViewAttribute()
        {
        }

        public LinkToViewAttribute(Type entityType)
        {
            _entityType = entityType;
        }

        public Type EntityType
        {
            get { return _entityType; }
        }

        public override void Alter(ActionCall call)
        {
            var entityType = _entityType ?? call.HandlerType.GetEntityType();

            var role = CrudRules.SecurableNameForViewing(entityType);
            call.ParentChain().Authorization.AddRole(role);
        }
    }
}