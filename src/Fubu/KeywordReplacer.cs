using System.Collections.Generic;
using System.Linq;

namespace Fubu
{
    public class KeywordReplacer : IKeywordReplacer
    {
        private Dictionary<string, string> _tokens;

        public KeywordReplacer()
        {
            _tokens = new Dictionary<string, string>();
        }

        public void SetTokens(IDictionary<string, string> tokens)
        {
            foreach (var token in tokens)
            {
                _tokens.Add(token.Key, token.Value);
            }
        }

        public string Replace(string input)
        {
            return _tokens.Aggregate(input, (memo, keyword) => memo.Replace(keyword.Key, keyword.Value));
        }

        public void SetToken(string token, string replacement)
        {
            _tokens.Add(token, replacement);
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
}