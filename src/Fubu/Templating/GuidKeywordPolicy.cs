using System;
using System.Text.RegularExpressions;
using FubuCore.Util;

namespace Fubu.Templating
{
    public class GuidKeywordPolicy : IKeywordPolicy
    {
        public static readonly Regex GuidExpression = new Regex("GUID([0-9]+)", RegexOptions.Compiled);

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
            foreach (Match match in matches)
            {
                var value = _guids[match.Value];
                input = input.Replace(match.Value, value);
            }

            return input;
        }
    }
}