namespace Bottles.Deployment
{
    public interface IBottleRepository
    {
        void CopyTo(string botttleName, string destination);
        void ExplodeTo(string bottleName, string destination);
    }
}