using FubuMVC.Core.Registration;

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