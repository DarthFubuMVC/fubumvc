using FubuMVC.Core;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Http;
using FubuMVC.OwinHost;

namespace Serenity.Jasmine
{
    public class SerenityJasmineRegistry : FubuRegistry
    {
        public SerenityJasmineRegistry()
        {
            IncludeDiagnostics(true);

            Actions.IncludeType<JasminePages>();
            Routes.HomeIs<JasminePages>(x => x.Home());

            Services(x =>
            {
                x.ReplaceService<ICombinationDeterminationService, NulloCombinationDeterminationService>();
                x.ReplaceService<ICurrentHttpRequest, OwinCurrentHttpRequest>();
            });
        }
    }
}