using System;
using FubuCore.Binding;
using FubuMVC.RavenDb.MultiTenancy;

namespace FubuMVC.RavenDb
{
    public interface ITransaction
    {
        void Execute<T>(ServiceArguments arguments, Action<T> action) where T : class;

        void Execute<T>(Action<T> action) where T : class;

        void Execute<T>(Guid tenantId, Action<T> action) where T : class;
    }

    public abstract class TransactionBase : ITransaction
    {
        public abstract void Execute<T>(ServiceArguments arguments, Action<T> action) where T : class;
        
        public void Execute<T>(Action<T> action) where T : class
        {
            Execute(new ServiceArguments(), action);
        }

        public void Execute<T>(Guid tenantId, Action<T> action) where T : class
        {
            Execute(SimpleTenantContext.ArgumentsForTenant(tenantId), action);
        }
    }

    public static class TransactionExtensions
    {
        public static void WithRepository(this ITransaction transaction, Action<IEntityRepository> action)
        {
            transaction.Execute(action);
        }

        public static void WithRepository(this ITransaction transaction, Guid tenantId, Action<IEntityRepository> action)
        {
            transaction.Execute(tenantId, action); 
        }
    }
}