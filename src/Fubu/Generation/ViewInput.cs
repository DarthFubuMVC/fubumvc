using System.ComponentModel;

namespace Fubu.Generation
{
    public class ViewInput
    {
        [Description("Name of the view and matching model without file extension")]
        public string Name { get; set; }

        [Description("If specified, will make this actionless view applied to the given url pattern")]
        public string UrlFlag { get; set; }

        [Description("open the view model and view after generation")]
        public bool OpenFlag { get; set; }

        [Description("Use a different template file for this view")]
        public string TemplateFlag { get; set; }
    }
}