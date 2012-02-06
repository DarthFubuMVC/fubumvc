using System;
using FubuMVC.Core;

namespace FubuMVC.Tests.Registration.Conventions.Handlers.Posts.Create
{
    public class get_handler
    {
        public JsonResponse Execute(CreatePostRequestModel requestModel)
         {
             throw new NotImplementedException();
         }
    }

    public class CreatePostRequestModel // HTTP GET
    {
        [QueryString]
        public string Input { get; set; }
    }

    public class CreatePostInputModel // HTTP Post
    {
    }
}