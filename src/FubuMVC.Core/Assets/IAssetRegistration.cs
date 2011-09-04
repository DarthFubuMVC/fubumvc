namespace FubuMVC.Core.Assets
{
    // TODO -- for your own sake, add xml comments here
    public interface IAssetRegistration
    {
        void Alias(string name, string alias);
        void Dependency(string dependent, string dependency);
        void Extension(string extender, string @base);
        void AddToSet(string setName, string name);
        void Preceeding(string beforeName, string afterName);
    }
}