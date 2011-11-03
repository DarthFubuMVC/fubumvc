namespace FubuMVC.Core.Assets
{
    // TODO -- for your own sake, add xml comments here
    public interface IAssetRegistration
    {
        void Alias(string name, string alias);
        /// <summary>
        /// Example dsl would be:
        ///     jquery.temp.js requires jquery
        /// 
        /// Usage: 
        ///     _registration.Dependency("jquery.temp.js", "jquery");
        /// </summary>
        /// <param name="dependent">In this case dependent would be jquery.temp.js</param>
        /// <param name="dependency">In this case dependency would be jquery</param>
        void Dependency(string dependent, string dependency);
        void Extension(string extender, string @base);
        void AddToSet(string setName, string name);
        void Preceeding(string beforeName, string afterName);

        void AddToCombination(string comboName, string names);
        void ApplyPolicy(string typeName);
    }
}