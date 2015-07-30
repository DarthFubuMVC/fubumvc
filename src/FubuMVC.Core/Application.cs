using FubuMVC.Core.Http.Hosting;

namespace FubuMVC.Core
{
    public class Application<T> where T : FubuRegistry, new()
    {
         public readonly T Registry;

        public Application(T registry)
        {
            Registry = registry;
        }

        public Application()
        {
            Registry = new T();
        }

        public string RootPath;
        //public int Port;
        //public IHost Host;
        //public string Mode;


    }

    public class BasicApplication : Application<FubuRegistry>
    {
        public BasicApplication(FubuRegistry registry) : base(registry)
        {
        }

        public BasicApplication()
        {
        }
    }
}