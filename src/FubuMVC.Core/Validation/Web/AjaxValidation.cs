using System.Collections.Generic;
using FubuMVC.Core.Ajax;

namespace FubuMVC.Core.Validation.Web
{
    public class AjaxValidation
    {
         public static AjaxContinuation BuildContinuation(Notification notification)
         {
             var continuation = new AjaxContinuation();
             continuation.Success = notification.IsValid();
             notification
                 .ToValidationErrors()
                 .Each(e => continuation
                                .Errors
                                .Add(new AjaxError
                                {
                                    field = e.field,
                                    label = e.label,
                                    message = e.message
                                }));

             return continuation;
         }
    }
}