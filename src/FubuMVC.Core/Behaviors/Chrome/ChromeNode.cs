using System;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Behaviors.Chrome
{
    public class ChromeNode : Wrapper
    {
        private readonly Type _contentType;

        public ChromeNode(Type contentType) : base(typeof(ChromeBehavior<>).MakeGenericType(contentType))
        {
            _contentType = contentType;
        }

        public Type ContentType
        {
            get
            {
                return _contentType;
            }
        }
    }
}