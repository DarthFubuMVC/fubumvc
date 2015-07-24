using StructureMap.Pipeline;

namespace FubuMVC.RavenDb.RavenDb.Multiple
{
    public class DocumentStoreInstance<T> : LambdaInstance<DocumentStore<T>> where T : RavenDbSettings
    {
        public DocumentStoreInstance() : base("Builds the IDocumentStore for settings class " + typeof (T).Name,
            c => {
                var inner = c.GetInstance<DocumentStoreBuilder<T>>().Build();
                return new DocumentStore<T>(inner);    
            })
        {
        }

    }
}