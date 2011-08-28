using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Tags;

namespace FubuMVC.Core.Registration.DSL
{
    public class AssetsExpression
    {
        private readonly FubuRegistry _registry;

        public AssetsExpression(FubuRegistry registry)
        {
            _registry = registry;
        }

        /// <summary>
        /// If applyPolicy is true, this directs FubuMVC to throw up the
        /// Yellow Screen of Death to fail fast if unknown or missing
        /// assets are requested in any screen.  This mode is mostly
        /// appropriate for *development mode*
        /// </summary>
        /// <param name="applyPolicy"></param>
        /// <returns></returns>
        public AssetsExpression YSOD_on_missing_assets(bool applyPolicy)
        {
            if (applyPolicy)
            {
                setService<IMissingAssetHandler, YellowScreenMissingAssetHandler>();
            }

            return this;
        }

        /// <summary>
        /// Replace the IMissingAssetHandler with a custom implementation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public AssetsExpression HandleMissingAssetsWith<T>() where T : IMissingAssetHandler
        {
            setService<IMissingAssetHandler, T>();

            return this;
        }

        /// <summary>
        /// This directs FubuMVC to apply a *very* naive combination policy to create a combination
        /// for each unique set of requested assets (styles will only be combined if they are in the
        /// same folder).  
        /// </summary>
        /// <returns></returns>
        public AssetsExpression CombineAllUniqueAssetRequests()
        {
            setService<ICombinationDeterminationService, CombineAllUniqueSetsCombinationDeterminationService>();
            return this;
        }

        private void setService<TInterface, TConcrete>() where TConcrete : TInterface
        {
            _registry.Services(x => x.ReplaceService<TInterface, TConcrete>());
        }
    }
}