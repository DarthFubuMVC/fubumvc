using System.Collections.Generic;

namespace Serenity.Jasmine
{
    public interface ISpecNode
    {
        SpecPath Path();
        IEnumerable<Specification> AllSpecifications { get; }
        IEnumerable<ISpecNode> AllNodes { get; }
        string FullName { get; }


        IEnumerable<ISpecNode> ImmediateChildren
        { 
            get;
        }

        string TreeClass { get; }

        ISpecNode Parent();

        void AcceptVisitor(ISpecVisitor visitor);
    }
}