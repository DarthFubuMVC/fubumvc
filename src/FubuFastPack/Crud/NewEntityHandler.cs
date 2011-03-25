namespace FubuFastPack.Crud
{
    public class NewEntityHandler<T> : INewEntityHandler<T> where T : EditEntityModel
    {
        public void HandleNew(T target)
        {
            // no-op
        }
    }
}