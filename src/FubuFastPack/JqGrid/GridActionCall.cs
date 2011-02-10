using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace FubuFastPack.JqGrid
{
    public class GridActionCall<T> : ActionCall where T : ISmartGrid
    {
        public GridActionCall()
            : base(typeof(SmartGridController<T>), ReflectionHelper.GetMethod<SmartGridController<T>>(x => x.Data(null)))
        {
        }
    }
}