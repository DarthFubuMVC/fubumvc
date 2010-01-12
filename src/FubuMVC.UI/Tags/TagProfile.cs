namespace FubuMVC.UI.Tags
{
    public class TagProfile
    {
        public static readonly string DEFAULT = "DEFAULT";
        private readonly string _name;

        public TagProfile(string name)
        {
            _name = name;
            Label = new TagFactory();
            Display = new TagFactory();
            Editor = new TagFactory();
        }

        public string Name { get { return _name; } }

        public TagFactory Label { get; private set; }
        public TagFactory Display { get; private set; }
        public TagFactory Editor { get; private set; }

        public void Import(TagProfile peer)
        {
            Label.Merge(peer.Label);
            Display.Merge(peer.Display);
            Editor.Merge(peer.Editor);
        }
    }
}