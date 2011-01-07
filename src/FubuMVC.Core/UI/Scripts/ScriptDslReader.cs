using System;
using System.Collections.Generic;
using FubuCore.CommandLine;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.UI.Scripts
{
    public class InvalidSyntaxException : Exception
    {
        public static readonly string USAGE = @"

  Valid usages:

  <name> is <alias>
  <name> requires <dependency names>
  <name> extends <name>
  <set> includes <names>

";


        public InvalidSyntaxException(string message) : base(message + USAGE)
        {
        }

        public InvalidSyntaxException(string message, Exception innerException) : base(message + USAGE, innerException)
        {
        }
    }


    public class ScriptDslReader
    {
        private readonly IScriptRegistration _registration;

        public ScriptDslReader(IScriptRegistration registration)
        {
            _registration = registration;
        }

        public void ReadLine(string text)
        {
            var tokens = new Queue<string>(StringTokenizer.Tokenize(text.Replace(',', ' ')));

            // TODO -- more specific exception
            if (tokens.Count() < 3)
            {
                throw new InvalidSyntaxException("Not enough tokens in the command line");
            }

            var key = tokens.Dequeue();
            var verb = tokens.Dequeue();


            switch (verb)
            {
                case "is":
                    if (tokens.Count > 1) throw new InvalidSyntaxException("Only one name can appear on the right side of the 'is' verb");
                    _registration.Alias(tokens.Dequeue(), key);
                    break;

                case "requires":
                    tokens.Each(name => _registration.Dependency(key, name));
                    break;

                case "extends":
                    if (tokens.Count > 1) throw new InvalidSyntaxException("Only one name can appear on the right side of the 'extends' verb");

                    _registration.Extension(key, tokens.Single());
                    break;

                case "includes":
                    tokens.Each(name => _registration.AddToSet(key, name));
                    break;

                default:
                    string message = "'{0}' is an invalid verb".ToFormat(verb);
                    throw new InvalidSyntaxException(message);
            }

        }
    }
}