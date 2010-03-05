using System;
using FubuMVC.Core.Models;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.Runtime
{
    public class ObjectResolver : IObjectResolver
    {
        private readonly IServiceLocator _services;
        private readonly IModelBinderCache _binders;

        // Leave this here
        public ObjectResolver()
        {
        }

        public ObjectResolver(IServiceLocator services, IModelBinderCache binders)
        {
            _services = services;
            _binders = binders;
        }

        public virtual BindResult BindModel(Type type, IRequestData data)
        {
            var context = new BindingContext(data, _services);
            return BindModel(type, context);
        }

        // Leave this virtual
        public virtual BindResult BindModel(Type type, IBindingContext context)
        {
            IModelBinder binder = _binders.BinderFor(type);
            try
            {
                return new BindResult()
                {
                    Value = binder.Bind(type, context),
                    Problems = context.Problems
                };
            }
            catch (Exception e)
            {
                throw new FubuException(2201, e, "Fatal error while binding model of type {0}.  See inner exception", type.AssemblyQualifiedName);
            }
        }
    }
}