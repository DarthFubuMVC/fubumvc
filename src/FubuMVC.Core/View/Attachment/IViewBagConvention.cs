using System;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.View.Attachment
{
    public interface IViewBagConvention
    {
        void Configure(ViewBag bag, BehaviorGraph graph);
    }
}