using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FubuValidation
{
    public static class TemplateParser
    {
        private static readonly string TemplateGroup;
        private static readonly Regex TemplateExpression;
        static TemplateParser()
        {
            TemplateGroup = "Template";
            TemplateExpression = new Regex(@"\{(?<" + TemplateGroup + @">\w+)\}", RegexOptions.Compiled);
        }

        public static string Parse(string template, IDictionary<string, string> substitutions)
        {
            MatchCollection matches = TemplateExpression.Matches(template);

            int lastIndex = 0;
            var builder = new StringBuilder();
            foreach (Match match in matches)
            {
                var key = match.Groups[TemplateGroup].Value;
                if ((lastIndex == 0 || match.Index > lastIndex) && substitutions.ContainsKey(key))
                {
                    builder.Append(template.Substring(lastIndex, match.Index - lastIndex));
                    builder.Append(substitutions[key]);
                }

                lastIndex = match.Index + match.Length;
            }

            if (lastIndex < template.Length - 1)
            {
                builder.Append(template.Substring(lastIndex, template.Length - lastIndex));
            }

            return builder.ToString();
        }
    }
}