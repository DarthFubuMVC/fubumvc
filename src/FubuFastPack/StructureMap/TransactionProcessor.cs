using System;
using FubuFastPack.NHibernate;
using StructureMap;

namespace FubuFastPack.StructureMap
{
    public class TransactionProcessor : ITransactionProcessor
    {
        private readonly object _locker = new object();
        private IContainer _container;

        // This IContainer is the parent container from
        // ObjectFactory.Container
        public TransactionProcessor(IContainer container)
        {
            _container = container;
        }

        public IContainer Container
        {
            get { return _container; }
            set
            {
                lock (_locker)
                {
                    _container = value;
                }
            }
        }

        public void Execute<T>(Action<T> action)
        {
            execute(c =>
            {
                var service = c.GetInstance<T>();
                action(service);
            });
        }

        public TReturn Execute<T, TReturn>(Func<T, TReturn> func)
        {
            TReturn result = default(TReturn);
            execute(c =>
            {
                var service = c.GetInstance<T>();
                result = func(service);
            });
            return result;
        }

        public void Execute<T>(Action<T, IContainer> action)
        {
            execute(c =>
            {
                var service = c.GetInstance<T>();
                action(service, c);
            });
        }

        public void Execute<T>(string instanceName, Action<T> action)
        {
            execute(c =>
            {
                var service = c.GetInstance<T>(instanceName);
                action(service);
            });
        }

        // This code is used in our codebase as a generic way to invoke
        // a service or action within the scope of a nested container
        private void execute(Action<IContainer> action)
        {
            IContainer container = null;
            lock (_locker)
            {
                container = _container;
            }

            using (IContainer nestedContainer = container.GetNestedContainer())
            {
                if (!nestedContainer.Model.HasDefaultImplementationFor<ITransactionBoundary>())
                {
                    throw new ApplicationException("This container has already been disposed");
                }

                var boundary = nestedContainer.GetInstance<ITransactionBoundary>();
                boundary.Start();
                action(nestedContainer);
                boundary.Commit();
            }
        }
    }
}