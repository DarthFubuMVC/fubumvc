namespace FubuMVC.IntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.HelloPartial
{
    public class HelloPartialController
    {
         public HelloPartialViewModel SayHelloPartial(HelloPartialInputModel input)
         {
             return new HelloPartialViewModel();
         }
    }

    public class HelloPartialInputModel
    {
    }

    public class HelloPartialViewModel
    {
    }
}