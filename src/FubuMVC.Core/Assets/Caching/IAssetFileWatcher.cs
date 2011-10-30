namespace FubuMVC.Core.Assets.Caching
{
    public interface IAssetFileWatcher
    {
        void StartWatchingAll();
        void StopWatching();
    }
}