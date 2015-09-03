using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Security.Authentication
{
    public class AuthenticationFilterNode : ActionCallBase
    {
        public new static MethodInfo Method = ReflectionHelper.GetMethod<AuthenticationFilter>(x => x.Authenticate());

        public AuthenticationFilterNode() : base(typeof (AuthenticationFilter), Method)
        {
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Authentication; }
        }
    }
}