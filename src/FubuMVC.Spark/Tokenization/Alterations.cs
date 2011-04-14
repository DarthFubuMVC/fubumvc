using FubuMVC.Spark.Tokenization.Model;

namespace FubuMVC.Spark.Tokenization
{
    public interface ISparkFileAlteration
    {
        void Alter(SparkFile file);
    }

    public class NamespaceAlteration : ISparkFileAlteration
    {
        private readonly INamespaceResolver _resolver;

        public NamespaceAlteration(INamespaceResolver resolver)
        {
            _resolver = resolver;
        }

        public void Alter(SparkFile file)
        {
            file.Namespace = _resolver.Resolve(file);
        }
    }

    public class ViewModelAlteration : ISparkFileAlteration
    {
        private readonly IViewModelTypeResolver _resolver;

        public ViewModelAlteration(IViewModelTypeResolver resolver)
        {
            _resolver = resolver;
        }

        public void Alter(SparkFile file)
        {
            file.ViewModel = _resolver.Resolve(file);
        }
    }
}