namespace FubuMVC.UI.Tags
{
    public class TagProfileExpression
    {
        private readonly TagProfile _profile;

        public TagProfileExpression(TagProfile profile)
        {
            _profile = profile;
            Labels = new TagFactoryExpression(profile.Label);
            Editors = new TagFactoryExpression(profile.Editor);
            Displays = new TagFactoryExpression(profile.Display);
        }

        protected TagProfile profile { get { return _profile; } }

        public TagFactoryExpression Labels { get; private set; }
        public TagFactoryExpression Editors { get; private set; }
        public TagFactoryExpression Displays { get; private set; }
    }
}