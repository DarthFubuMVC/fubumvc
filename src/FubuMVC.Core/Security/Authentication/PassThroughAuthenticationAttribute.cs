using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Security.Authentication
{
	public class PassThroughAuthenticationAttribute : ModifyChainAttribute
	{
	    public override void Alter(ActionCallBase call)
	    {
	        var chain = call.ParentChain();

            chain.Prepend(ActionFilter.For<PassThroughAuthenticationFilter>(a => a.Filter()));
	    }
	}
}