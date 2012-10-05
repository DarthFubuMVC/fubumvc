namespace ViewEngineIntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.HelloPartial
{
    public class HelloPartialEndpoint
    {
         public HelloPartialViewModel SayHelloPartial(HelloPartialInputModel input)
         {
             return new HelloPartialViewModel();
         }
         public HelloPartialViewModel Render()
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