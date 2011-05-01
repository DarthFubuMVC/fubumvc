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
            : base(new T(), new FubuPage<T>())
        {
            Container = container;
            Request = new InMemoryFubuRequest();
            container.Inject(Request);

            Request.Set(Model);

            container.BuildUp(Page);

            Page.SetModel(Request);
        }

        public IFubuRequest Request { get; private set; }
        public IContainer Container { get; private set; }
    }
}