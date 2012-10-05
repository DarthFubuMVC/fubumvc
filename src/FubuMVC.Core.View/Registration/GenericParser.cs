using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;

namespace FubuMVC.Core.View.Registration
{
    public interface IGenericParser
    {
        Type Parse(string typeName);
        IEnumerable<string> ParseErrors { get; }
    }

    public class GenericParser : IGenericParser
    {
        private readonly IEnumerable<Assembly> _assemblies;
        private readonly List<string> _parseErrors = new List<string>();

        public IEnumerable<string> ParseErrors
        {
            get { return _parseErrors; }
        }

        public GenericParser(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        public GenericTypeDefinition parseTypeName(string typeName)
        {
            var leftGenericDelimiter = typeName.IndexOf('<');
            var rightGenericDelimiter = typeName.IndexOf('>');

            var genericArgumentsNames = typeName.Substring(leftGenericDelimiter + 1, rightGenericDelimiter - leftGenericDelimiter - 1);
            var genericArguments = genericArgumentsNames.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            var openTypeName = typeName.Substring(0, leftGenericDelimiter);
            var openTypeNameWithArgCount = openTypeName + "`" + genericArguments.Length;

            return new GenericTypeDefinition
            {
                OpenTypeName = openTypeNameWithArgCount,
                ArgumentTypeNames = genericArguments
            };
        }

        public Type Parse(string typeName)
        {
            if (!IsGeneric(typeName))
            {
                _parseErrors.Add("Type {0} does not appear to be generic.".ToFormat(typeName));
                return null;
            }

            var typeDefinition = parseTypeName(typeName);

            var openType = findOpenType(typeDefinition);

            var genericArgumentTypes = findGenericArgumentTypes(typeDefinition);

            if (_parseErrors.Count > 0)
                return null;

            return openType.MakeGenericType(genericArgumentTypes.ToArray());
        }

        public Type findOpenType(GenericTypeDefinition typeDefinition)
        {
            var openTypes = findTypes(typeDefinition.OpenTypeName).ToArray();

            if (openTypes.Count() < 1)
            {
                _parseErrors.Add("No generic type matching {0} was found.".ToFormat(typeDefinition.OpenTypeName));
                return null;
            }
            if (openTypes.Count() > 1)
            {
                var candidates = openTypes.Select(x => x.AssemblyQualifiedName).Join(", ");
                _parseErrors.Add("More than one generic types matching {0} was found. Type ambiguity on: {1}".ToFormat(typeDefinition.OpenTypeName, candidates));
                return null;
            }

            return openTypes.First();
        }

        public IEnumerable<Type> findGenericArgumentTypes(GenericTypeDefinition typeDefinition)
        {
            var genericArguments = new List<Type>();
            foreach (var argumentTypeName in typeDefinition.ArgumentTypeNames)
            {
                var argumentTypes = findTypes(argumentTypeName).ToArray();

                if (argumentTypes.Count() < 1)
                {
                    _parseErrors.Add("No generic argument type matching {0} was found.".ToFormat(argumentTypeName));
                    return null;
                }
                if (argumentTypes.Count() > 1)
                {
                    var candidates = argumentTypes.Select(x => x.AssemblyQualifiedName).Join(", ");
                    _parseErrors.Add("More than one generic argument types matching {0} was found. Type ambiguity on: {1}".ToFormat(argumentTypeName, candidates));
                    return null;
                }

                genericArguments.Add(argumentTypes.First());
            }

            return genericArguments;
        }

        private IEnumerable<Type> findTypes(string typeName)
        {
            var types = _assemblies.Select(assembly => assembly.GetType(typeName));

            return types.Where(type => type != null);
        }

        public static bool IsGeneric(string typeName)
        {
            return typeName.IndexOf('<') != -1;
        }
    }

    public class GenericTypeDefinition
    {
        public string OpenTypeName { get; set; }
        public IEnumerable<string> ArgumentTypeNames { get; set; }
    }
}