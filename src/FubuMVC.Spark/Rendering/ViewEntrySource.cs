﻿using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface IViewEntrySource
    {
        ISparkViewEntry GetViewEntry();
        ISparkViewEntry GetPartialViewEntry();
    }

    public class ViewEntrySource : IViewEntrySource
    {
        private readonly IViewEntryProviderCache _provider;
        private readonly ViewDefinition _viewDefinition;

        public ViewEntrySource(ViewDefinition viewDefinition, IViewEntryProviderCache provider)
        {
            _provider = provider;
            _viewDefinition = viewDefinition;
        }

        public ISparkViewEntry GetViewEntry()
        {
            return _provider.GetViewEntry(_viewDefinition.ViewDescriptor);
        }

        public ISparkViewEntry GetPartialViewEntry()
        {
            return _provider.GetViewEntry(_viewDefinition.PartialDescriptor);
        }
    }
}