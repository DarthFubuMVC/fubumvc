namespace FubuMVC.Core.UI.Scripts
{
    public interface IScript : IScriptObject
    {
        bool ShouldBeAfter(IScript script);

        void OrderedAfter(IScript script);
        void OrderedBefore(IScript script);
    }
}