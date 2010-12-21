using System;
using System.Collections.Generic;

namespace FubuCore.CommandLine
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UsageAttribute : Attribute
    {
        private readonly string _name;
        private readonly string _description;

        public UsageAttribute(string name, string description)
        {
            _name = name;
            _description = description;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _description; }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredUsageAttribute : Attribute
    {
        private readonly string[] _usages;

        public RequiredUsageAttribute(params string[] usages)
        {
            _usages = usages;
        }

        public string[] Usages
        {
            get { return _usages; }
        }
    }




    //public class CommandUsage
    //{
    //    public string CommandDescription { get; set; }
    //    public string Description { get; set; }
    //    public IEnumerable<>
    //}


}