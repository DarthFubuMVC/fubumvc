using FubuMVC.Core;

namespace FubuMVC.Tests.Registration.Conventions.Handlers.Posts.Sub.Route
{
    public class PostHandler
    {
        public PostHandlerViewModel Execute(ViewPostHandlerRequestModel request)
        {
            return new PostHandlerViewModel();
        }
    }

    public class PostHandlerViewModel
    {
    }

    public class ViewPostHandlerRequestModel
    {
        [QueryString]
        public string Optional { get; set; }
    }
}