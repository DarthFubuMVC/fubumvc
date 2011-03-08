using FubuCore.Reflection;
using FubuLocalization;

namespace FubuFastPack.Querying
{
    public class FilterRule
    {
        public Accessor Accessor { get; set; }
        public object Value { get; set; }
        public StringToken Operator { get; set; }
    }
}