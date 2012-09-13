using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FubuMVC.Core
{
    public static class ReflectionExtensions
    {
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

        public static bool Matches(this ParameterInfo parameter1, ParameterInfo parameter2)
        {
            return parameter1.Name == parameter2.Name && parameter1.ParameterType == parameter2.ParameterType;
        }
    }
}
