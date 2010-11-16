using System;
using FubuMVC.Core.UI.Forms;

namespace FubuMVC.Core.UI.Tags
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
            BeforePartial = new TagFactory();
            AfterPartial = new TagFactory();
            BeforeEachOfPartial = new PartialTagFactory();
            AfterEachOfPartial = new PartialTagFactory();
        }

        public string Name { get { return _name; } }

        public TagFactory Label { get; private set; }
        public TagFactory Display { get; private set; }
        public TagFactory Editor { get; private set; }
        public TagFactory BeforePartial { get; private set; }
        public TagFactory AfterPartial { get; private set; }

        public PartialTagFactory BeforeEachOfPartial { get; private set; }
        public PartialTagFactory AfterEachOfPartial { get; private set; }


        public void Import(TagProfile peer)
        {
            Label.Merge(peer.Label);
            Display.Merge(peer.Display);
            Editor.Merge(peer.Editor);
            BeforePartial.Merge(peer.BeforePartial);
            AfterPartial.Merge(peer.AfterPartial);
            BeforeEachOfPartial.Merge(peer.BeforeEachOfPartial);
            AfterEachOfPartial.Merge(peer.AfterEachOfPartial);
        }

        private Func<ILabelAndFieldLayout> _layoutBuilder = () => new DefinitionListLabelAndField();

        public ILabelAndFieldLayout NewLabelAndFieldLayout()
        {
            return _layoutBuilder();
        }

        public void UseLabelAndFieldLayout<T>() where T : ILabelAndFieldLayout, new()
        {
            _layoutBuilder = () => new T();
        }

        public void UseLabelAndFieldLayout(Func<ILabelAndFieldLayout> layoutBuilder)
        {
            _layoutBuilder = layoutBuilder;
        }
    }
}