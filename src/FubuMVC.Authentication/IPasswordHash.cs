namespace FubuMVC.Authentication
{
    public interface IPasswordHash
    {
        string CreateHash(string password);
    }
}