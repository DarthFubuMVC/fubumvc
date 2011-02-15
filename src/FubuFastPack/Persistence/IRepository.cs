using System;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuFastPack.Domain;

namespace FubuFastPack.Persistence
{
    public interface IRepository
    {
        DomainEntity FindByPath(string path);

        // Find an Entity by its primary key
        // We assume and enforce that every Entity
        // is identified by an "Id" property of 
        // type long
        T Find<T>(Guid id) where T : Entity;

        T Find<T>(string text) where T : Entity;

        // Query for a specific type of Entity
        // with Linq expressions.  More on this later
        IQueryable<T> Query<T>();
        IQueryable<T> Query<T>(Expression<Func<T, bool>> where);
        IQueryable<T> Query<T>(IQueryExpression<T> queryExpression) where T : DomainEntity;

        T FindBy<T, U>(Expression<Func<T, U>> expression, U search) where T : class;
        T FindBy<T>(Expression<Func<T, bool>> where);

        // Basic operations on an Entity
        void Delete(object target);
        void Save(object target);
        void Insert(object target);

        // TODO -- this shouldn't be here
        // Move these two methods to ITransactionBoundary?????
        void RejectChanges(object target);
        void FlushChanges();


        T[] GetAll<T>();
    }

    // TODO -- add UT's for this
    public static class RepositoryExtensions
    {
        public static DomainEntity Find(this IRepository repository, Type entityType, Guid id)
        {
            return buildFinderType(entityType).Find(id, repository);
        }

        private static IFinder buildFinderType(Type entityType)
        {
            return typeof(Finder<>).CloseAndBuildAs<IFinder>(entityType);
        }

        public static DomainEntity FindByIdentifier(this IRepository repository, Type entityType, string identifier)
        {
            return buildFinderType(entityType).FindByIdentifier(identifier, repository);
        }


        public static T FindRequired<T>(this IRepository repository, Guid id) where T : Entity
        {
            var entity = repository.Find<T>(id);
            if (entity == null) throw new NonExistentEntityException(typeof(T), id);
            return entity;
        }

        public interface IFinder
        {
            DomainEntity Find(Guid id, IRepository repository);
            DomainEntity FindByIdentifier(string identifier, IRepository repository);
        }

        public class Finder<T> : IFinder where T : DomainEntity
        {
            public DomainEntity Find(Guid id, IRepository repository)
            {
                return repository.Find<T>(id);
            }

            public DomainEntity FindByIdentifier(string identifier, IRepository repository)
            {
                return repository.FindBy<T>(x => ((IHaveIdentifier)x).Identifier == identifier);
            }
        }
    }
}