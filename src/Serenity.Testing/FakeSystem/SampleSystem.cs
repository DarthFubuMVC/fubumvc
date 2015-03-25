using FubuMVC.Core;
using Serenity;

namespace Serenity.Testing.FakeSystem
{
    public class SampleSystem : FubuMvcSystem<MyApplication>
    {
         
    }

    public class MyApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            throw new System.NotImplementedException();
        }
    }
}