using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FubuMVC.Core.UI.Configuration;
using HtmlTags;
using HtmlTags.Extended.Attributes;

namespace FubuMVC.Core.Security.AntiForgery
{
    public class AntiForgeryFormModifier : IFormElementModifier
    {
        public FormTagModifier CreateModifier(FormDef accessorDef)
        {
            if (accessorDef.IsInBound)
            {
                return (req, tag) =>
                           {
                               var antiForgeryNode = req.TargetChain.OfType<AntiForgeryNode>().FirstOrDefault();
                               if (antiForgeryNode == null)
                               {
                                   return;
                               }

                               var antiForgeryService = req.Get<IAntiForgeryService>();
                               var cookieToken = antiForgeryService.SetCookieToken(null, null);
                               var formToken = antiForgeryService.GetFormToken(cookieToken, antiForgeryNode.Salt);
                               tag.Children.Add(new HiddenTag().Name(formToken.Name).Value(formToken.TokenString));
                           };
            }

            return null;
        }
    }
}
