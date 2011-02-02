namespace TestPackage4
{
	public class HelloSparkController
	{
		public HelloWorld HelloWorld(HelloWorldRequest request)
		{
			return new HelloWorld { Message = string.IsNullOrWhiteSpace(request.Message) ?  "Hello, World!" : request.Message };
		}
	}
}