using System;
using FubuMVC.Core.UI.Configuration;

namespace FubuMVC.Core.UI.Tags
{
    public class FormLambdaElementModifier : IFormElementModifier
    {
        private readonly Func<FormDef, bool> _matches;
        private readonly Func<FormDef, FormTagModifier> _modifierBuilder;

        public FormLambdaElementModifier(Func<FormDef, bool> matches, Func<FormDef, FormTagModifier> modifierBuilder)
        {
            _matches = matches;
            _modifierBuilder = modifierBuilder;
        }

        public FormTagModifier CreateModifier(FormDef accessorDef)
        {
            return _matches(accessorDef) ? _modifierBuilder(accessorDef) : null;
        }
    }
}