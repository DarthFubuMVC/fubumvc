using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuLocalization;

namespace FubuFastPack.Querying
{
    public class OperatorKeys : StringToken
    {
        private IList<Type> _numericTypes = new List<Type>
                                    {
                                        typeof (byte),
                                        typeof (short),
                                        typeof (int),
                                        typeof (long),
                                        typeof (sbyte),
                                        typeof (ushort),
                                        typeof (uint),
                                        typeof (ulong),
                                        typeof (byte?),
                                        typeof (short?),
                                        typeof (int?),
                                        typeof (long?),
                                        typeof (sbyte?),
                                        typeof (ushort?),
                                        typeof (uint?),
                                        typeof (ulong?)
                                    };

        private IList<Type> _dateTypes = new List<Type>
                                {
                                    typeof (DateTime),
                                    typeof (DateTime?),
                                    typeof (DateTimeOffset),
                                    typeof (DateTimeOffset?)
                                };

        private IList<Type> _booleanTypes = new List<Type>
                                   {
                                       typeof (bool),
                                       typeof (bool?)
                                   };


        protected OperatorKeys(string key, string defaultValue) : base(key, defaultValue)
        {
        }

        private static OperatorKeys StringFilter(string key, string defaultValue, Expression<Func<string, bool>> stringMethod)
        {
            var @operator = new OperatorKeys(key, defaultValue);


            return @operator;
        }

        public static readonly OperatorKeys EQUAL = new OperatorKeys("EQUAL", "Equals");
        public static readonly OperatorKeys NOTEQUAL = new OperatorKeys("NOTEQUAL", "Not Equals");
        public static readonly OperatorKeys LESSTHAN = new OperatorKeys("LESSTHAN", "Less Than");
        public static readonly OperatorKeys LESSTHANOREQUAL = new OperatorKeys("LESSTHANOREQUAL", "Less Than or Equal To");
        public static readonly OperatorKeys GREATERTHAN = new OperatorKeys("GREATERTHAN", "Greater Than");
        public static readonly OperatorKeys GREATERTHANOREQUAL = new OperatorKeys("GREATERTHANOREQUAL", "Greater Than or Equal To");
        public static readonly OperatorKeys STARTSWITH = new OperatorKeys("STARTSWITH", "Starts With");
        public static readonly OperatorKeys ENDSWITH = new OperatorKeys("ENDSWITH", "Ends With");
        public static readonly OperatorKeys CONTAINS = new OperatorKeys("CONTAINS", "Contains");
    }

    
}