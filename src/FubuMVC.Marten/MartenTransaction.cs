using System;
using System.Data;
using FubuCore.Binding;
using Marten;
using StructureMap;

namespace FubuMVC.Marten
{
    public class MartenTransaction : ITransaction
    {
        private readonly IContainer _container;

        public MartenTransaction(IContainer container)
        {
            _container = container;
        }

        public void Execute<T>(ServiceArguments arguments, Action<T> action) where T : class
        {
            using (var nested = _container.GetNestedContainer())
            {
                nested.Apply(arguments);

                var service = nested.GetInstance<T>();
                action(service);

                nested.GetInstance<ISessionBoundary>().SaveChanges();
            }
        }

        public void Execute<T>(Action<T> action) where T : class
        {
            using (var nested = _container.GetNestedContainer())
            {
                var service = nested.GetInstance<T>();
                action(service);

                nested.GetInstance<ISessionBoundary>().SaveChanges();
            }
        }
    }
}