using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Navigation
{
    public interface IStringTokenMatcher
    {
        bool Matches(StringToken token);
        StringToken DefaultKey();

        string Description { get; }
    }
}