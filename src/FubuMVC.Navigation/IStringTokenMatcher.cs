using FubuLocalization;

namespace FubuMVC.Navigation
{
    public interface IStringTokenMatcher
    {
        bool Matches(StringToken token);
        StringToken DefaultKey();

        string Description { get; }
    }
}