using System;

namespace Fubu.Templating
{
    public interface IKeywordPolicy
    {
        bool Matches(string token);
        string Replace(string input);
    }

    public class LambdaKeywordPolicy : IKeywordPolicy
    {
        private readonly Func<string, bool> _matches;
        private readonly Func<string, string> _replaces;

        public LambdaKeywordPolicy(string token, Func<string, bool> matches, Func<string, string> replaces)
        {
            Token = token;
            _matches = matches;
            _replaces = replaces;
        }

        public string Token { get; private set; }

        public bool Matches(string token)
        {
            return _matches(token);
        }

        public string Replace(string input)
        {
            return _replaces(input);
        }
    }
}