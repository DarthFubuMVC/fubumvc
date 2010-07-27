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

        public HelloViewModel Hello(HelloInputModel model)
        {
            return new HelloViewModel{Text = "Hello from Hello()"};
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

    public class HelloViewModel
    {
        public string Text{ get; set;}
    }

    public class HelloInputModel
    {
        
    }
    
    public class Home : FubuPage<HomeViewModel>
    {
    }
}