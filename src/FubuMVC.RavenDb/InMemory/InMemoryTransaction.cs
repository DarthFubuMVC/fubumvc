using System;
using FubuCore.Binding;
using StructureMap;
using StructureMap.Pipeline;

namespace FubuMVC.RavenDb.InMemory
{
    public class InMemoryTransaction : TransactionBase
    {
        private readonly IContainer _container;

        public InMemoryTransaction(IContainer container)
        {
            _container = container;
        }

        public override void Execute<T>(ServiceArguments arguments, Action<T> action) 
        {
            using (var nested = _container.GetNestedContainer())
            {
                var explicits = new ExplicitArguments();
                arguments.EachService((type, o) =>  explicits.Set(type, o));

                action(nested.GetInstance<T>(explicits));
            }
        }

    }
}