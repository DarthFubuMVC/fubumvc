namespace FubuMVC.Core
{
    /// <summary>
    /// Implementations of this contract interact with a <see cref="FubuRegistry"/>
    /// instance
    /// </summary>
    public interface IFubuRegistryExtension
    {
        void Configure(FubuRegistry registry);
    }
}