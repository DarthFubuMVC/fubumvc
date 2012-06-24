using FubuLocalization;

namespace FubuMVC.Core.UI.Navigation
{
    public interface IStringTokenMatcher
    {
        bool Matches(StringToken token);
        StringToken DefaultKey();

        string Description { get; }
    }
}