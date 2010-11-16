using System;
using System.Collections.Generic;
using System.Reflection;

namespace FubuCore.CommandLine
{
    public class BooleanFlag : ITokenHandler
    {
        private readonly PropertyInfo _property;

        public BooleanFlag(PropertyInfo property)
        {
            _property = property;
        }

        public bool Handle(object input, Queue<string> tokens)
        {
            if (tokens.NextIsFlag(_property))
            {
                tokens.Dequeue();
                _property.SetValue(input, true, null);

                return true;
            }

            return false;
        }
    }
}