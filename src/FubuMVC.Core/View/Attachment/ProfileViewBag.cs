using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.View.Attachment
{
    public class ProfileViewBag 
    {

        public ProfileViewBag(IViewProfile profile, ViewBag views)
        {
            Condition = profile.Condition;
            Views = profile.Filter(views);
        }

        public ViewBag Views { get; private set; }
        public IConditional Condition { get; private set; }

    }
}