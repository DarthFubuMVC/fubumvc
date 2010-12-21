using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using FubuCore.Reflection;

namespace FubuCore.CommandLine
{
    public class InputParser
    {
        public static readonly string FLAG_PREFIX = "-";
        public static readonly string FLAG_SUFFIX = "Flag";
        private readonly ObjectConverter _converter = new ObjectConverter();

        public object BuildInput(Type inputType, Queue<string> tokens)
        {
            // Important to leave the ToList() there to force it to be evaluated
            var handlers = inputType.GetProperties().Select(BuildHandler).ToList();
            var model = Activator.CreateInstance(inputType);

            // TODO -- need to throw a good message here
            while (tokens.Any())
            {
                try
                {
                    handlers.First(h => h.Handle(model, tokens));
                }
                catch (InvalidOperationException e)
                {
                    // TODO -- show usage here
                    throw new ApplicationException("Trying to process token " + tokens.Peek(), e);
                }
            }

            return model;
        }

        public ITokenHandler BuildHandler(PropertyInfo property)
        {
            if (!property.Name.EndsWith(FLAG_SUFFIX))
            {
                return new Argument(property, _converter);
            }

            if (property.PropertyType == typeof(bool))
            {
                return new BooleanFlag(property);
            }

            return new Flag(property, _converter);
        }

        public static string ToFlagName(PropertyInfo property)
        {
            var name = property.Name.Substring(0, property.Name.Length - 4);
            property.ForAttribute<FlagAliasAttribute>(att => name = att.Alias);

            return FLAG_PREFIX + name.ToLower();
        }
    }
}