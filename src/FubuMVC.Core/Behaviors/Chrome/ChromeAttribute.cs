using System;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Behaviors.Chrome
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ChromeAttribute : ModifyChainAttribute
    {
        private readonly Type _contentType;

        public ChromeAttribute(Type contentType)
        {
            if (!contentType.CanBeCastTo<ChromeContent>())
            {
                throw new ArgumentOutOfRangeException("contentType", "contentType must be ChromeContent or a subclass of ChromeContent");
            }

            _contentType = contentType;
        }

        public string Title { get; set; }

        public override void Alter(ActionCall call)
        {
            call.AddBefore(new ChromeNode(_contentType){
                Title = () => Title
            });
        }
    }
}