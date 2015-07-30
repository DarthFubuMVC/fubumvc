namespace FubuMVC.Core
{
    public class Application<T> where T : FubuRegistry, new()
    {
         public readonly T Registry = new T(); 
    }

    public class BasicApplication : Application<FubuRegistry>
    {
        
    }
}