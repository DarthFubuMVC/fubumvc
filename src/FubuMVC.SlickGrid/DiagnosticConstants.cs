using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.SlickGrid
{
    public static class DiagnosticConstants
    {
        public static readonly string UrlPrefix = "_diagnostics";

        public static readonly string GetDataMethodName =
            ReflectionHelper.GetMethod<IGridDataSource<BehaviorChain>>(x => x.GetData()).Name;
    }
}