using System;
using System.IO;

namespace FubuTransportation.Runtime.Routing
{
    public class SingleTypeRoutingRule<T> : IRoutingRule
    {
        public bool Matches(Type type)
        {
            return type == typeof (T);
        }

        public string Describe()
        {
            return typeof(T).Name;
        }


    }
}