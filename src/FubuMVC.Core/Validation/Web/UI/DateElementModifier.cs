using System;
using FubuCore;
using FubuCore.Dates;
using FubuMVC.Core.UI.Elements;

namespace FubuMVC.Core.Validation.Web.UI
{
    public class DateElementModifier : InputElementModifier
    {
        public override bool Matches(ElementRequest token)
        {
            var type = token.Accessor.PropertyType;
            return type.CanBeCastTo<DateTime>() || type.CanBeCastTo<Date>();
        }

        protected override void modify(ElementRequest request)
        {
            request.CurrentTag.AddClass("date");
        }
    }
}