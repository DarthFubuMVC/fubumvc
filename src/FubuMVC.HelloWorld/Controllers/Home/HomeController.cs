using FubuMVC.Core.View;

namespace FubuMVC.HelloWorld.Controllers.Home
{
    public class HomeController
    {
        public HomeViewModel Home(HomeInputModel model)
        {
            return new HomeViewModel
            {
                Text = "Hello, world.",
                CurrentUrl = model.Url
            };
        }
    }

    public class HomeViewModel
    {
        public string Text { get; set; }
        public string CurrentUrl { get; set; }
    }

    public class HomeInputModel
    {
        public string Url { get; set; }
    }
    
    public class Home : FubuPage<HomeViewModel>
    {
    }
}