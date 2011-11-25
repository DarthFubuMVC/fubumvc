using System.Collections.Generic;

namespace Fubu.Templating
{
    public interface IKeywordReplacer
    {
        void SetToken(string token, string replacement);
        void SetTokens(IDictionary<string,string> tokens);
        string Replace(string input);
        string GetToken(string token);
        bool ContainsToken(string token);
    }
}