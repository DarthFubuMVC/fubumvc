namespace FubuMVC.Core.UI.Configuration
{
    public abstract class AlwaysElementModifier : OptionalElementModifier
    {
        public override bool Matches(AccessorDef def)
        {
            return true;
        }
    }
}