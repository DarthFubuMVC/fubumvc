using System.Linq;
using FubuCore.Reflection;

namespace FubuMVC.Core.Validation.Fields
{
    public class IsValid : IFieldRuleCondition
    {
        public bool Matches(Accessor accessor, ValidationContext context)
        {
            return !context.Notification.MessagesFor(accessor).Any();
        }
    }
}