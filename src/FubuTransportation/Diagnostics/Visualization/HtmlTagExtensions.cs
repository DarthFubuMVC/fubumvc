using HtmlTags;

namespace FubuTransportation.Diagnostics.Visualization
{
    public static class HtmlTagExtensions
    {
        public static LiteralTag ToLiteral(this object target)
        {
            return new LiteralTag(target.ToString());
        }
    }
}