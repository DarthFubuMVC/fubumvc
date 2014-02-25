using System;
using FubuCore;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.Registration.Conventions
{
    [MarkedForTermination]
    public class StringOutputPolicy : Policy
    {
        public StringOutputPolicy()
        {
            Where.ResourceTypeIs<string>();
            ModifyBy(chain => chain.Output.Add(new StringWriter()));
        }
    }

    [MarkedForTermination]
    public class HtmlStringOutputPolicy : Policy
    {
        public HtmlStringOutputPolicy()
        {
            Where.ResourceTypeIs<string>();
            Where.LastActionMatches(
                call => call.Method.Name.EndsWith("HTML", StringComparison.InvariantCultureIgnoreCase));

            Conneg.AddHtml();
        }
    }
}