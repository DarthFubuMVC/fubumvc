using System;

namespace FubuMVC.Core.UI.Configuration
{
    public class PartialLambdaElementBuilder : IPartialElementBuilder
    {
        private readonly Func<AccessorDef, EachPartialTagBuilder> _builderCreator;
        private readonly Func<AccessorDef, bool> _matches;

        public PartialLambdaElementBuilder(Func<AccessorDef, bool> matches, Func<AccessorDef, EachPartialTagBuilder> builderCreator)
        {
            _matches = matches;
            _builderCreator = builderCreator;
        }

        public EachPartialTagBuilder CreateInitial(AccessorDef accessorDef)
        {
            return _matches(accessorDef) ? _builderCreator(accessorDef) : null;
        }
    }
}