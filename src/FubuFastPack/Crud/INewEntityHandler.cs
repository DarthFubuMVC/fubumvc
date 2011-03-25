namespace FubuFastPack.Crud
{
    public interface INewEntityHandler<T> where T : EditEntityModel
    {
        void HandleNew(T target);
    }
}