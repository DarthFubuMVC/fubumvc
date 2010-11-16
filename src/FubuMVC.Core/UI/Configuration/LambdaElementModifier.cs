using System;

namespace FubuMVC.Core.UI.Configuration
{
    public class LambdaElementModifier : IElementModifier
    {
        private readonly Func<AccessorDef, bool> _matches;
        private readonly Func<AccessorDef, TagModifier> _modifierBuilder;

        public LambdaElementModifier(Func<AccessorDef, bool> matches, Func<AccessorDef, TagModifier> modifierBuilder)
        {
            _matches = matches;
            _modifierBuilder = modifierBuilder;
        }

        public TagModifier CreateModifier(AccessorDef accessorDef)
        {
            return _matches(accessorDef) ? _modifierBuilder(accessorDef) : null;
        }
    }
}