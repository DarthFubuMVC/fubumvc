namespace FubuMVC.HelloWorld.Controllers.Home
{
    public class HomeController
    {
        public HomeViewModel Home(HomeInputModel model)
        {
            return new HomeViewModel
            {
                Text = "Hello, world."
            };
        }
    }

    public class HomeViewModel
    {
        public string Text { get; set; }
    }

    public class HomeInputModel
    {
    }
}