using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FubuCore.Util;

namespace Fubu
{
    public class KeywordReplacer : IKeywordReplacer
    {
        private readonly IList<IKeywordPolicy> _policies = new List<IKeywordPolicy>();
        private readonly Dictionary<string, string>  _tokens = new Dictionary<string, string>();

        public KeywordReplacer()
        {
            setupDefaults();
        }

        private void setupDefaults()
        {
            _policies.Add(new GuidKeywordPolicy());
        }

        public void SetTokens(IDictionary<string, string> tokens)
        {
            foreach (var token in tokens)
            {
                SetToken(token.Key, token.Value);
            }
        }

        public string Replace(string input)
        {
            var value = input;
            _policies
                .Where(p => p.Matches(value))
                .Each(p =>
                          {
                              value = p.Replace(value);
                          });

            return value;
        }

        public void SetToken(string token, string replacement)
        {
            _tokens.Add(token, replacement);
            _policies.Fill(new LambdaKeywordPolicy(token, t => t.Contains(token), input => input.Replace(token, replacement)));
        }

        public string GetToken(string token)
        {
            return _tokens[token];
        }

        public bool ContainsToken(string token)
        {
            return _tokens.ContainsKey(token);
        }
    }

    public interface IKeywordPolicy
    {
        bool Matches(string token);
        string Replace(string input);
    }

    public class GuidKeywordPolicy : IKeywordPolicy
    {
        public static readonly Regex GuidExpression = new Regex("GUID([0-9]*)", RegexOptions.Compiled);

        private readonly Cache<string, string> _guids;

        public GuidKeywordPolicy()
        {
            _guids = new Cache<string, string>(key => Guid.NewGuid().ToString().ToUpper());
        }

        public bool Matches(string token)
        {
            return GuidExpression.IsMatch(token);
        }

        public string Replace(string input)
        {
            var matches = GuidExpression.Matches(input);
            foreach(Match match in matches)
            {
                var value = _guids[match.Value];
                input = input.Replace(match.Value, value);
            }

            return input;
        }
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