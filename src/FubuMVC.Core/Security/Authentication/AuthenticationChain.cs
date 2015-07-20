using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Security.Authentication
{
    public class AuthenticationChain : Chain<AuthenticationNode, AuthenticationChain>
    {
    }
}