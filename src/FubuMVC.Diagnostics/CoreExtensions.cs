using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FubuMVC.Diagnostics
{
    public static class CoreExtensions
    {
        public static IEnumerable<PropertyInfo> PropertiesWhere(this Type type, Func<PropertyInfo, bool> predicate)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return properties.Where(predicate);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string fieldName) where T : class
        {
            MethodCallExpression resultExp = GenerateMethodCall<T>(source, "OrderBy", fieldName);
            return source.Provider.CreateQuery<T>(resultExp) as IOrderedQueryable<T>;
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string fieldName) where T : class
        {
            MethodCallExpression resultExp = GenerateMethodCall<T>(source, "OrderByDescending", fieldName);
            return source.Provider.CreateQuery<T>(resultExp) as IOrderedQueryable<T>;
        }

        private static LambdaExpression GenerateSelector<T>(String propertyName, out Type resultType) where T : class
        {
            // Create a parameter to pass into the Lambda expression (Entity => Entity.OrderByField).
            var parameter = Expression.Parameter(typeof(T), "Entity");
            //  create the selector part, but support child properties
            PropertyInfo property;
            Expression propertyAccess;
            if (propertyName.Contains('.'))
            {
                // support to be sorted on child fields.
                String[] childProperties = propertyName.Split('.');
                property = typeof(T).GetProperty(childProperties[0], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
                for (int i = 1; i < childProperties.Length; i++)
                {
                    property = property.PropertyType.GetProperty(childProperties[i], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                }
            }
            else
            {
                property = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
            }
            resultType = property.PropertyType;
            // Create the order by expression.
            return Expression.Lambda(propertyAccess, parameter);
        }
        private static MethodCallExpression GenerateMethodCall<T>(IQueryable<T> source, string methodName, String fieldName) where T : class
        {
            Type type = typeof(T);
            Type selectorResultType;
            LambdaExpression selector = GenerateSelector<T>(fieldName, out selectorResultType);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName,
                            new Type[] { type, selectorResultType },
                            source.Expression, Expression.Quote(selector));
            return resultExp;
        }
    }
}