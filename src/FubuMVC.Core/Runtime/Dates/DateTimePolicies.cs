using System;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Dates;
using FubuCore.Formatting;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Runtime.Dates
{
    public class DateTimePolicies : IFubuRegistryExtension
    {
        private readonly DateTimeFormattingRules _formattingRules = new DateTimeFormattingRules();
        private Action<IServiceRegistry> _timeZoneRegistration = s => { };

        public string DefaultDateTimeFormat
        {
            get { return _formattingRules.DefaultDateTimeFormat; }
            set { _formattingRules.DefaultDateTimeFormat = value; }
        }


        public UtcDateHandling UtcDateHandling
        {
            get { return _formattingRules.UtcDateHandling; }
            set { _formattingRules.UtcDateHandling = value; }
        }


        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            Func<GetStringRequest, DateTime, DateTime> dateSource = (req, time) => time;
            if (UtcDateHandling == UtcDateHandling.StoreUtcDisplayLocal)
            {
                registry.Models.ConvertUsing<DateTimeToUtcConverterFamily>();
                dateSource = (req, time) =>
                {
                    var timeZone = req.Get<ITimeZoneContext>().GetTimeZone();

                    return time.ToLocalTime(timeZone);
                };
            }

            var formatter = new DateTimeFormatter(_formattingRules.AllRules().ToList(), dateSource);

            registry.Policies.StringConversions(r =>
            {
                r.IfIsType<DateTime>().ConvertBy((req, time) => formatter.Format(req, time));
                r.IfIsType<DateTime?>().ConvertBy((req, time) => formatter.Format(req, time));
            });

            registry.Services(_timeZoneRegistration);
        }

        public FormatExpression FormatAs(string dateTimeFormat)
        {
            return new FormatExpression(this, dateTimeFormat);
        }

        public void TimeZoneContext<T>() where T : ITimeZoneContext
        {
            _timeZoneRegistration = s => s.ReplaceService<ITimeZoneContext, T>();
        }

        #region Nested type: FormatExpression

        public class FormatExpression
        {
            private readonly string _format;
            private readonly DateTimePolicies _parent;

            public FormatExpression(DateTimePolicies parent, string format)
            {
                _parent = parent;
                _format = format;
            }

            public FormatExpression IfThePropertyNameMatches(Expression<Func<string, bool>> propertyNameFilter,
                                                             string description = "")
            {
                var rule = new DateFormattingRule{
                    Description = description,
                    Filter = propertyNameFilter,
                    Format = _format
                };

                _parent._formattingRules.AddRule(rule);

                return this;
            }
        }

        #endregion
    }
}