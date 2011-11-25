using System.Collections.Generic;
using System.Linq;

namespace Fubu.Templating
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

    
}