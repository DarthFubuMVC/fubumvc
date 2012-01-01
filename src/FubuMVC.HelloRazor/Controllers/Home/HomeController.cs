namespace FubuMVC.HelloRazor.Controllers.Home
{
    public class HomeController
    {
        public HelloWorldViewModel SayHello(HelloWorldInputModel input)
        {
            return new HelloWorldViewModel {Message = "Hello World! FubuMVC + Razor"};
        }

        public HelloPartialViewModel SayHelloPartial(HelloPartialInputModel input)
        {
            return new HelloPartialViewModel {Message = "Hello from Partial"};
        }
        
    }

    public class HelloPartialViewModel
    {
        public string Message { get; set; }
    }

    public class HelloPartialInputModel
    {
    }

    public class HelloWorldInputModel
    {
    }

    public class HelloWorldViewModel
    {
        public string Message { get; set; }
    }
}