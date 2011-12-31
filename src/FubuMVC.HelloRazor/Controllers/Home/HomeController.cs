namespace FubuMVC.HelloRazor.Controllers.Home
{
    public class HomeController
    {
         public HelloWorldViewModel SayHello(HelloWorldInputModel input)
         {
             return new HelloWorldViewModel{Message = "Hello World! FubuMVC + Razor"};
         }
    }

    public class HelloWorldInputModel
    {
    }

    public class HelloWorldViewModel
    {
        public string Message { get; set; }
    }
}