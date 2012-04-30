using System;

namespace FubuMVC.Core.View.Attachment
{
    public interface IViewProfile
    {
        Type ConditionType { get; }
        ViewBag Filter(ViewBag bag);
    }
}