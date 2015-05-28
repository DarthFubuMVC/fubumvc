using FubuMVC.Core.Security;

namespace FubuMVC.Authentication.IntegrationTesting
{
    public class TargetModel { }

    [NotAuthenticated]
    public class PublicModel
    {
        public string Message { get; set; }
    }

    public class SampleController
    {
        public TargetModel get_some_authenticated_route(TargetModel target)
        {
            return target;
        }

        public string get_say_Message(PublicModel request)
        {
            return request.Message;
        }
    }
}