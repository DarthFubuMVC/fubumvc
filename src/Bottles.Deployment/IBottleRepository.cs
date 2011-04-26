namespace Bottles.Deployment
{
    public interface IBottleRepository
    {
        void CopyTo(string bottleName, string destination);
        void ExplodeTo(string bottleName, string destination);
    }
}