using System;
using FubuCore.Descriptions;
using StructureMap;

namespace FubuMVC.Core.Diagnostics
{

    public interface IClientSideView
    {
        object Convert(object target);
    }

    public interface IClientSideView<T> : IClientSideView
    {
        
    }

    public class DescriptionClientSideView<T> : IClientSideView<T>
    {
        public object Convert(object target)
        {
            return Description.For(target).ToDictionary();
        }
    }

    public class PassthroughClientSideView<T> : IClientSideView<T>
    {
        public object Convert(object target)
        {
            return target;
        }
    }

    public class ClientSideViewWriter
    {
        private readonly IContainer _container;

        public ClientSideViewWriter(IContainer container)
        {
            _container = container;
        }

        public object Convert(object target)
        {
            if (target == null) throw new ArgumentNullException("target");

            var converter = _container
                .ForGenericType(typeof (IClientSideView<>))
                .WithParameters(target.GetType())
                .GetInstanceAs<IClientSideView>();

            return converter.Convert(target);
        }
    }
}