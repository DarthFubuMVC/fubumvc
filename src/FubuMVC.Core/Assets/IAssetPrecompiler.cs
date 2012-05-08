using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets
{
    public interface IAssetPrecompiler
    {
        void Precompile(IAssetPipeline pipeline, IAssetRegistration registration);
    }
}