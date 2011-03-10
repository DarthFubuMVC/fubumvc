using System;
using System.Collections.Generic;
using FubuCore.Reflection;
using FubuLocalization;

namespace FubuFastPack.Querying
{
    public class OperatorKeys : StringToken
    {
        private static readonly IList<OperatorKeys> _keys = new List<OperatorKeys>();
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


        protected OperatorKeys(string key, string defaultValue) : base(key, defaultValue)
        {
            _keys.Add(this);
        }

        public OperatorDto ToOperator()
        {
            return new OperatorDto{
                display = ToString(),
                value = Key
            };
        }

        public static IEnumerable<OperatorKeys> Keys
        {
            get { return _keys; }
        }
    }
}