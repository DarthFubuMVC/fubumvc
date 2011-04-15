namespace Bottles.Deployment
{
    /// <summary>
    /// This represents an absraction of the profile zip/package in its entirity.
    /// </summary>
    public interface IProfile
    {
        string GetPathForBottle(string bottleName);
    }
}