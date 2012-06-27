using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Formatting;
using FubuCore.Util;

namespace FubuMVC.Core.Runtime.Dates
{
    // Tested through DateTimeFormatting_import_integration_testing
    public class DateTimeFormatter
    {
        private readonly Cache<string, string> _formats = new Cache<string, string>();

        private readonly IEnumerable<DateFormattingRule> _rules;
        private readonly Func<GetStringRequest, DateTime, DateTime> _source;

        public DateTimeFormatter(IEnumerable<DateFormattingRule> rules,
                                 Func<GetStringRequest, DateTime, DateTime> source)
        {
            _rules = rules;
            _source = source;

            _formats.OnMissing = name => _rules.First(x => x.Matches(name)).Format;
        }

        public string Format(GetStringRequest request, DateTime time)
        {
            var timeToDisplay = _source(request, time);
            return timeToDisplay.ToString(_formats[request.Property.Name]);
        }

        public string Format(GetStringRequest request, DateTime? time)
        {
            if (time == null) return string.Empty;

            return Format(request, time.Value);
        }
    }
}