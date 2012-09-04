using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;

namespace FubuMVC.Core
{
    public static class ConfigurationActionListExtensions
    {
        public static void FillAction<T>(this IList<T> actions, T action)
        {
            var actionType = action.GetType();


            if (TypeIsUnique(actionType) && actions.Any(x => x.GetType() == actionType))
            {
                return;
            }

            actions.Fill(action);
        }

        public static bool TypeIsUnique(Type type)
        {
            if (type.HasAttribute<CanBeMultiplesAttribute>()) return false;

            // If it does not have any non-default constructors
            if (type.GetConstructors().Any(x => x.GetParameters().Any()))
            {
                return false;
            }

            if (type.GetProperties().Any(x => x.CanWrite))
            {
                return false;
            }

            return true;
        }
    }
}