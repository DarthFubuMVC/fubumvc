using System.Collections.Generic;
using FubuCore.Reflection;
using FubuLocalization;

namespace FubuFastPack.Querying
{
    public class FilteredProperty
    {
        public Accessor Accessor { get; set; }
        public string Header { get; set; }

        public IEnumerable<StringToken> Operators { get; set; }
    }
}