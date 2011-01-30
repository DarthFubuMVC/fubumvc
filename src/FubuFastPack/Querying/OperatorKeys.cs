using System;
using System.Collections.Generic;
using FubuCore.Reflection;
using FubuLocalization;

namespace FubuFastPack.Querying
{
    public class OperatorKeys : StringToken
    {
        public static readonly OperatorKeys EQUAL = new OperatorKeys("EQUAL", "Equals");
        public static readonly OperatorKeys NOTEQUAL = new OperatorKeys("NOTEQUAL", "Not Equals");
        public static readonly OperatorKeys LESSTHAN = new OperatorKeys("LESSTHAN", "Less Than");

        public static readonly OperatorKeys LESSTHANOREQUAL = new OperatorKeys("LESSTHANOREQUAL",
                                                                               "Less Than or Equal To");

        public static readonly OperatorKeys GREATERTHAN = new OperatorKeys("GREATERTHAN", "Greater Than");

        public static readonly OperatorKeys GREATERTHANOREQUAL = new OperatorKeys("GREATERTHANOREQUAL",
                                                                                  "Greater Than or Equal To");

        public static readonly OperatorKeys STARTSWITH = new OperatorKeys("STARTSWITH", "Starts With");
        public static readonly OperatorKeys ENDSWITH = new OperatorKeys("ENDSWITH", "Ends With");
        public static readonly OperatorKeys CONTAINS = new OperatorKeys("CONTAINS", "Contains");

        public static readonly StringToken WITHIN_X_DAYS = new OperatorKeys("WITHIN_X_DAYS", "Within x days");
        public static readonly StringToken AFTER_X_DAYS = new OperatorKeys("AFTER_X_DAYS", "After x days");
        public static readonly StringToken AFTER_DATE = new OperatorKeys("AFTER_DATE", "After Date");
        public static readonly StringToken BEFORE_DATE = new OperatorKeys("BEFORE_DATE", "Before Date");

        protected OperatorKeys(string key, string defaultValue) : base(key, defaultValue)
        {
        }

        public OperatorDTO ToOperator()
        {
            return new OperatorDTO{
                display = ToString(),
                value = Key
            };
        }
    }

    public class FilterDTO
    {
        // Localized header
        public string display;

        // All possibly operators
        public OperatorDTO[] operators;

        // property name
        public string value;
    }

    public class OperatorDTO
    {
        // localized text
        public string display { get; set; }

        // 
        public string value { get; set; }
    }
}