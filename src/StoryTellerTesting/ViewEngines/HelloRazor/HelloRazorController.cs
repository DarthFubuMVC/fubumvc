namespace IntegrationTesting.ViewEngines.HelloRazor
{
    //Currently the extra mvc references are needed for tooling only
    //This requirement will be removed at some point
    public class HelloRazorController
    {
        public HelloWorldRazorViewModel SayHello(HelloWorldRazorInputModel input)
        {
            return new HelloWorldRazorViewModel {Message = "Hello World! FubuMVC + Razor"};
        }

        public HelloRazorPartialViewModel SayHelloPartial(HelloRazorPartialInputModel input)
        {
            return new HelloRazorPartialViewModel {Message = "Hello from Partial"};
        }
        
    }

    public class HelloRazorPartialViewModel
    {
        public string Message { get; set; }
    }

    public class HelloRazorPartialInputModel
    {
    }

    public class HelloWorldRazorInputModel
    {
    }

    public class HelloWorldRazorViewModel
    {
        public string Message { get; set; }
    }
}