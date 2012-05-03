using System;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.View.Attachment
{
    /// <summary>
    /// More or less a "nullo" object
    /// </summary>
    public class DefaultProfile : IViewProfile
    {
        public Type ConditionType
        {
            get { return typeof(Always); }
        }

        public ViewBag Filter(ViewBag bag)
        {
            return bag;
        }
    }
}