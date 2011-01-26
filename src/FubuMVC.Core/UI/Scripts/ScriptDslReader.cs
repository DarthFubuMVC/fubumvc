using System;
using System.Collections.Generic;
using FubuCore.CommandLine;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.UI.Scripts
{
    [Serializable]
    public class InvalidSyntaxException : Exception
    {
        public static readonly string USAGE = @"

  Valid usages:
  # comments
  <before name> preceeds <after name>
  <name> is <alias>
  <name> requires <dependency names>
  <name> extends <name>
  <set> includes <names>
  ordered set <name> is
  <script1>
  <script2>
  <script3>
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
        private Action<string> _readerAction;
        private string _lastName;

        public ScriptDslReader(IScriptRegistration registration)
        {
            _registration = registration;
        }

        public void ReadLine(string text)
        {
            if (text.Trim().StartsWith("#")) return;

            var tokens = new Queue<string>(StringTokenizer.Tokenize(text.Replace(',', ' ')));

            if (tokens.Count == 1)
            {
                if (_readerAction == null)
                {
                    throw new InvalidSyntaxException("Not enough tokens in the command line");
                }

                _readerAction(tokens.First());
                return;
            }

            _lastName = null;
            _readerAction = null;

            if (tokens.Count() < 3)
            {
                throw new InvalidSyntaxException("Not enough tokens in the command line");
            }

            var key = tokens.Dequeue();
            var verb = tokens.Dequeue();

            if (key == "ordered")
            {
                handleOrderedSet(tokens, verb);

                return;
            }

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

                case "preceeds":
                    tokens.Each(name => _registration.Preceeding(key, name));
                    break;

                default:
                    string message = "'{0}' is an invalid verb".ToFormat(verb);
                    throw new InvalidSyntaxException(message);
            }

        }

        private void handleOrderedSet(Queue<string> tokens, string verb)
        {
            if (verb != "set")
            {
                throw new InvalidSyntaxException("Malformed call to ordered set <name> is");
            }

            var setName = tokens.Dequeue();
            _readerAction = name =>
            {
                _registration.AddToSet(setName, name);
                _lastName = name;

                _readerAction = next =>
                {
                    _registration.AddToSet(setName, next);
                    if (_lastName.IsNotEmpty())
                    {
                        _registration.Dependency(next, _lastName);
                    }

                    _lastName = next;
                };
            };
        }
    }
}