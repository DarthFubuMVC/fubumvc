using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FubuValidation
{
    public class TemplateParser
    {
        public static string Parse(string template, IDictionary<string, string> substitutions)
        {
            MatchCollection matches = Regex.Matches(template, @"\{(\w+)\}");
            var keys = (from Match match in matches select match.Groups[1].Value).ToList();

            int lastIndex = 0;
            var builder = new StringBuilder();
            foreach (Match match in matches)
            {
                if (match.Index > lastIndex)
                {
                    builder.Append(template.Substring(lastIndex, match.Index - lastIndex));
                    builder.Append(substitutions[match.Value.Replace("{", string.Empty).Replace("}", string.Empty)]);
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