namespace FubuMVC.Core.Security.Authentication
{
    public interface IPasswordHash
    {
        string CreateHash(string password);
    }
}