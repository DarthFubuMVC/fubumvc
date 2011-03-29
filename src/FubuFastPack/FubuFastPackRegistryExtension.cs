using System;
using FubuCore;
using FubuFastPack.Binding;
using FubuFastPack.Crud;
using FubuFastPack.JqGrid;
using FubuFastPack.Querying;
using FubuMVC.Core;

namespace FubuFastPack
{
    public class FubuFastPackRegistryExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services(x => x.SetServiceIfNone<IObjectConverter, FastPackObjectConverter>());
            registry.Services(x => x.SetServiceIfNone<IQueryService, QueryService>());
            registry.Services(x => x.SetServiceIfNone<ISmartGridService, SmartGridService>());

            registry.Models
                //.BindModelsWith<EditEntityModelBinder>()
                //.BindModelsWith<EntityModelBinder>()
                .ConvertUsing<EntityConversionFamily>();
        }
    }
}