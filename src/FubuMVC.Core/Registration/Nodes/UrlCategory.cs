using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Registration.Nodes
{
    public class UrlCategory
    {
        public UrlCategory()
        {
            Creates = new List<Type>();
        }

        public string Category { get; set; }
        public IList<Type> Creates { get; private set; }
    }
}