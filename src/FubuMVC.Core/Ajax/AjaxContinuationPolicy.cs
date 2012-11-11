using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Ajax
{
    public class AjaxContinuationPolicy : Policy
    {
        public AjaxContinuationPolicy()
        {
            Where.ResourceTypeImplements<AjaxContinuation>();

            Conneg.AllowHttpFormPosts();
            Conneg.AcceptJson();

            Conneg.ClearAllWriters();
            Conneg.AddWriter(typeof (AjaxContinuationWriter<>));
        }
    }


}