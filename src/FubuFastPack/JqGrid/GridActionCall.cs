using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace FubuFastPack.JqGrid
{
    public class GridActionCall<T> : ActionCall where T : ISmartGrid
    {
        public GridActionCall()
            : base(typeof(SmartGridHarness<T>), ReflectionHelper.GetMethod<SmartGridHarness<T>>(x => x.Data(null)))
        {
        }
    }
}