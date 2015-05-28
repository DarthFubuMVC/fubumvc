using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Authentication
{
    public class AuthenticationChain : Chain<AuthenticationNode, AuthenticationChain>
    {
    }
}