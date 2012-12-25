using System;

using Autofac;

using FubuCore;


namespace FubuMVC.Autofac
{
    public class AutofacServiceLocator : IServiceLocator
    {
        private readonly IComponentContext _context;


        public AutofacServiceLocator(IComponentContext context)
        {
            _context = context;
        }


        public IComponentContext Context
        {
            get { return _context; }
        }


        public object GetInstance(Type type)
        {
            return _context.Resolve(type);
        }

        public T GetInstance<T>()
        {
            return _context.Resolve<T>();
        }

        public T GetInstance<T>(string name)
        {
            return _context.ResolveNamed<T>(name);
        }
    }
}