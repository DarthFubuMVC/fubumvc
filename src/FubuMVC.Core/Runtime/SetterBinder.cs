using System;
using FubuCore.Binding;

namespace FubuMVC.Core.Runtime
{
    public class SetterBinder : ISetterBinder
    {
        private readonly StandardModelBinder _binder;
        private readonly IBindingContext _context;

        public SetterBinder(StandardModelBinder binder, IBindingContext context)
        {
            _binder = binder;
            _context = context;
        }

        public void BindProperties<T>(T target)
        {
            BindProperties(typeof(T), target);
        }

        public void BindProperties(Type type, object target)
        {
            _binder.BindProperties(type, target, _context);
        }
    }

    public interface ISetterBinder
    {
        void BindProperties<T>(T target);
        void BindProperties(Type type, object target);
    }
}