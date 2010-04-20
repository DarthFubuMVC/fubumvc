using System;
using System.Linq;
using System.Text.RegularExpressions;
using FubuCore.Util;

namespace FubuCore
{
    public interface IObjectConverter
    {
        object FromString(string stringValue, Type type);
        T FromString<T>(string stringValue);
        bool CanBeParsed(Type type);
    }

    public class ObjectConverter : IObjectConverter
    {
        public const string EMPTY = "EMPTY";
        public const string NULL = "NULL";
        public const string BLANK = "BLANK";

        private const string TIMESPAN_PATTERN =
            @"
(?<quantity>\d+     # quantity is expressed as some digits
(\.\d+)?)           # optionally followed by a decimal point and more digits
\s*                 # optional whitespace
(?<units>\w+)       # units is expressed as a word";

        public const string TODAY = "TODAY";

        private readonly Cache<Type, Func<string, object>> _froms;

        public ObjectConverter()
        {
            _froms = new Cache<Type, Func<string, object>>(createFinder);
            Clear();
        }

        private Func<string, object> createFinder(Type type)
        {
            if (type.IsEnum)
            {
                return createEnumFinder(type);
            }

            if (type.IsNullable())
            {
                return createNullableFinder(type);
            }

            if (type.IsArray)
            {
                return createArrayFinder(type);
            }

            if (type.IsGenericEnumerable())
            {
                return createGenericEnumerableFinder(type);
            }

            // TODO -- change to using the TypeDescriptor stuff
            return stringValue => Convert.ChangeType(stringValue, type);
        }

        private Func<string, object> createGenericEnumerableFinder(Type type)
        {
            return createEnumerableFinder(type.GetGenericArguments()[0]);
        }

        private static Func<string, object> createEnumFinder(Type type)
        {
            return stringValue => Enum.Parse(type, stringValue);
        }

        private Func<string, object> createArrayFinder(Type type)
        {
            var innerType = type.GetElementType();

            return createEnumerableFinder(innerType);
        }

        private Func<string, object> createEnumerableFinder(Type innerType)
        {
            var singleObjectFinder = _froms[innerType];

            return stringValue =>
            {
                if (stringValue.ToUpper() == EMPTY)
                {
                    return Array.CreateInstance(innerType, 0);
                }

                var strings = stringValue.ToDelimitedArray();
                var array = Array.CreateInstance(innerType, strings.Length);

                for (var i = 0; i < strings.Length; i++)
                {
                    var value = singleObjectFinder(strings[i]);
                    array.SetValue(value, i);
                }

                return array;
            };
        }

        private Func<string, object> createNullableFinder(Type type)
        {
            Func<string, object> inner = _froms[type.GetInnerTypeFromNullable()];

            return stringValue =>
            {
                if (stringValue == NULL) return null;
                return inner(stringValue);
            };
        }

        public bool CanBeParsed(Type type)
        {
            if (type.IsArray) return CanBeParsed(type.GetElementType());
            if (type.IsGenericEnumerable()) return CanBeParsed(type.GetGenericArguments()[0]);

            if (_froms.Has(type)) return true;
            if (type.IsNullable()) return true;
            if (type == typeof(DateTime)) return true;
            if (type.IsSimple()) return true;

            return false;
        }

        public void RegisterConverter<T>(Func<string, T> finder)
        {
            _froms[typeof(T)] = x => finder(x);
        }

        public void Clear()
        {
            _froms.ClearAll();
            _froms[typeof(string)] = parseString;
            _froms[typeof(DateTime)] = key => GetDateTime(key);
            _froms[typeof(TimeSpan)] = key => GetTimeSpan(key);
        }

        public virtual object FromString(string stringValue, Type type)
        {
            if (stringValue == NULL)
            {
                return null;
            }

            if (string.IsNullOrEmpty(stringValue))
            {
                throw new ArgumentNullException("stringValue", "Trying to resolve type " + type.FullName);
            }

            return _froms[type](stringValue);
        }

        public virtual T FromString<T>(string stringValue)
        {
            return (T)FromString(stringValue, typeof(T));
        }

        private static string parseString(string stringValue)
        {
            if (stringValue == BLANK || stringValue == EMPTY)
            {
                return string.Empty;
            }

            if (stringValue == NULL)
            {
                return null;
            }

            return stringValue;
        }

        public static DateTime GetDateTime(string dateString)
        {
            string trimmedString = dateString.Trim();
            if (trimmedString == TODAY)
            {
                return DateTime.Today;
            }

            if (trimmedString.Contains(TODAY))
            {
                string dayString = trimmedString.Substring(5, trimmedString.Length - 5);
                int days = int.Parse(dayString);

                return DateTime.Today.AddDays(days);
            }

            if (isDayOfWeek(dateString))
            {
                return convertToDateFromDayAndTime(dateString);
            }

            return DateTime.Parse(trimmedString);
        }

        private static DateTime convertToDateFromDayAndTime(string dateString)
        {
            dateString = dateString.Replace("  ", " ");
            string[] parts = dateString.Split(' ');
            var day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), parts[0], true);
            int minutes = minutesFrom24HourTime(parts[1]);

            DateTime date = DateTime.Today.AddMinutes(minutes);
            while (date.DayOfWeek != day)
            {
                date = date.AddDays(1);
            }

            return date;
        }

        private static bool isDayOfWeek(string text)
        {
            string[] days = Enum.GetNames(typeof(DayOfWeek));
            return days.FirstOrDefault(x => text.ToLower().StartsWith(x.ToLower())) != null;
        }

        private static int minutesFrom24HourTime(string time)
        {
            string[] parts = time.Split(':');
            return 60 * int.Parse(parts[0]) + int.Parse(parts[1]);
        }

        public static TimeSpan GetTimeSpan(string timeString)
        {
            Match match = Regex.Match(timeString, TIMESPAN_PATTERN, RegexOptions.IgnorePatternWhitespace);
            if (!match.Success)
            {
                return TimeSpan.Parse(timeString);
            }

            double number = double.Parse(match.Groups["quantity"].Value);
            string units = match.Groups["units"].Value.ToLower();
            switch (units)
            {
                case "s":
                case "second":
                case "seconds":
                    return TimeSpan.FromSeconds(number);
                case "m":
                case "minute":
                case "minutes":
                    return TimeSpan.FromMinutes(number);

                case "h":
                case "hour":
                case "hours":
                    return TimeSpan.FromHours(number);

                case "d":
                case "day":
                case "days":
                    return TimeSpan.FromDays(number);
            }

            throw new ApplicationException("Time periods must be expressed in seconds, minutes, hours, or days.");
        }
    }
}