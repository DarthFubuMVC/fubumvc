namespace FubuFastPack.Persistence
{
    public class EntityFinder : IEntityFinder
    {
        private readonly IEntityFinderCache _cache;

        public EntityFinder(IEntityFinderCache cache)
        {
            _cache = cache;
        }

        public T Find<T>(IRepository repository, string text)
        {
            return _cache.FinderFor<T>()(repository, text);
        }
    }
}