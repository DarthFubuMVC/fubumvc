using System;
using FubuCore.Binding;
using StructureMap;

namespace FubuMVC.RavenDb.RavenDb
{
    public class RavenTransaction : TransactionBase
    {
        private readonly IContainer _container;

        public RavenTransaction(IContainer container)
        {
            _container = container;
        }

        public override void Execute<T>(ServiceArguments arguments, Action<T> action)
        {
            using (IContainer nested = _container.GetNestedContainer())
            {
                nested.Apply(arguments);

                var service = nested.GetInstance<T>();
                action(service);

                nested.GetInstance<ISessionBoundary>().SaveChanges();
            }
        }
    }
}