using FubuCore;

namespace FubuMVC.HelloWorld.Hello
{
    public class get_Name_handler
    {
        public string  Execute(SayHelloRequestModel request)
        {
            return "Hello, {0}".ToFormat(request.Name);
        }
    }

    public class SayHelloRequestModel
    {
        public string Name { get; set; }
    }
}