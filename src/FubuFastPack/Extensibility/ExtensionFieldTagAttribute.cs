using System;
using System.Linq;

namespace FubuFastPack.Extensibility
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ExtensionFieldTagAttribute : Attribute
    {
        public string[] TagNames { get; set; }

        public ExtensionFieldTagAttribute(params string[] tagNames)
        {
            TagNames = tagNames;
        }

        public bool HasTag(string tagName)
        {
            return TagNames.Contains(tagName);
        }
    }
}