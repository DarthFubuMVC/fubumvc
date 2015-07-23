using StructureMap.AutoMocking;

namespace FubuMVC.Tests.TestSupport
{
    /// <summary>
    /// Provides an "Auto Mocking Container" for the concrete class TARGETCLASS using Rhino.Mocks
    /// </summary>
    /// <typeparam name="T">The concrete class being tested</typeparam>
    public class RhinoAutoMocker<T> : AutoMocker<T> where T : class
    {
        public RhinoAutoMocker()
            : base(new RhinoMocksAAAServiceLocator())
        {
        }
    }
}