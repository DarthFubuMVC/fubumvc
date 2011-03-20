using System;

namespace FubuValidation
{
    public class MaximumStringLengthAttribute : Attribute
    {
        private readonly int _length;

        public MaximumStringLengthAttribute(int length)
        {
            _length = length;
        }

        public int Length
        {
            get { return _length; }
        }
    }
}