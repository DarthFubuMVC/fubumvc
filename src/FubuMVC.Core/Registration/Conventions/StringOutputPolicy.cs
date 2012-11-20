using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.Registration.Conventions
{
    public class StringOutputPolicy : Policy
    {
        public StringOutputPolicy()
        {
            Where.ResourceTypeIs<string>();
            Conneg.AddWriter<WriteString>();
        }
    }

    public class HtmlStringOutputPolicy : Policy
    {
        public HtmlStringOutputPolicy()
        {
            Where.ResourceTypeIs<string>();
            Where.LastActionMatches(call => call.Method.Name.EndsWith("HTML", StringComparison.InvariantCultureIgnoreCase));

            Conneg.AddHtml();
        }
    }


}