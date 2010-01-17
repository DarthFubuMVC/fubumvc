namespace FubuMVC.HelloWorld.Controllers.LimitMethods
{
    public class LimitMethodsController
    {

        public GreetingViewModel GreetingCommand()
        {
            return new GreetingViewModel();
        }

        public GreetingViewModel QueryGreeting()
        {
            return new GreetingViewModel();
        }
    }
    public class GreetingViewModel { }
}