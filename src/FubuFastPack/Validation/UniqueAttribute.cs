using System;

namespace FubuFastPack.Validation
{
    public class UniqueAttribute : Attribute
    {
        public string Key { get; set; }
    }
}