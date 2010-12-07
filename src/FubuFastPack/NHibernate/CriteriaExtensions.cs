using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using NHibernate;
using NHibernate.Criterion;
using Expression = NHibernate.Criterion.Expression;

namespace FubuFastPack.NHibernate
{
    public static class CriteriaExtensions
    {
        public static string ToPropertyName(this Accessor accessor)
        {
            return accessor.PropertyNames.Join(".");
        }

        public static string ToPropertyName<T, U>(this Expression<Func<T, U>> property)
        {
            return ReflectionHelper.GetAccessor(property).ToPropertyName();
        }

        public static string ToPropertyName<T>(this Expression<Func<T, object>> expression)
        {
            return ReflectionHelper.GetAccessor(expression).ToPropertyName();
        }

        public static DetachedCriteria SetProjection<T>(this DetachedCriteria criteria, Expression<Func<T, object>> expression)
        {
            var propertyName = expression.ToPropertyName();
            return criteria.SetProjection(Projections.Property(propertyName));
        }

        public static DetachedCriteria WhereEqualTo<T, U>(this DetachedCriteria criteria, Expression<Func<T, U>> expression, U value)
        {
            var propertyName = expression.ToPropertyName();
            return criteria.Add(Expression.Eq(propertyName, value));
        }

        public static ICriteria WhereEqualTo<T, U>(this ICriteria criteria, Expression<Func<T, U>> expression, U value)
        {
            var propertyName = expression.ToPropertyName();
            return criteria.Add(Expression.Eq(propertyName, value));
        }

        public static ICriteria WhereIsNull<T>(this ICriteria criteria, Expression<Func<T, object>> expression)
        {
            var propertyName = expression.ToPropertyName();
            return criteria.Add(Restrictions.IsNull(propertyName));
        }


        public static ICriteria WhereOnOrAfter<T>(this ICriteria criteria, Expression<Func<T, object>> expression, object value)
        {
            var propertyName = expression.ToPropertyName();
            return criteria.Add(Expression.Ge(propertyName, value));
        }

        public static DetachedCriteria WhereIn<T>(this DetachedCriteria criteria, Expression<Func<T, object>> expression, DetachedCriteria subquery)
        {
            var propertyName = expression.ToPropertyName();
            return criteria.Add(Subqueries.PropertyIn(propertyName, subquery));
        }


        public static ICriteria WhereIn<T>(this ICriteria criteria, Expression<Func<T, object>> expression, DetachedCriteria subquery)
        {
            var propertyName = expression.ToPropertyName();
            return criteria.Add(Subqueries.PropertyIn(propertyName, subquery));
        }


        public static ICriteria WhereNotIn<T>(this ICriteria criteria, Expression<Func<T, object>> expression, DetachedCriteria subquery)
        {
            var propertyName = expression.ToPropertyName();
            return criteria.Add(Subqueries.PropertyNotIn(propertyName, subquery));
        }

        public static ICriteria WhereCaseInsensitiveContains<T>(this ICriteria criteria, Expression<Func<T, object>> expression, string value)
        {
            var criterion = Restrictions.InsensitiveLike(expression.ToAccessor().Name, "%" + value + "%");
            return criteria.Add(criterion);
        }
    }
}