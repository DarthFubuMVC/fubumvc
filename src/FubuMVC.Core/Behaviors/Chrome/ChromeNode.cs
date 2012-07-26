using System;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Behaviors.Chrome
{
    public class ChromeNode : Wrapper
    {
        public ChromeNode(Type contentType) : base(typeof(ChromeBehavior<>).MakeGenericType(contentType))
        {
        }
    }
}