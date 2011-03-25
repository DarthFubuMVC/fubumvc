using System;

namespace FubuFastPack.Lists
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ListValueAttribute : Attribute
    {
        private readonly string _listName;

        public ListValueAttribute(string listName)
        {
            _listName = listName;
        }

        public virtual string GetListName(Type type)
        {
            return _listName;
        }
    }
}