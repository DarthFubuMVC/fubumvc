namespace Serenity.Jasmine
{
    public interface ISpecVisitor
    {
        void Specification(Specification spec);
        void Folder(SpecificationFolder folder);
        void Graph(SpecificationGraph graph);
    }
}