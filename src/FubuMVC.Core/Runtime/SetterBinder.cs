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
            _binder.Bind(typeof (T), target, _context);
        }
    }

    public interface ISetterBinder
    {
        void BindProperties<T>(T target);
    }
}