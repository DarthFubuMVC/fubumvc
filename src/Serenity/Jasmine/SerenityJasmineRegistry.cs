using FubuMVC.Core;
using FubuMVC.Core.Assets.Combination;

namespace Serenity.Jasmine
{
    public class SerenityJasmineRegistry : FubuRegistry
    {
        public SerenityJasmineRegistry()
        {
            Actions.IncludeType<JasminePages>();
            Routes.HomeIs<JasminePages>(x => x.Home());

            Services(x =>
            {
                x.ReplaceService<ICombinationDeterminationService, NulloCombinationDeterminationService>();
            });
        }
    }
}