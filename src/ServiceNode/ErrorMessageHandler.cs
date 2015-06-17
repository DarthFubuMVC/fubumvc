using System.Reflection;

namespace ServiceNode
{
    public class ErrorMessageHandler
    {
        public void Handle(ErrorMessage message)
        {
            throw new AmbiguousMatchException();
        }
    }
}