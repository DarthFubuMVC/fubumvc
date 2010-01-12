using System;

namespace FubuMVC.Core
{
    public static class TypeExtensions
    {
        public static bool CanBeCastTo<T>(this Type type)
        {
            if (type == null) return false;

            if (type == typeof (T)) return true;

            return typeof (T).IsAssignableFrom(type);
        }

        public static bool IsInNamespace(this Type type, string nameSpace)
        {
            return type.Namespace.StartsWith(nameSpace);
        }

        public static bool IsInNamespaceContaining<T>(this Type type)
        {
            return type.IsInNamespace(typeof (T).Namespace);
        }

        public static bool IsOpenGeneric(this Type type)
        {
            return type.IsGenericTypeDefinition || type.ContainsGenericParameters;
        }

        public static bool IsConcreteTypeOf(this Type pluggedType, Type pluginType)
        {
            return pluggedType.IsConcrete() && pluginType.IsAssignableFrom(pluggedType);
        }

        public static bool IsConcreteTypeOf<T>(this Type pluggedType)
        {
            return pluggedType.IsConcrete() && typeof (T).IsAssignableFrom(pluggedType);
        }

        public static bool ImplementsInterfaceTemplate(this Type pluggedType, Type templateType)
        {
            if (!pluggedType.IsConcrete()) return false;

            foreach (Type interfaceType in pluggedType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == templateType)
                {
                    return true;
                }
            }

            return false;
        }

        public static Type FindInterfaceThatCloses(this Type pluggedType, Type templateType)
        {
            if (!pluggedType.IsConcrete()) return null;

            foreach (Type interfaceType in pluggedType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == templateType)
                {
                    return interfaceType;
                }
            }

            return pluggedType.BaseType == typeof (object)
                       ? null
                       : pluggedType.BaseType.FindInterfaceThatCloses(templateType);
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
        }

        public static bool Closes(this Type type, Type openType)
        {
            Type baseType = type.BaseType;
            if (baseType == null) return false;

            bool closes = baseType.IsGenericType && baseType.GetGenericTypeDefinition() == openType;
            if (closes) return true;

            return type.BaseType == null ? false : type.BaseType.Closes(openType);
        }

        public static Type GetInnerTypeFromNullable(this Type nullableType)
        {
            return nullableType.GetGenericArguments()[0];
        }


        public static string GetName(this Type type)
        {
            if (type.IsGenericType)
            {
                string[] parameters = Array.ConvertAll(type.GetGenericArguments(), t => t.GetName());
                string parameterList = String.Join(", ", parameters);
                return "{0}<{1}>".ToFormat(type.Name, parameterList);
            }

            return type.Name;
        }

        public static string GetFullName(this Type type)
        {
            if (type.IsGenericType)
            {
                string[] parameters = Array.ConvertAll(type.GetGenericArguments(), t => t.GetName());
                string parameterList = String.Join(", ", parameters);
                return "{0}<{1}>".ToFormat(type.Name, parameterList);
            }

            return type.FullName;
        }


        public static bool IsString(this Type type)
        {
            return type.Equals(typeof (string));
        }

        public static bool IsPrimitive(this Type type)
        {
            return type.IsPrimitive && !IsString(type) && type != typeof (IntPtr);
        }

        public static bool IsSimple(this Type type)
        {
            return type.IsPrimitive || IsString(type) || type.IsEnum;
        }

        public static bool IsConcrete(this Type type)
        {
            return !type.IsAbstract && !type.IsInterface;
        }

        public static bool IsNotConcrete(this Type type)
        {
            return !type.IsConcrete();
        }
    }
}