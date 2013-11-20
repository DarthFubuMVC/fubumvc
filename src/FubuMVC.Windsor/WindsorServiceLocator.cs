using System;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;
using FubuCore;

namespace FubuMVC.Windsor
{
    public class WindsorServiceLocator : IServiceLocator, IDisposable
    {
        private readonly IKernel _kernel;
        private readonly IDisposable _scope;
        public WindsorServiceLocator(IKernel kernel)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");
            _kernel = kernel;
            _scope = _kernel.RequireScope();
        }

        public T GetInstance<T>()
        {
            return _kernel.Resolve<T>();
        }

        public object GetInstance(Type type)
        {
            return _kernel.Resolve(type);
        }

        public T GetInstance<T>(string name)
        {
            return _kernel.Resolve<T>(name);
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}