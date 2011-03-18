using System;
using FubuCore;
using FubuValidation.Strategies;

namespace FubuValidation
{


    public class CollectionLengthAttribute : Attribute
    {
        private readonly int _length;

        public CollectionLengthAttribute(int length) 
        {
            _length = length;
        }

        public int Length
        {
            get { return _length; }
        }

    }
}