using System;
using FubuCore.Binding;
using StructureMap;

namespace FubuMVC.RavenDb.InMemory
{
    public class DelegatingTransaction : TransactionBase
    {
        private readonly IContainer _container;

        public DelegatingTransaction(IContainer container)
        {
            _container = container;
        }

        public override void Execute<T>(ServiceArguments arguments, Action<T> action)
        {
            _container.Apply(arguments);
            action(_container.GetInstance<T>());
        }


    }
}