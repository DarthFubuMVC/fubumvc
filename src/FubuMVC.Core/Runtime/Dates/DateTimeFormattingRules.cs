using System.Collections.Generic;

namespace FubuMVC.Core.Runtime.Dates
{
    // Tested through DateTimeFormatting_import_integration_testing
    public class DateTimeFormattingRules
    {
        private readonly IList<DateFormattingRule> _rules = new List<DateFormattingRule>();

        public string DefaultDateTimeFormat = "F";
        public UtcDateHandling UtcDateHandling = UtcDateHandling.StoreUtcDisplayLocal;

        public IEnumerable<DateFormattingRule> Rules
        {
            get { return _rules; }
        }

        public void AddRule(DateFormattingRule rule)
        {
            _rules.Add(rule);
        }

        public IEnumerable<DateFormattingRule> AllRules()
        {
            foreach (var rule in _rules)
            {
                yield return rule;
            }

            foreach (var rule in defaultRules())
            {
                yield return rule;
            }

            yield return new DateFormattingRule{
                Description = "Default DateTime formatting to 'F'",
                Filter = name => true,
                Format = DefaultDateTimeFormat
            };
        }

        private static IEnumerable<DateFormattingRule> defaultRules()
        {
            yield return new DateFormattingRule{
                Filter = name => name.EndsWith("Date"),
                Format = "d"
            };

            yield return new DateFormattingRule{
                Filter = name => name.EndsWith("Time"),
                Format = "f"
            };

            yield return new DateFormattingRule{
                Filter = name => name.Contains("Timestamp"),
                Format = "F"
            };
        }
    }
}