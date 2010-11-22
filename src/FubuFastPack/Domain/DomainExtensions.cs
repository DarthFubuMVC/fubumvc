using System;
using NHibernate.Proxy;

namespace FubuFastPack.Domain
{
    public static class DomainExtensions
    {
        public static Type GetTrueType<T>(this T entity)
        {
            return entity as INHibernateProxy != null ? entity.GetType().BaseType : entity.GetType();
        }

        public static Type ToTrueType(this Type type)
        {
            return typeof(INHibernateProxy).IsAssignableFrom(type) ? type.BaseType : type;
        }

        /// <summary>
        /// True if the provided type can be cast as <see cref="DomainEntity"/>
        /// </summary>
        /// <param name="type">The Type to test</param>
        public static bool IsEntity(this Type type)
        {
            return typeof(DomainEntity).IsAssignableFrom(type);
        }
    }
}