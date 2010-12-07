using System;
using FubuMVC.Core.UI.Configuration;

namespace FubuMVC.Core.UI.Tags
{
    public class PartialLambdaElementModifier : IPartialElementModifier
    {
        private readonly Func<AccessorDef, bool> _matches;
        private readonly Func<AccessorDef, EachPartialTagModifier> _modifierBuilder;

        public PartialLambdaElementModifier(Func<AccessorDef, bool> matches, Func<AccessorDef, EachPartialTagModifier> modifierBuilder)
        {
            _matches = matches;
            _modifierBuilder = modifierBuilder;
        }

        public EachPartialTagModifier CreateModifier(AccessorDef accessorDef)
        {
            return _matches(accessorDef) ? _modifierBuilder(accessorDef) : null;
        }
    }
}