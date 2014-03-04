using System;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.View.Attachment
{
    /// <summary>
    /// More or less a "nullo" object
    /// </summary>
    public class DefaultProfile : IViewProfile
    {
        public IConditional Condition
        {
            get { return Always.Flyweight; }
        }

        public ViewBag Filter(ViewBag bag)
        {
            return bag;
        }
    }
}