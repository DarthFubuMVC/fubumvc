namespace FubuFastPack.Crud
{
    public class CreationRequest<T> where T : EditEntityModel
    {
        private readonly T _input;

        public CreationRequest(T input)
        {
            _input = input;
        }

        public T Input
        {
            get { return _input; }
        }
    }
}