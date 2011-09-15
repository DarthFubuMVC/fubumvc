namespace FubuMVC.Core.Projections
{
    public interface IMediaNode
    {
        IMediaNode AddChild(string name);
        void SetAttribute(string name, object value);



    }
}