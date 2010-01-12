using System;

namespace FubuMVC.Core.Registration.DSL
{
    public class ChainFilterExpression
    {
        private readonly BehaviorVisitor _visitor;

        public ChainFilterExpression(BehaviorVisitor visitor)
        {
            _visitor = visitor;
        }

        // TODO -- add filters to the visitor to control which behaviors get the whatever

        public ChainFilterExpression WithoutAnyOutput()
        {
            throw new NotImplementedException();
        }
    }
}