using System;
using System.Linq.Expressions;
using FubuCore;

namespace FubuMVC.Core.Runtime.Dates
{
    // Tested through DateTimeFormatting_import_integration_testing
    public class DateFormattingRule
    {
        private Expression<Func<string, bool>> _filter;
        private Lazy<Func<string, bool>> _func;
        public string Format { get; set; }

        public Expression<Func<string, bool>> Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                _func = new Lazy<Func<string, bool>>(value.Compile);
            }
        }

        public string Description { get; set; }

        public override string ToString()
        {
            return Description.IsEmpty()
                       ? string.Format("Format: {0} if the property name matches {1}", Format, Filter)
                       : Description;
        }

        public bool Matches(string propertyName)
        {
            return _func.Value(propertyName);
        }
    }
}