namespace FubuMVC.Tests.Registration.Conventions.Handlers.Posts
{
    public class get_Year_Month_Title_handler
    {
        public PostDetailsViewModel Execute(ViewPostRequestModel request)
        {
            return new PostDetailsViewModel();
        }
    }

    public class ViewPostRequestModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string Title { get; set; }
    }

    public class PostDetailsViewModel
    {
    }
}