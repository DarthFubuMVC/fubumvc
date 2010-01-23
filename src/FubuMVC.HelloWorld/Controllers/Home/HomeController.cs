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

        public HomeViewModel Test(HomeInputModel model)
        {
            return new HomeViewModel
            {
                Text = "Hello, world."
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
    }

    public class HomeInputModel
    {
    }

    public class HelloViewModel
    {
        public string Text{ get; set;}
    }

    public class HelloInputModel
    {
        
    }
}