using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FubuMVC.Core
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// You must use this extension method in place of MethodInfo.Equals(MethodInfo) 
        /// because MethodInfo.Equals is a liar.
        /// </summary>
        /// <param name="method1"></param>
        /// <param name="method2"></param>
        /// <returns></returns>
        public static bool Matches(this MethodInfo method1, MethodInfo method2)
        {
            if (method1.DeclaringType != method2.DeclaringType || method1.Name != method2.Name)
                return false;
            var parameters = method1.GetParameters();
            if (parameters.Count() != method2.GetParameters().Count()) 
                return false;
            for (int i = 0; i < parameters.Count(); i++)
            {
                if (!Matches(parameters[i], method2.GetParameters()[i])) 
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Equivalency check for ParameterInfo objects
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <returns></returns>
        public static bool Matches(this ParameterInfo parameter1, ParameterInfo parameter2)
        {
            return parameter1.Name == parameter2.Name && parameter1.ParameterType == parameter2.ParameterType;
        }
    }
}
