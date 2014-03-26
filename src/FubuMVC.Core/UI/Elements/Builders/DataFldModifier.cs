using System.ComponentModel;

namespace FubuMVC.Core.UI.Elements.Builders
{
    [Description("Adds a @data-fld attribute for the field name to aid in automated testing scenarios")]
    public class DataFldModifier : IElementModifier
    {
        public bool Matches(ElementRequest token)
        {
            return true;
        }

        public void Modify(ElementRequest request)
        {
            request.OriginalTag.Attr("data-fld", request.ElementId);
        }
    }
}