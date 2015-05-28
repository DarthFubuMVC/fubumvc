using System.Collections.Generic;

namespace FubuPersistence.RavenDb.Multiple
{
    public class DocumentStoreBuilder<T> : DocumentStoreBuilder where T : RavenDbSettings
    {
        public DocumentStoreBuilder(T settings, IEnumerable<IDocumentStoreConfigurationAction<T>> configurations) : base(settings, configurations)
        {
        }
    }
}