using System;
using FubuMVC.Core.UI.Forms;
using FubuMVC.Core.UI.Security;

namespace FubuMVC.Core.UI.Tags
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


            Form = new FormTagFactoryExpression(profile.Form);
            BeforePartial = new TagFactoryExpression(profile.BeforePartial);
            AfterPartial = new TagFactoryExpression(profile.AfterPartial);
            BeforeEachOfPartial = new PartialTagFactoryExpression(profile.BeforeEachOfPartial);
            AfterEachOfPartial = new PartialTagFactoryExpression(profile.AfterEachOfPartial);
        }

 

        public void DegradeAccessToFields()
        {
            _profile.Editor.InsertFirstBuilder(new DegradingAccessElementBuilder());
        }

        protected TagProfile profile { get { return _profile; } }

        public TagFactoryExpression Labels { get; private set; }
        public TagFactoryExpression Editors { get; private set; }
        public TagFactoryExpression Displays { get; private set; }

        public FormTagFactoryExpression Form { get; private set; }
        public TagFactoryExpression BeforePartial { get; private set; }
        public TagFactoryExpression AfterPartial { get; private set; }
        public PartialTagFactoryExpression BeforeEachOfPartial { get; private set; }
        public PartialTagFactoryExpression AfterEachOfPartial { get; private set; }

        public void UseLabelAndFieldLayout<T>() where T : ILabelAndFieldLayout, new()
        {
            _profile.UseLabelAndFieldLayout<T>();
        }

        public void UseLabelAndFieldLayout(Func<ILabelAndFieldLayout> layoutBuilder)
        {
            _profile.UseLabelAndFieldLayout(layoutBuilder);
        }
    }
}