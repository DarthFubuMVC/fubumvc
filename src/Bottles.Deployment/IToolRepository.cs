namespace Bottles.Deployment
{
    public interface IToolRepository
    {
        void CopyTo(string toolName, string destination);
    }
}