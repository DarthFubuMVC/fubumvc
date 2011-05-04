using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using StructureMap;

namespace FubuMVC.StructureMap
{
    /// <summary>
    /// Used for automated testing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StructureMapPageHarness<T> : PageHarness<T> where T : class, new()
    {
        public StructureMapPageHarness(IContainer container)
            : base(new T(), new SimpleFubuPage<T>())
        {
            Container = container;
            Request = new InMemoryFubuRequest();
            container.Inject(Request);

            Request.Set(Model);

            Page.ServiceLocator = new StructureMapServiceLocator(Container);

            Page.Model = Model;
        }

        public IFubuRequest Request { get; private set; }
        public IContainer Container { get; private set; }
    }
}